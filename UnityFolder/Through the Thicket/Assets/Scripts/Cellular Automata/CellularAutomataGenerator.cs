using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CellularAutomataGenerator
{
    public static void GenerateChunkAt(int chunkX, int chunkY, string filePath, int seed)
    {
        //we need a grid of 9 chunks so we can simulate all surrounding tile generation
        AutomataChunk[,] chunks = new AutomataChunk[3, 3];
        GenerateChunks(chunks);
    }
    private static void GenerateChunks(AutomataChunk[,] chunks)
    {

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
    }
    private class AutomataChunk
    {
        private AutomataTile[,] tiles;
        private AutomataChunk()
        {
            tiles = new AutomataTile[Chunk.ChunkSize(), Chunk.ChunkSize()];
        }
    }
}
