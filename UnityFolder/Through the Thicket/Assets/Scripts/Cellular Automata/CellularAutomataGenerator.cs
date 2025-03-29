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
        int iterations = 8;
        for(int i = 0; i < iterations; i++)
        {
            tiles = CellularIteration(tiles, new AdvancedIslandRule());
        }
        iterations = 1;
        for (int i = 0; i < iterations; i++)
        {
            tiles = CellularIteration(tiles, new IslandRefinerRule());
        }
        iterations = 5;
        for (int i = 0; i < iterations; i++)
        {
            tiles = CellularIteration(tiles, new GrassPopulaterRule(seed));
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
    private static AutomataTile[,] CellularIteration(AutomataTile[,] oldTiles, AutomataRule rule)
    {
        AutomataTile[,] newTiles = new AutomataTile[48, 48];
        for (int x = 0; x < 48; x++)
        {
            for(int y = 0; y < 48; y++)
            {
                newTiles[x, y] = rule.ApplyRule(GetNeighbourhood(oldTiles,x, y, rule.GetRange()));
            }
        }
        return newTiles;
    }
    private static AutomataTile[,] GetNeighbourhood(AutomataTile[,] tiles, int x, int y, int range)
    {
        int gridSize = (range * 2) + 1;
        AutomataTile[,] neighbourhood = new AutomataTile[gridSize, gridSize];
        for(int xOff = -range;xOff <= range; xOff++)
        {
            for (int yOff = -range; yOff <= range; yOff++)
            {
                try
                {
                    neighbourhood[xOff + range, yOff + range] = tiles[x + xOff, y + yOff];
                }
                catch (Exception)
                {
                    neighbourhood[xOff + range, yOff + range] = new AutomataTile(AutomataTileType.None);
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
    enum AutomataFoliageType
    {
        None,
        TallGrass,
    }
    enum AutomataObjectType
    {
        None,
    }
    private class AutomataTile
    {
        public AutomataTileType type;
        public AutomataFoliageType foliageType;
        public AutomataObjectType objectType;
        public AutomataTile(AutomataTileType startType)
        {
            type = startType;
            foliageType = AutomataFoliageType.None;
            objectType = AutomataObjectType.None;
        }
        public AutomataTile(AutomataTileType startType, AutomataFoliageType foliage)
        {
            type = startType;
            foliageType = foliage;
            objectType = AutomataObjectType.None;
        }
        public AutomataTile(AutomataTileType startType, AutomataFoliageType foliage, AutomataObjectType objType)
        {
            type = startType;
            foliageType = foliage;
            objectType = objType;
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
                    chunk.tiles[x + (y * 16)] = 
                        new Tile(x + (chunkX * 16), y + (chunkY * 16), 0, chunkX, chunkY, 
                        tiles[x, y].type == AutomataTileType.Mud ? new Stone() : 
                        tiles[x,y].type == AutomataTileType.River ? new River() : 
                        new Grass(),
                        tiles[x,y].foliageType == AutomataFoliageType.TallGrass ? new EmptyFoliage() :
                        new EmptyFoliage(),
                        //This is currently comparing foliage type just for testing, in future this will query the object type to get the object
                        tiles[x,y].foliageType == AutomataFoliageType.TallGrass ? new CaveEntrance() : // TODO replace with objects when added
                        new EmptyObject());
                }
            }
            return chunk;
        }
    }
    private abstract class AutomataRule
    {
        public abstract AutomataTile ApplyRule(AutomataTile[,] tiles);
        //defines range of neighbourhood to request for each rule application
        public virtual int GetRange() { return 1; }
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
    private class IslandRule : AutomataRule
    {
        public override AutomataTile ApplyRule(AutomataTile[,] tiles)
        {
            int mudNeighbourCount = 0;
            int riverNeighbourCount = 0;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (x == 1 && y == 1)
                    {
                        continue;
                    }
                    if (tiles[x, y].type == AutomataTileType.Mud)
                    {
                        mudNeighbourCount++;
                    }
                    if (tiles[x,y].type == AutomataTileType.River)
                    {
                        riverNeighbourCount++;
                    }
                }
            }
            if (tiles[1,1].type == AutomataTileType.River)
            {
                return new AutomataTile(mudNeighbourCount >= 5 ? AutomataTileType.Mud : AutomataTileType.River);
            }
            else
            {
                return new AutomataTile(mudNeighbourCount >= 4 ? AutomataTileType.Mud : AutomataTileType.River);
            }
        }
    }
    private class AdvancedIslandRule : AutomataRule
    {
        public override AutomataTile ApplyRule(AutomataTile[,] tiles)
        {
            int landNeighbourCount = 0;
            int riverNeighbourCount = 0;
            int mudNeighbourCount = 0;

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (x == 1 && y == 1) continue;

                    if (tiles[x, y].type == AutomataTileType.Grass)
                    {
                        landNeighbourCount++;
                    }
                    if (tiles[x, y].type == AutomataTileType.River)
                    {
                        riverNeighbourCount++;
                        landNeighbourCount++;
                    }
                    if (tiles[x, y].type == AutomataTileType.Mud)
                    {
                        mudNeighbourCount++;
                    }
                }
            }

            if (tiles[1, 1].type == AutomataTileType.Grass)
            {
                return new AutomataTile(landNeighbourCount == 8 ? AutomataTileType.Grass : AutomataTileType.Mud);
            }
            if (tiles[1, 1].type == AutomataTileType.Mud)
            {
                if (mudNeighbourCount == 8)
                {
                    return new AutomataTile(AutomataTileType.Grass);
                }
                return new AutomataTile(riverNeighbourCount >= 5 ? AutomataTileType.River : AutomataTileType.Mud);
            }
            else
            {
                return new AutomataTile(riverNeighbourCount >= 4 ? AutomataTileType.River : AutomataTileType.Mud);
            }
        }
    }

    private class IslandRefinerRule : AutomataRule
    {
        public override AutomataTile ApplyRule(AutomataTile[,] tiles)
        {
            int landNeighbourCount = 0;
            int riverNeighbourCount = 0;
            int mudNeighbourCount = 0;

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (x == 1 && y == 1) continue;

                    if (tiles[x, y].type == AutomataTileType.Grass)
                    {
                        landNeighbourCount++;
                    }
                    if (tiles[x, y].type == AutomataTileType.River)
                    {
                        riverNeighbourCount++;
                        landNeighbourCount++;
                    }
                    if (tiles[x, y].type == AutomataTileType.Mud)
                    {
                        mudNeighbourCount++;
                    }
                }
            }

            if (tiles[1, 1].type == AutomataTileType.Grass)
            {
                return new AutomataTile(mudNeighbourCount <= 4 ? AutomataTileType.Grass : AutomataTileType. Mud);
            }
            if (tiles[1, 1].type == AutomataTileType.Mud)
            {
                return new AutomataTile(riverNeighbourCount >= 5 ? AutomataTileType.River : AutomataTileType.Mud);
            }
            else
            {
                return new AutomataTile(AutomataTileType.River);
            }
        }
    }
    private class GrassPopulaterRule : AutomataRule
    {
        private Unity.Mathematics.Random rand;

        public GrassPopulaterRule(int seed)
        {
            rand = new Unity.Mathematics.Random((uint)seed);
        }

        public override AutomataTile ApplyRule(AutomataTile[,] tiles)
        {
            int tallGrassNeighbourCount = 0;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (x == 1 && y == 1)
                    {
                        continue;
                    }
                    if (tiles[x, y].foliageType == AutomataFoliageType.TallGrass)
                    {
                        tallGrassNeighbourCount++;
                    }
                }
            }
            if (tiles[1,1].type != AutomataTileType.Grass)
            {
                return new AutomataTile(tiles[1,1].type, tiles[1,1].foliageType, tiles[1,1].objectType);
            }
            if(tallGrassNeighbourCount < 2)
            {
                if(rand.NextInt(1,3) == 1)
                {
                    return new AutomataTile(AutomataTileType.Grass, AutomataFoliageType.None, tiles[1, 1].objectType);
                }
                return new AutomataTile(AutomataTileType.Grass, AutomataFoliageType.TallGrass, tiles[1,1].objectType);
            }
            return new AutomataTile(AutomataTileType.Grass, AutomataFoliageType.None, tiles[1, 1].objectType);
        }
    }
}
