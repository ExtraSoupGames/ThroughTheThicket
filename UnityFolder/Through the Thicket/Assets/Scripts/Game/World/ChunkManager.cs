using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
public struct ChunkPos
{
    public int X;
    public int Y;

    public ChunkPos(int x, int y)
    {
        X = x;
        Y = y;
    }
}
public abstract class ChunkManager : MonoBehaviour
{
    //the distance, in tiles, around the player in which to render tiles
    [SerializeField] private int RenderDistance;
    //the persistent data path, in native array format, to be passed to jobs
    NativeArray<char> persistentDataPath;
    //track players location to determine which chunks need to be loaded
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform tileParent;
    [SerializeField] private GameObject tilePrefab;
    //locations where chunks are needed
    private NativeQueue<ChunkPos> chunksToLoad;
    //queue for tiles that need to be loaded
    private NativeQueue<Tile> tilesToLoad;
    //list of every currently loaded tile being rendered
    private List<ProcessedTileData> loadedTiles;
    //pool of tile objects to render the world with
    private GameObject[] tilePool;
    //all chunks being currently loaded
    List<ChunkPos> activeChunks;
    //JobHandles to track other threads
    JobHandle ChunkGrabber;
    //seed for the world generator
    private int worldSeed;
    private TileDisplayGetter tileDisplayGetter;

    private static readonly object fileAccessLock = new object();
    private Dictionary<Vector2Int, Chunk> chunkCache = new Dictionary<Vector2Int, Chunk>();

