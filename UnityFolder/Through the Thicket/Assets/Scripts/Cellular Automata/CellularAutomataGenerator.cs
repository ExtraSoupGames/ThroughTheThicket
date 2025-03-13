using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CellularAutomataGenerator
{
    public static void GenerateChunkAt(int chunkX, int chunkY, string filePath, int seed)
    {
        AutomataChunk generatedChunk = GenerateChunk(chunkX, chunkY, seed);
        //TODO save chunk as at filepath
    }
    private static AutomataChunk GenerateChunk(int chunkX, int chunkY, int seed)
    {
        AutomataChunk[,] chunks = InitialNoiseMap(chunkX, chunkY, seed);
        int iterations = 5;
        for(int i = 0; i < iterations; i++)
        {
            chunks = CellularIteration(chunks);
        }
        return chunks[1,1];
    }
    private static AutomataChunk[,] InitialNoiseMap(int chunkX, int chunkY, int seed)
    {
        //we need a grid of 9 chunks so we can simulate all surrounding tile generation
        AutomataChunk[,] noiseChunks = new AutomataChunk[3, 3];
        for(int x = 0; x < 3; x++)
        {
            for(int y = 0; y < 3; y++)
            {
                noiseChunks[x, y] = new AutomataChunk(chunkX, chunkY, seed);
            }
        }
        return noiseChunks;
    }
    private static AutomataChunk[,] CellularIteration(AutomataChunk[,] chunks)
    {
        //TODO implement cellular automata rule application
        return null;
    }
    enum AutomataTileType
    {
        None,
        Grass,
        Path,
        Mud,
        River
    }
    private class AutomataTile
    {
        public AutomataTileType type;
        public AutomataTile(AutomataTileType startType)
        {
            type = startType;
        }
    }
    private class AutomataChunk
    {
        private AutomataTile[,] tiles;
        public AutomataChunk(int chunkX, int chunkY, int seed)
        {
            //reset the seed and multiply by the chunkX and chunkY so each chunk will have a unique seed
            UnityEngine.Random.InitState(seed * chunkX * chunkY);
            for (int x  = 0; x < 16;x++)
            {
                for(int y = 0; y < 16; y++)
                {
                    tiles[x, y] = new AutomataTile(Random.Range(0, 1) == 1 ? AutomataTileType.River : AutomataTileType.Mud);
                }
            }
            tiles = new AutomataTile[Chunk.ChunkSize(), Chunk.ChunkSize()];
        }
    }
    private abstract class AutomataRule
    {
        //Moores neighbourhood is assumed
        public abstract AutomataTile[,] ApplyRule(AutomataTile[,] tiles);
    }
    public 
}
