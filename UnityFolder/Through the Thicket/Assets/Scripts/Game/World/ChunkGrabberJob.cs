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
    public void Execute()
    {
        //search for the chunk if it already exists
        string dataPathString = new string(persistentDataPath.ToArray());
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
            if(biomeX == X && biomeY == Y)
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
    private SerializableChunk LoadChunk(int X, int Y)
    {
        string dataPathString = new string(persistentDataPath.ToArray());
        if (!File.Exists(Path.Combine(dataPathString,"chunk" + X + "," + Y + ".json")))
        {
            Debug.Log("Invalid chunk attempted to be loaded");
            throw new Exception("Invalid chunk load attempt made");
        }
        string chunkAsJSON = File.ReadAllText(Path.Combine(dataPathString, "chunk" + X + "," + Y + ".json"));
        SerializableChunk chunk = JsonUtility.FromJson<SerializableChunk>(chunkAsJSON);
        return chunk;
    }
}
