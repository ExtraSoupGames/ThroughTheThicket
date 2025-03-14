using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public static class CellularAutomataGenerator
{
    public static void GenerateChunkAt(int chunkX, int chunkY, string filePath, int seed)
    {
        AutomataChunk generatedChunk = GenerateChunk(chunkX, chunkY, seed);
        //TODO save chunk as at filepath
        SerializableChunk chunkToSave = generatedChunk.GetChunkForSaving();
        string chunkAsJSON = JsonUtility.ToJson(chunkToSave, true);
        string fileName = Path.Combine(filePath, "chunk" + chunkToSave.X + "," + chunkToSave.Y);
        File.WriteAllText(fileName + ".json", chunkAsJSON);
    }
    private static AutomataChunk GenerateChunk(int chunkX, int chunkY, int seed)
    {
        AutomataChunk[,] chunks = InitialNoiseMap(chunkX, chunkY, seed);
        AutomataTile[,] tiles = new AutomataTile[48, 48];
        for (int cX = 0; cX < 3; cX++)
        {
            for (int cY = 0; cY < 3; cY++)
            {
                for (int tileX = 0; tileX < 16; tileX++)
                {
                    for (int tileY = 0; tileY < 16; tileY++)
                    {
                        tiles[cX * 16 + tileX, cY * 16 + tileY] = chunks[cX, cY].GetTile(tileX, tileY);
                    }
                }
            }
        }
        int iterations = 5;
        for(int i = 0; i < iterations; i++)
        {
            tiles = CellularIteration(tiles);
        }
        AutomataChunk returnChunk = new AutomataChunk(tiles, chunkX, chunkY);
        return returnChunk;
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
    private static AutomataTile[,] CellularIteration(AutomataTile[,] oldTiles)
    {
        AutomataRule rule = new TestRule();
        //TODO implement cellular automata rule application
        AutomataTile[,] newTiles = new AutomataTile[48, 48];
        for (int x = 0; x < 48; x++)
        {
            for(int y = 0; y < 48; y++)
            {
                newTiles[x, y] = rule.ApplyRule(GetNeighbourhood(oldTiles,x, y));
            }
        }
        return newTiles;
    }
    private static AutomataTile[,] GetNeighbourhood(AutomataTile[,] tiles, int x, int y)
    {
        AutomataTile[,] neighbourhood = new AutomataTile[3, 3];
        for(int xOff = -1;xOff < 2; xOff++)
        {
            for (int yOff = -1; yOff < 2; yOff++)
            {
                try
                {
                    neighbourhood[xOff + 1, yOff + 1] = tiles[x + xOff, y + yOff];
                }
                catch (Exception e)
                {
                    neighbourhood[xOff + 1, yOff + 1] = new AutomataTile(AutomataTileType.None);
                }
            }
        }
        return neighbourhood;
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
        int chunkX, chunkY;
        //This generates the chunk as a random noise map
        public AutomataChunk(int chunkX, int chunkY, int seed)
        {
            //reset the seed and multiply by the chunkX and chunkY so each chunk will have a unique seed
            uint chunkSeed = (uint)((chunkX + 500) * (chunkY + 500) * seed);
            Unity.Mathematics.Random random = new Unity.Mathematics.Random(chunkSeed);
            tiles = new AutomataTile[Chunk.ChunkSize(), Chunk.ChunkSize()];

            for (int x  = 0; x < 16;x++)
            {
                for(int y = 0; y < 16; y++)
                {
                    tiles[x, y] = new AutomataTile(random.NextInt(1, 3) == 1 ? AutomataTileType.Mud : AutomataTileType.River);
                }
            }
            this.chunkX = chunkX;
            this.chunkY = chunkY;
        }
        //this constructor generates the chunk using a 48x48 array of tiles, and extracts the tile data from the centre 16x16 of that
        public AutomataChunk(AutomataTile[,] sourceTiles, int chunkX, int chunkY)
        {
            tiles = new AutomataTile[Chunk.ChunkSize(), Chunk.ChunkSize()];

            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    tiles[x, y] = sourceTiles[x + 16,y + 16];
                }
            }
            this.chunkX = chunkX;
            this.chunkY = chunkY;
        }
        public AutomataTile GetTile(int x, int y)
        {
            return tiles[x, y];
        }
        public SerializableChunk GetChunkForSaving()
        {
            SerializableChunk chunk = new SerializableChunk(chunkX, chunkY);
            for(int x = 0; x < 16; x++)
            {
                for (int y = 0;y < 16; y++)
                {
                    chunk.tiles[x + (y * 16)] = new Tile(x + (chunkX * 16), y + (chunkY * 16), 0, chunkX, chunkY, tiles[x, y].type == AutomataTileType.Mud ? new Grass() : new Stone());
                }
            }
            return chunk;
        }
    }
    private abstract class AutomataRule
    {
        //Moores neighbourhood is assumed
        public abstract AutomataTile ApplyRule(AutomataTile[,] tiles);
    }
    private class TestRule : AutomataRule
    {
        public override AutomataTile ApplyRule(AutomataTile[,] tiles)
        {
            int riverCount = 0;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if(x == 1 && y == 1)
                    {
                        continue;
                    }
                    if (tiles[x,y].type == AutomataTileType.River)
                    {
                        riverCount++;
                    }
                }
            }
            return new AutomataTile(riverCount >= 4 ? AutomataTileType.Mud : AutomataTileType.River);
        }
    }
}
