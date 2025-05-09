using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using TreeEditor;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.XR;

public static class CellularAutomataGenerator
{
    public static void GenerateChunkAt(int chunkX, int chunkY, string filePath, int seed)
    {
        //Generate a chunk at the given coordinates and save it
        AutomataChunk generatedChunk = GenerateChunk(chunkX, chunkY, seed);
        SerializableChunk chunkToSave = generatedChunk.GetChunkForSaving();
        string chunkAsJSON = JsonUtility.ToJson(chunkToSave, true);
        string fileName = Path.Combine(filePath, "chunk" + chunkToSave.X + "," + chunkToSave.Y);
        File.WriteAllText(fileName + ".json", chunkAsJSON);
    }
    private static AutomataChunk GenerateChunk(int chunkX, int chunkY, int seed)
    {
        //Create the initial noise map
        AutomataTile[,] tiles = InitialEmptyGrid(chunkX, chunkY, seed);

        
        //Go through each rule a specified number of times, applying the rule each time
        int iterations = 2;
        for(int i = 0; i < iterations; i++)
        {
            tiles = CellularIteration(tiles, new RiverExpander());
        }
        iterations = 2;
        for (int i = 0; i < iterations; i++)
        {
            tiles = CellularIteration(tiles, new AdvancedIslandRule());
        }
        iterations = 7;
        for (int i = 0; i < iterations; i++)
        {
            tiles = CellularIteration(tiles, new TreePopulaterRule(seed));
        }
        iterations = 11;
        for (int i = 0; i < iterations; i++)
        {
            tiles = CellularIteration(tiles, new GrassPopulaterRule(seed));
        }
        iterations = 3;
        for(int i = 0; i < iterations; i++)
        {
            tiles = CellularIteration(tiles, new FoliagePopulaterRule());
        }
        iterations = 3;
        for (int i = 0; i < iterations; i++)
        {
            tiles = CellularIteration(tiles, new MushroomPopulaterRule());
        }
        iterations = 3;
        for (int i = 0; i < iterations; i++)
        {
            tiles = CellularIteration(tiles, new PebblePopulaterRule());
        }
        tiles = CellularIteration(tiles, new CaveEntranceGenerator(seed));
        AutomataChunk returnChunk = new AutomataChunk(tiles, chunkX, chunkY);
        return returnChunk;
    }
    private static AutomataTile[,] InitialEmptyGrid(int chunkOffsetX, int chunkOffsetY, int seed)
    {
        //we need a grid of 9 chunks so we can simulate all surrounding tile generation
        AutomataTile[,] tiles = new AutomataTile[48, 48];
        for(int x = 0; x < 3; x++)
        {
            for(int y = 0; y < 3; y++)
            {
                for(int inChunkX = 0; inChunkX <16; inChunkX++)
                {
                    for (int inChunkY = 0; inChunkY < 16; inChunkY++)
                    {
                        int tileX = inChunkX + (x + chunkOffsetX) * 16;
                        int tileY = inChunkY + (y + chunkOffsetY) * 16;
                        float2 noiseOffset = new float2(Mathf.Pow(seed, 5), Mathf.Pow(seed, 5));
                        float noiseValue = noise.snoise(new float2(tileX * 0.02f + noiseOffset.x, tileY * 0.02f + noiseOffset.y));
                        bool isRiver = noiseValue > 0.45f && noiseValue < 0.55f;
                        tiles[x * 16 + inChunkX, y * 16 + inChunkY] = new AutomataTile(isRiver ? AutomataTileType.River : AutomataTileType.Mud);
                    }
                }
            }
        }
        return tiles;
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
        TreeStump,
        Mushroom,
        Foliage,
        Pebble,
        Entrance
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
                        tiles[x, y].type == AutomataTileType.River ? new River() :
                        new Grass(),

                        tiles[x, y].foliageType == AutomataFoliageType.TallGrass ? new TallGrass() :
                        tiles[x, y].foliageType == AutomataFoliageType.TreeStump ? new TreeStump() :
                        tiles[x, y].foliageType == AutomataFoliageType.Mushroom ? new Redcap() :
                        tiles[x,y].foliageType == AutomataFoliageType.Foliage ? new Twigs() :
                        new EmptyFoliage(),

                        tiles[x, y].foliageType == AutomataFoliageType.Pebble ? new Pebble() :
                        tiles[x,y].foliageType == AutomataFoliageType.Entrance ? new DungeonEntrance() :
                        new EmptyObject());
                }
            }
            return chunk;
        }
    }
    //Abstract rule
    private abstract class AutomataRule
    {
        public abstract AutomataTile ApplyRule(AutomataTile[,] tiles);
        //defines range of neighbourhood to request for each rule application
        public virtual int GetRange() { return 1; }
    }
    //Here we define all of our rules
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

    private class TreePopulaterRule : AutomataRule
    {
        private Unity.Mathematics.Random rand;

        public TreePopulaterRule(int seed)
        {
            rand = new Unity.Mathematics.Random((uint)seed);
        }
        public override int GetRange()
        {
            return 2;
        }

        public override AutomataTile ApplyRule(AutomataTile[,] tiles)
        {
            int tallGrassNeighbourCount = 0;
            int treeNeighbourCount = 0;
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    if (x == 2 && y == 2)
                    {
                        continue;
                    }
                    if (tiles[x, y].foliageType == AutomataFoliageType.TallGrass)
                    {
                        tallGrassNeighbourCount++;
                    }
                    if (tiles[x,y].foliageType == AutomataFoliageType.TreeStump)
                    {
                        treeNeighbourCount++;
                    }
                }
            }
            if (tiles[2, 2].type != AutomataTileType.Grass)
            {
                return new AutomataTile(tiles[2, 2].type, tiles[2, 2].foliageType, tiles[2, 2].objectType);
            }
            if (tiles[2,2].foliageType == AutomataFoliageType.TreeStump)
            {
                if(treeNeighbourCount > 1)
                {
                    return new AutomataTile(AutomataTileType.Grass, AutomataFoliageType.None, tiles[2, 2].objectType);
                }
                if (rand.NextInt(1, 19) != 1)
                {
                    return new AutomataTile(AutomataTileType.Grass, AutomataFoliageType.None, tiles[2, 2].objectType);
                }
                return new AutomataTile(AutomataTileType.Grass, AutomataFoliageType.TreeStump, tiles[2, 2].objectType);
            }
            if(rand.NextInt(1,25) == 1)
            {
                if(treeNeighbourCount == 0)
                {
                    return new AutomataTile(AutomataTileType.Grass, AutomataFoliageType.TreeStump, tiles[2, 2].objectType);
                }
            }
            return new AutomataTile(AutomataTileType.Grass, AutomataFoliageType.None, tiles[2, 2].objectType);
        }
    }
    private class GrassPopulaterRule : AutomataRule
    {
        private Unity.Mathematics.Random rand;
        public GrassPopulaterRule(int seed)
        {
            rand = new Unity.Mathematics.Random((uint)seed);
        }
        public override int GetRange()
        {
            return 2;
        }
        public override AutomataTile ApplyRule(AutomataTile[,] tiles)
        {
            int tallGrassNeighbourCount = 0;
            int treeNeighbourCount = 0;
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    if (x == 2 && y == 2)
                    {
                        continue;
                    }
                    if (tiles[x, y].foliageType == AutomataFoliageType.TallGrass)
                    {
                        tallGrassNeighbourCount++;
                    }
                    if (tiles[x, y].foliageType == AutomataFoliageType.TreeStump)
                    {
                        treeNeighbourCount++;
                    }
                }
            }
            if (tiles[2, 2].type != AutomataTileType.Grass || tiles[2,2].foliageType == AutomataFoliageType.TreeStump)
            {
                return new AutomataTile(tiles[2, 2].type, tiles[2, 2].foliageType, tiles[2, 2].objectType);
            }
            if(treeNeighbourCount > 0)
            {
                if(rand.NextInt(1, 3) == 1)
                {
                    return new AutomataTile(tiles[2, 2].type, AutomataFoliageType.TallGrass);
                }
                return new AutomataTile(tiles[2, 2].type, AutomataFoliageType.None);
            }
            if(rand.NextInt(1,6) == 1)
            {
                return new AutomataTile(tiles[2, 2].type, AutomataFoliageType.TallGrass);
            }
            return new AutomataTile(tiles[2, 2].type, AutomataFoliageType.None);
        }
    }
    private class MushroomPopulaterRule : AutomataRule
    {
        public override AutomataTile ApplyRule(AutomataTile[,] tiles)
        {
            int treeNeighbourCount = 0;
            int mushroomNeighbourCount = 0;
            int foliageNeighbourCount = 0;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (x == 1 && y == 1)
                    {
                        continue;
                    }
                    if (tiles[x, y].foliageType == AutomataFoliageType.Mushroom)
                    {
                        mushroomNeighbourCount++;
                    }
                    if (tiles[x, y].foliageType == AutomataFoliageType.TreeStump)
                    {
                        treeNeighbourCount++;
                    }
                    if (tiles[x, y].foliageType == AutomataFoliageType.Foliage)
                    {
                        foliageNeighbourCount++;
                    }
                }
            }
            if (tiles[1,1].type != AutomataTileType.Grass || !(tiles[1,1].foliageType == AutomataFoliageType.None || tiles[1,1].foliageType == AutomataFoliageType.Foliage))
            {
                return new AutomataTile(tiles[1,1].type, tiles[1,1].foliageType, tiles[1,1].objectType);
            }
            if(tiles[1, 1].foliageType == AutomataFoliageType.Foliage)
            {
                if(mushroomNeighbourCount == 0 && treeNeighbourCount == 1 && foliageNeighbourCount == 2)
                {
                    return new AutomataTile(tiles[1, 1].type, AutomataFoliageType.Mushroom);
                }
                return new AutomataTile(tiles[1, 1].type, tiles[1, 1].foliageType, tiles[1, 1].objectType);

            }
            if (treeNeighbourCount > 0)
            {
                if(mushroomNeighbourCount == 0)
                {
                    return new AutomataTile(tiles[1,1].type, AutomataFoliageType.Mushroom);
                }
                return new AutomataTile(tiles[1,1].type, tiles[1, 1].foliageType);
            }
            if(mushroomNeighbourCount > 0 && mushroomNeighbourCount < 1)
            {
                return new AutomataTile(tiles[1, 1].type, AutomataFoliageType.Mushroom);
            }
            return new AutomataTile(tiles[1, 1].type, tiles[1, 1].foliageType);
        }
    }
    private class FoliagePopulaterRule : AutomataRule
    {
        public override AutomataTile ApplyRule(AutomataTile[,] tiles)
        {
            int treeNeighbourCount = 0;
            int foliageNeighbourCount = 0;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (x == 1 && y == 1)
                    {
                        continue;
                    }
                    if (tiles[x, y].foliageType == AutomataFoliageType.Foliage)
                    {
                        foliageNeighbourCount++;
                    }
                    if (tiles[x, y].foliageType == AutomataFoliageType.TreeStump)
                    {
                        treeNeighbourCount++;
                    }
                }
            }
            if (tiles[1, 1].type != AutomataTileType.Grass || tiles[1, 1].foliageType != AutomataFoliageType.None)
            {
                return new AutomataTile(tiles[1, 1].type, tiles[1, 1].foliageType, tiles[1, 1].objectType);
            }
            if (treeNeighbourCount > 0)
            {
                if (foliageNeighbourCount == 0)
                {
                    return new AutomataTile(tiles[1, 1].type, AutomataFoliageType.Foliage);
                }
                return new AutomataTile(tiles[1, 1].type, AutomataFoliageType.None);
            }
            if (foliageNeighbourCount > 0 && foliageNeighbourCount < 1)
            {
                return new AutomataTile(tiles[1, 1].type, AutomataFoliageType.Foliage);
            }
            return new AutomataTile(tiles[1, 1].type, AutomataFoliageType.None);
        }
    }
    private class PebblePopulaterRule : AutomataRule
    {
        public override AutomataTile ApplyRule(AutomataTile[,] tiles)
        {
            int riverNeighbourCount = 0;
            int pebbleNeighbourCount = 0;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (x == 1 && y == 1)
                    {
                        continue;
                    }
                    if (tiles[x, y].foliageType == AutomataFoliageType.Pebble)
                    {
                        pebbleNeighbourCount++;
                    }
                    if (tiles[x, y].type == AutomataTileType.River)
                    {
                        riverNeighbourCount++;
                    }
                }
            }
            if (tiles[1,1].type != AutomataTileType.Mud)
            {
                return new AutomataTile(tiles[1, 1].type, tiles[1, 1].foliageType, tiles[1, 1].objectType);
            }
            if(riverNeighbourCount == 4 && pebbleNeighbourCount == 0)
            {
                return new AutomataTile(tiles[1, 1].type, AutomataFoliageType.Pebble);
            }
            if(pebbleNeighbourCount > 0 && pebbleNeighbourCount < 3)
            {
                return new AutomataTile(tiles[1, 1].type, AutomataFoliageType.Pebble);
            }
            return new AutomataTile(tiles[1, 1].type, AutomataFoliageType.None);
        }
    }
    private class RiverExpander : AutomataRule
    {
        public override AutomataTile ApplyRule(AutomataTile[,] tiles)
        {
            int riverNeighbourCount = 0;

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (x == 1 && y == 1) continue;
                    if (tiles[x, y].type == AutomataTileType.River)
                    {
                        riverNeighbourCount++;
                    }
                }
            }


            if (tiles[1, 1].type == AutomataTileType.River)
            {
                return new AutomataTile(AutomataTileType.River);
            }
            else
            {
                if(riverNeighbourCount > 2)
                {
                    return new AutomataTile(AutomataTileType.River);
                }
                return new AutomataTile(AutomataTileType.Mud);
            }
        }
    }
    private class CaveEntranceGenerator : AutomataRule
    {
        private Unity.Mathematics.Random rand;
        public CaveEntranceGenerator(int seed)
        {
            rand = new Unity.Mathematics.Random((uint)seed);
        }
        public override AutomataTile ApplyRule(AutomataTile[,] tiles)
        {
            if (tiles[1, 1].type == AutomataTileType.River)
            {
                return new AutomataTile(AutomataTileType.River, tiles[1,1].foliageType, tiles[1, 1].objectType);
            }
            else
            {
                if(rand.NextInt(1,100) == 1)
                {
                    return new AutomataTile(tiles[1, 1].type, AutomataFoliageType.Entrance);
                }
                return new AutomataTile(tiles[1,1].type, tiles[1,1].foliageType, tiles[1,1].objectType);
            }
        }
    }
}