    protected abstract string GetChunkPath();
    protected abstract bool UseSurfaceGenerator();
    public void Awake()
    {
        worldSeed = 5;
        //TODO get random world seed and use persistent world seed for one world
        int tilePoolSize = (RenderDistance * 2) * (RenderDistance * 2);
        persistentDataPath = new NativeArray<char>(GetChunkPath().ToCharArray(), Allocator.Persistent);
        chunksToLoad = new NativeQueue<ChunkPos>(Allocator.Persistent);
        tilesToLoad = new NativeQueue<Tile>(Allocator.Persistent);
        loadedTiles = new List<ProcessedTileData>();
        tilePool = new GameObject[tilePoolSize];
        for(int i = 0;i < tilePoolSize; i++)
        {
            tilePool[i] = Instantiate(tilePrefab, tileParent);
        }
        activeChunks = new List<ChunkPos>();
        tileDisplayGetter = new TileDisplayGetter();
    }
    public void OnDestroy()
    {
        ChunkGrabber.Complete();
        tilesToLoad.Dispose();
        persistentDataPath.Dispose();
        chunksToLoad.Dispose();
    }
    public void ShowWorld()
    {
        tileParent.gameObject.SetActive(true);
        RefreshTilePool();
    }
    public void HideWorld()
    {
        tileParent.gameObject.SetActive(false);
    }
    public void QueueManage()
    {
        //This is a mess that ensures things happen in the correct order
        if (!ChunkGrabber.IsCompleted)
        {
            return;
        }
        ChunkGrabber.Complete();
        ManageChunkQueue();
        ManageTileRenderQueue();
        UpdateRequiredChunks();
    }
    //Update the list of chunks to keep only required chunks loaded
    protected void UpdateRequiredChunks()
    {
        //add all chunks that are needed but not active
        ChunkPos[] neededChunks = GetNeededChunks();
        foreach(ChunkPos neededChunk in neededChunks)
        {
            if (activeChunks.Contains<ChunkPos>(neededChunk))
            {
                continue;
            }
            LoadChunk(neededChunk);

        }
        //remove all chunks that are active but not needed
        Queue<ChunkPos> chunksToRemove = new Queue<ChunkPos>();
        foreach(ChunkPos activeChunk in activeChunks)
        {
            if (neededChunks.Contains<ChunkPos>(activeChunk))
            {
                continue;
            }
            chunksToRemove.Enqueue(activeChunk);
        }
        while (chunksToRemove.Count > 0)
        {
            UnloadChunk(chunksToRemove.Dequeue());
        }
    }
    //add a chunk to active loaded list, and queue its tiles to be loaded
    private void LoadChunk(ChunkPos chunkPosition)
    {
        FileHelper.DirectoryCheck();
        activeChunks.Add(chunkPosition);
        chunksToLoad.Enqueue(chunkPosition);
    }
    //remove a chunk from active loaded list, and remove its associated tiles
    private void UnloadChunk(ChunkPos chunkToRemove)
    {
        activeChunks.Remove(chunkToRemove);
        RemoveTilesFromChunk(chunkToRemove);
    }
    //remove the tiles associated with a single chunk
    protected void RemoveTilesFromChunk(ChunkPos chunkToRemoveTiles)
    {
        int removalChunkX = chunkToRemoveTiles.X;
        int removalChunkY = chunkToRemoveTiles.Y;
        Queue<ProcessedTileData> tilesToRemove = new Queue<ProcessedTileData>();
        foreach (ProcessedTileData tile in loadedTiles)
        {
            if(tile.ChunkX == removalChunkX && tile.ChunkY == removalChunkY)
            {
                tilesToRemove.Enqueue(tile);
            }
        }
        while (tilesToRemove.Count > 0)
        {
            loadedTiles.Remove(tilesToRemove.Dequeue());
        }
    }
    private ChunkPos[] GetNeededChunks()
    {
        //eventually will calculate required amount of chunks based on render distance
        ChunkPos[] neededChunks = new ChunkPos[9];
        int playerChunkX = (int)MathF.Round(playerTransform.position.x / Chunk.ChunkSize());
        int playerChunkY = (int)MathF.Round(playerTransform.position.z / Chunk.ChunkSize());
        neededChunks[0] = new ChunkPos(playerChunkX - 1, playerChunkY - 1);
        neededChunks[1] = new ChunkPos(playerChunkX - 1, playerChunkY);
        neededChunks[2] = new ChunkPos(playerChunkX - 1, playerChunkY + 1);
        neededChunks[3] = new ChunkPos(playerChunkX, playerChunkY - 1);
        neededChunks[4] = new ChunkPos(playerChunkX, playerChunkY);
        neededChunks[5] = new ChunkPos(playerChunkX, playerChunkY + 1);
        neededChunks[6] = new ChunkPos(playerChunkX + 1, playerChunkY - 1);
        neededChunks[7] = new ChunkPos(playerChunkX + 1, playerChunkY);
        neededChunks[8] = new ChunkPos(playerChunkX + 1, playerChunkY + 1);
        return neededChunks;
    }
    private void ManageChunkQueue()
    {
        if(chunksToLoad.Count > 0)
        {
            //Ensure path exists
            FileHelper.DirectoryCheckChunk(GetChunkPath());
            persistentDataPath.Dispose();
            persistentDataPath = new NativeArray<char>(GetChunkPath().ToCharArray(), Allocator.Persistent);
            ChunkPos newChunkPos = chunksToLoad.Dequeue();
            ChunkGrabberJob newChunkJob = new ChunkGrabberJob()
            {
                tileQueue = tilesToLoad,
                X = newChunkPos.X,
                Y = newChunkPos.Y,
                persistentDataPath = persistentDataPath,
                seed = worldSeed,
                useSurfaceGenerator = UseSurfaceGenerator()

            };
            ChunkGrabber = newChunkJob.Schedule();
        }
    }
    private void ManageTileRenderQueue()
    {
        ChunkGrabber.Complete();
        while (tilesToLoad.Count > 0)
        {
            loadedTiles.Add(new ProcessedTileData(tilesToLoad.Dequeue()));
        }
        UpdateTilePool();
    }
    private void UpdateTilePool()
    {
        int tilePoolIndex = 0;
        int loadedTileIndex = 0;
        while(tilePoolIndex < tilePool.Length)
        {
            if(loadedTileIndex >= loadedTiles.Count)
            {
                tilePool[tilePoolIndex].SetActive(false);
                tilePoolIndex++;
                continue;
            }
            if (!loadedTiles[loadedTileIndex].IsInPlayerRange(playerTransform.position.x, playerTransform.position.z, RenderDistance))
            {
                loadedTileIndex++;
                continue;
            }
            loadedTiles[loadedTileIndex].ApplyTileProperties(tilePool[tilePoolIndex], tileDisplayGetter);
            loadedTileIndex++;
            tilePoolIndex++;
        }
    }
    //save a chunk
    protected void SaveChunk(Chunk chunk)
    {
        lock (fileAccessLock)
        {
            try
            {
                string path = GetChunkPath();
                Directory.CreateDirectory(path); // Ensure directory exists

                string fileName = Path.Combine(path, $"chunk_{chunk.X},{chunk.Y}.json");
                string tempFileName = fileName + ".tmp";

                // Write to temp file first
                using (FileStream fs = new FileStream(tempFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(JsonUtility.ToJson(chunk.GetChunkForSerialization(), true));
                }

                // Atomic replace
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                File.Move(tempFileName, fileName);

                // Update cache
                var chunkKey = new Vector2Int(chunk.X, chunk.Y);
                if (chunkCache.ContainsKey(chunkKey))
                {
                    chunkCache[chunkKey] = chunk;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save chunk: {ex.Message}");
            }
            finally
            {
                chunk.Dispose();
            }
        }
    }
    //clear the chunks directory
    protected void DeleteAllChunks()
    {
        FileHelper.DirectoryCheck();
        string path = new string(GetChunkPath().ToArray());
        // Delete all files
        string[] files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            File.Delete(file);
        }
    }
    public List<TravelTile> GrabPathContext()
    {
        //returns all loaded tiles as a list of tile travel costs
        List<TravelTile> surroundingTileCosts = new List<TravelTile>();
        foreach(ProcessedTileData tile in loadedTiles)
        {
            surroundingTileCosts.Add(new TravelTile(tile));
        }
        return surroundingTileCosts;
    }
    public void RefreshTilePool()
    {
        foreach (GameObject tile in tilePool)
        {
            if (!tile.activeSelf)
            {
                continue;
            }
            TileDataHolder tileData = tile.GetComponent<TileDataHolder>();
            if (tileData == null)
            {
                continue;
            }
            if(tileData.initialized == false)
            {
                continue;
            }
            tileData.thisTileData.ApplyTileProperties(tile, tileDisplayGetter);
        }
    }
    public void SetTile(int x, int y, Layers layer, ITileSegment segment)
    {
        // Find the tile in loadedTiles
        for (int i = 0; i < loadedTiles.Count; i++)
        {
            if (loadedTiles[i].X == x && loadedTiles[i].Y == y)
            {
                ProcessedTileData updatedTile = loadedTiles[i];

                // Modify the corresponding layer
                switch (layer)
                {
                    case Layers.Base:
                        updatedTile.BaseType = (ITileSegmentBaseLayer)segment;
                        break;
                    case Layers.Foliage:
                        updatedTile.FoliageType = (ITileSegmentFoliageLayer)segment;
                        break;
                    case Layers.Object:
                        updatedTile.ObjectType = (ITileSegmentObjectLayer)segment;
                        break;
                }

                // Recalculate travel cost
                updatedTile.TravelCost = updatedTile.BaseType.GetTravelCost() +
                                         updatedTile.FoliageType.GetTravelCost() +
                                         updatedTile.ObjectType.GetTravelCost();

                // Replace the tile in the list
                loadedTiles[i] = updatedTile;

                // Find and update the corresponding GameObject in tilePool
                foreach (GameObject tile in tilePool)
                {
                    TileDataHolder tileData = tile.GetComponent<TileDataHolder>();
                    if (tileData != null && tileData.initialized && tileData.thisTileData.X == x && tileData.thisTileData.Y == y)
                    {
                        updatedTile.ApplyTileProperties(tile, tileDisplayGetter);
                        //Save the chunk with the changes
                        UpdateChunkFile(x, y, layer, segment.GetContentsEnum(), updatedTile.ChunkX, updatedTile.ChunkY);
                        return;
                    }
                }
                return;
            }
        }
    }
    private void UpdateChunkFile(int x, int y, Layers layer, LayerContents contents, int chunkX = 0, int chunkY = 0)
    {
        if (UseSurfaceGenerator())
        {
            lock (fileAccessLock)
            {
                try
                {
                    string path = GetChunkPath();
                    string fullPath = Path.Combine(path, $"chunk{chunkX},{chunkY}.json");

                    // Read file with shared read access
                    SerializableChunk chunk;
                    using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        chunk = JsonUtility.FromJson<SerializableChunk>(sr.ReadToEnd());
                    }

                    // Modify the chunk data
                    int tileIndex = 0;
                    foreach (Tile t in chunk.tiles)
                    {
                        if (t.X == x && t.Y == y)
                        {
                            switch (layer)
                            {
                                case Layers.Base:
                                    chunk.tiles[tileIndex].Layers = new TileSegmentDataHolder(contents,
                                        chunk.tiles[tileIndex].Layers.foliageType,
                                        chunk.tiles[tileIndex].Layers.objectType);
                                    break;
                                case Layers.Foliage:
                                    chunk.tiles[tileIndex].Layers = new TileSegmentDataHolder(
                                        chunk.tiles[tileIndex].Layers.baseType,
                                        contents,
                                        chunk.tiles[tileIndex].Layers.objectType);
                                    break;
                                case Layers.Object:
                                    chunk.tiles[tileIndex].Layers = new TileSegmentDataHolder(
                                        chunk.tiles[tileIndex].Layers.baseType,
                                        chunk.tiles[tileIndex].Layers.foliageType,
                                        contents);
                                    break;
                            }
                            break;
                        }
                        tileIndex++;
                    }

                    // Write with exclusive access
                    using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(JsonUtility.ToJson(chunk, true));
                    }

                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to update chunk file: {ex.Message}");
                }
            }
        }
        else
        {
            lock (fileAccessLock)
            {
                try
                {
                    string path = GetChunkPath();
                    string fullPath = Path.Combine(path, $"chunk0,0.json"); // Dungeon uses single chunk file

                    // Read file with shared read access
                    SerializableChunk chunk;
                    using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        chunk = JsonUtility.FromJson<SerializableChunk>(sr.ReadToEnd());
                    }

                    // Modify the chunk data
                    int tileIndex = 0;
                    foreach (Tile t in chunk.tiles)
                    {
                        if (t.X == x && t.Y == y)
                        {
                            switch (layer)
                            {
                                case Layers.Base:
                                    chunk.tiles[tileIndex].Layers = new TileSegmentDataHolder(contents,
                                        chunk.tiles[tileIndex].Layers.foliageType,
                                        chunk.tiles[tileIndex].Layers.objectType);
                                    break;
                                case Layers.Foliage:
                                    chunk.tiles[tileIndex].Layers = new TileSegmentDataHolder(
                                        chunk.tiles[tileIndex].Layers.baseType,
                                        contents,
                                        chunk.tiles[tileIndex].Layers.objectType);
                                    break;
                                case Layers.Object:
                                    chunk.tiles[tileIndex].Layers = new TileSegmentDataHolder(
                                        chunk.tiles[tileIndex].Layers.baseType,
                                        chunk.tiles[tileIndex].Layers.foliageType,
                                        contents);
                                    break;
                            }
                            break;
                        }
                        tileIndex++;
                    }

                    // Write with exclusive access
                    using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(JsonUtility.ToJson(chunk, true));
                    }

                    // Update cache
                    var chunkKey = new Vector2Int(0, 0);
                    if (chunkCache.ContainsKey(chunkKey))
                    {
                        chunkCache[chunkKey] = chunk.ToChunk();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to update chunk file: {ex.Message}");
                }
            }
        }
    }
    protected void ClearActiveChunks()
    {
        activeChunks = new List<ChunkPos>();
    }

}
