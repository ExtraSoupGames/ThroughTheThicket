using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using System;
using System.IO;
using System.Linq;
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
public class ChunkManager : MonoBehaviour
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
    private Queue<ChunkPos> chunksToLoad;
    //queue for tiles that need to be loaded
    private NativeQueue<Tile> tilesToLoad;
    //queue of tiles to process into render ready structs
    private NativeQueue<Tile> tilesToProcess;
    //queue for tiles that have been loaded and need to be rendered
    private NativeQueue<ProcessedTileData> tilesToRender;
    //list of every currently loaded tile being rendered
    private List<ProcessedTileData> loadedTiles;
    //pool of tile objects to render the world with
    private GameObject[] tilePool;
    //all chunks being currently loaded
    List<ChunkPos> activeChunks;
    //JobHandles to track other threads
    JobHandle ChunkGrabber;
    JobHandle TileProcessor;
    private TileDisplayGetter tileDisplayGetter;
    public void Start()
    {
        int tilePoolSize = (RenderDistance * 2) * (RenderDistance * 2);
        persistentDataPath = new NativeArray<char>(Application.persistentDataPath.ToCharArray(), Allocator.Persistent);
        chunksToLoad = new Queue<ChunkPos>();
        tilesToLoad = new NativeQueue<Tile>(Allocator.Persistent);
        tilesToRender = new NativeQueue<ProcessedTileData>(Allocator.Persistent);
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
        TileProcessor.Complete();
        ChunkGrabber.Complete();
        tilesToLoad.Dispose();
        tilesToRender.Dispose();
        persistentDataPath.Dispose();
    }
    public void Tests()
    {
        DeleteAllChunks();
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                Chunk chunk = new Chunk(i, j);
                SaveChunk(chunk);
            }
        }
    }
    public void QueueManage()
    {
        //This is a mess that ensures things happen in the correct order
        if (TileProcessor.IsCompleted)
        {
            TileProcessor.Complete();
            if (tilesToProcess.IsCreated)
            {
                tilesToProcess.Dispose();
            }
            ManageTileRenderQueue();
        }
        if (TileProcessor.IsCompleted && ChunkGrabber.IsCompleted)
        {
            ChunkGrabber.Complete();
            TileProcessor.Complete();
            ManageTileQueue();
        }
        if (TileProcessor.IsCompleted && ChunkGrabber.IsCompleted)
        {
            ChunkGrabber.Complete();
            TileProcessor.Complete();
            ManageChunkQueue();
        }
        if (TileProcessor.IsCompleted && ChunkGrabber.IsCompleted)
        {
            ChunkGrabber.Complete();
            TileProcessor.Complete();
            UpdateRequiredChunks();
        }

    }
    //Update the list of chunks to keep only required chunks loaded
    private void UpdateRequiredChunks()
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
    private void RemoveTilesFromChunk(ChunkPos chunkToRemoveTiles)
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
            ChunkPos newChunkPos = chunksToLoad.Dequeue();
            ChunkGrabberJob newChunkJob = new ChunkGrabberJob()
            {
                tileQueue = tilesToLoad,
                X = newChunkPos.X,
                Y = newChunkPos.Y,
                persistentDataPath = persistentDataPath
            };
            ChunkGrabber = newChunkJob.Schedule();
        }
    }
    private void ManageTileQueue()
    {
        TileProcessor.Complete();
        ChunkGrabber.Complete();
        //this is the number of tiles each tilesProccessorJob takes on, higher = less overhead, lower = less chance that some tiles will be left unloaded
        int tilesAtATime = 32;
        if(tilesToLoad.Count >= tilesAtATime) // this causes problems, Count()
        {
            tilesToProcess = new NativeQueue<Tile>(Allocator.Persistent);
            for(int i = 0;i< tilesAtATime; i++)
            {
                tilesToProcess.Enqueue(tilesToLoad.Dequeue());
            }
            TileProcessorJob tileProcessorJob = new TileProcessorJob()
            {
                tilesToProcess = tilesToProcess,
                tileRenderQueue = tilesToRender
            };
            TileProcessor = tileProcessorJob.Schedule();
        }
    }
    private void ManageTileRenderQueue()
    {
        while (tilesToRender.Count >= 1)
        {
            loadedTiles.Add(tilesToRender.Dequeue());
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
    private void SaveChunk(Chunk chunk)
    {
        FileHelper.DirectoryCheck();
        string chunkAsJSON = JsonUtility.ToJson(chunk.GetChunkForSerialization());
        string fileName = "/chunks/chunk" + chunk.X + "-" + chunk.Y;
        File.WriteAllText(Application.persistentDataPath + fileName + ".json", chunkAsJSON);
        chunk.Dispose();
    }
    //clear the chunks directory
    private void DeleteAllChunks()
    {
        string path = Application.persistentDataPath + "/chunks";
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
}
