using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using System.IO;
using System;

public struct ChunkGrabberJob : IJob
{
    public NativeArray<char> persistentDataPath;
    public int X, Y;
    public NativeQueue<Tile> tileQueue;
    public bool useSurfaceGenerator;
    public int seed;
    public NativeArray<Color> WFCInputPixels;
    public int WFCInputWidth;
    public int dungeonID;
    public void Execute()
    {
        //search for the chunk if it already exists
        string dataPathString = new string(persistentDataPath.ToArray());
        if (useSurfaceGenerator)
        {
            string[] allChunks = Directory.GetFiles(dataPathString);
            bool found = false;
            foreach (string chunk in allChunks)
            {
                //removes the path from the filename
                string chunkLocation = chunk.Substring(chunk.IndexOf("Chunks\\chunk") + 12);
                //split the coordinates into an array of strings, and remove the .json from the end
                string[] chunkCoordinates = chunkLocation.Substring(0, chunkLocation.IndexOf(".")).Split(",");
                int biomeX = int.Parse(chunkCoordinates[0]);
                int biomeY = int.Parse(chunkCoordinates[1]);
                if (biomeX == X && biomeY == Y)
                {
                    found = true;
                }
            }
            //if the chunk already exists, enqueue its tiles
            if (found)
            {
                SerializableChunk chunk = LoadChunk(X, Y);
                foreach (Tile t in chunk.tiles)
                {
                    tileQueue.Enqueue(t);
                }
                return;
            }
            //if the chunk does not already exist, generate it
            if (useSurfaceGenerator)
            {
                CellularAutomataGenerator.GenerateChunkAt(X, Y, new string(persistentDataPath.ToArray()), seed);
                SerializableChunk chunk = LoadChunk(X, Y);
                foreach (Tile t in chunk.tiles)
                {
                    tileQueue.Enqueue(t);
                }
                return;
            }
        }
        else
        {
            //for dungeons, we only have one chunk file (0,0)
            string dungeonChunkPath = Path.Combine(dataPathString, "chunk0,0.json");

            try
            {
                //use FileShare.ReadWrite to allow concurrent reads
                string chunkAsJson;
                using (FileStream fs = new FileStream(dungeonChunkPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fs))
                {
                    chunkAsJson = sr.ReadToEnd();
                }

                SerializableChunk chunk = JsonUtility.FromJson<SerializableChunk>(chunkAsJson);
                foreach (Tile t in chunk.tiles)
                {
                    tileQueue.Enqueue(t);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to load dungeon chunk: {ex.Message}");
                //generate new dungeon if loading fails

                int imageWidth = WFCInputWidth;
                int imageHeight = WFCInputPixels.Length / imageWidth;
                Color[,] inputImagePixels = new Color[imageWidth, imageHeight];
                for (int x = 0; x < imageWidth; x++)
                {
                    for (int y = 0; y < imageHeight; y++)
                    {
                        inputImagePixels[x,y] = WFCInputPixels[x + (imageWidth * y)];
                    }
                }
                WaveFunctionCollapse.GenerateDungeon(0, 10, dataPathString, seed + dungeonID, inputImagePixels);

                //retry loading
                //TODO use proper file loading for this too
                string chunkAsJson = File.ReadAllText(dungeonChunkPath);
                SerializableChunk chunk = JsonUtility.FromJson<SerializableChunk>(chunkAsJson);
                foreach (Tile t in chunk.tiles)
                {
                    tileQueue.Enqueue(t);
                }
            }
        }
    }
    private SerializableChunk LoadChunk(int X, int Y)
    {
        string dataPathString = new string(persistentDataPath.ToArray());
        if (!File.Exists(Path.Combine(dataPathString,"chunk" + X + "," + Y + ".json")))
        {
            Debug.Log("Invalid chunk attempted to be loaded");
            throw new Exception("Invalid chunk load attempt made");
        }
        string chunkAsJson;
        using (FileStream fs = new FileStream(Path.Combine(dataPathString, "chunk" + X + "," + Y + ".json"), FileMode.Open, FileAccess.Read, FileShare.Read))
        using (StreamReader sr = new StreamReader(fs))
        {
            chunkAsJson = sr.ReadToEnd();
        }
        SerializableChunk chunk = JsonUtility.FromJson<SerializableChunk>(chunkAsJson);
        return chunk;
    }
}
