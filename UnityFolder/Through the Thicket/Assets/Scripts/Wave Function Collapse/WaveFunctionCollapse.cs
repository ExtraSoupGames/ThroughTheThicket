using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

public class WaveFunctionCollapse : MonoBehaviour
{
    private static int2[] NeighbourOffsets()
    {
        return new int2[] { new int2(-1, 0), new int2(1, 0), new int2(0, 1), new int2(0,-1)};
    }
    public static void GenerateDungeon(int ID, int size, string filePath, int seed, Color[,] inputImagePixels)
    {
        //Initialize a random number engine for collapsing tiles
        Unity.Mathematics.Random randomEngine = new Unity.Mathematics.Random((uint)seed);
        // the dungeon will be a square with an odd number of tiles width and height, the size parameter represents the apothem
        int sideLength = size * 2 + 1;
        //instantiate a grid of tiles with the specified size
        WFCTile[,] tiles = new WFCTile[sideLength, sideLength];
        //instantiate a grid of floats to represent the entropy of each tile
        int[,] entropies = new int[sideLength, sideLength];
        //Populate the tiles grid with empty tiles and set all entropies to 1
        for (int i = 0; i < sideLength; i++)
        {
            for (int j = 0; j < sideLength; j++)
            {
                //Create a tile with all possibilities
                tiles[i, j] = new WFCTile(i, j);
                //Update the entropy grid accordingly
                entropies[i, j] = tiles[i, j].CalculateEntropy();
            }
        }
        //set the middle tile to an entrance tile
        tiles[size, size] = new WFCTile(WFCTileType.Entrance);
        //Update the entropy grid accordingly
        entropies[size, size] = tiles[size, size].CalculateEntropy();
        WFCRuleSet rules = new WFCRuleSet(inputImagePixels);
        while(!AllTilesCollapsed(tiles))
        {
            //Update all entropies and collapse the tile with the smallest entropy
            int2 lowestEntropyPosition = UpdateAllEntropies(tiles, entropies);
            int x = lowestEntropyPosition.x;
            int y = lowestEntropyPosition.y;
            tiles[x, y].Collapse(ref randomEngine);
            //update possible tiles for surrounding tiles repeatedly until all tiles have been updated and any resultant changes have been updated
            UpdateAllPossibilities(tiles, new int2(x, y), rules);
        }
        FileHelper.DirectoryCheckChunk(filePath);
        // write dungeon to relevant file
        SerializableChunk chunkToSave = GetFinalDungeon(tiles, sideLength);
        string chunkAsJSON = JsonUtility.ToJson(chunkToSave, true);
        string fileName = Path.Combine(filePath, "chunk" + chunkToSave.X + "," + chunkToSave.Y);
        File.WriteAllText(fileName + ".json", chunkAsJSON);
    }
    private static bool AllTilesCollapsed(WFCTile[,] tiles)
    {
        foreach(WFCTile tile in tiles)
        {
            if (!tile.IsCollapsed()) return false;
        }
        return true;
    }
    private static void UpdateAllPossibilities(WFCTile[,] tiles, int2 updateAt, WFCRuleSet rules)
    {
        if (tiles[updateAt.x, updateAt.y] == null)
        {
            return;
        }
        if (tiles[updateAt.x, updateAt.y].IsCollapsed())
        {
            return;
        }
        WFCTile[] neighbours = new WFCTile[4];//get neighbours
        int neighbourIndex = 0;
        foreach(int2 offset in NeighbourOffsets())
        {
            try
            {
                neighbours[neighbourIndex] = tiles[updateAt.x + offset.x, updateAt.y + offset.y];
            }
            catch
            {
                neighbours[neighbourIndex] = null;
            }
        }
        //update neighbour possibilities
        if (tiles[updateAt.x, updateAt.y].UpdatePossibilities(rules, neighbours))
        {
            foreach(int2 offset in NeighbourOffsets())
            {
                UpdateAllPossibilities(tiles, updateAt + offset, rules);
            }
        }
        return;
    }
    private static int2 UpdateAllEntropies(WFCTile[,] tiles, int[,] entropies)
    {
        int2 lowestEntropyPosition = int2.zero;
        int lowestEntropy = 100;
        int sideLength = tiles.GetLength(0);
        //The grids should be square and equal
        Assert.AreEqual(tiles.GetLength(0), tiles.GetLength(1));
        Assert.AreEqual(entropies.GetLength(0), entropies.GetLength(1));
        Assert.AreEqual(tiles.GetLength(0), entropies.GetLength(0));
        //iterate through and update entropies
        for(int i = 0; i < sideLength; i++)
        {
            for(int j = 0;j < sideLength; j++)
            {
                entropies[i,j] = tiles[i,j].CalculateEntropy();
                if (entropies[i, j] < lowestEntropy)
                {
                    lowestEntropy = entropies[i, j];
                    lowestEntropyPosition = new int2(i, j);
                }
            }
        }
        return lowestEntropyPosition;
    }
    private static SerializableChunk GetFinalDungeon(WFCTile[,] tiles, int sideLength)
    {
        SerializableChunk returnDungeon = new SerializableChunk(0, 0, sideLength); // all dungeons are a chunk at 0,0 containing all tile data
        for(int i = 0; i < sideLength; i++)
        {
            for(int j = 0; j < sideLength; j++)
            {
                returnDungeon.tiles[(i + (j * sideLength))] = (tiles[i,j].GetCollapsedTile());
            }
        }
        return returnDungeon;
    }
    private enum WFCTileType
    {
        None,
        Entrance
    }
    private class CollapsedTile
    {
        public WFCTileType type;
        public CollapsedTile(WFCTileType t)
        {
            type = t;
        }
    }
    private class WFCTile
    {
        public int x;
        public int y;
        private List<CollapsedTile> collapsePossibilities;
        //Instantiate a tile with all possibilities
        public WFCTile(int tileX, int tileY)
        {
            collapsePossibilities = new List<CollapsedTile>();
            foreach(WFCTileType type in Enum.GetValues(typeof(WFCTileType)))
            {
                collapsePossibilities.Add(new CollapsedTile(type));
            }
            x = tileX;
            y = tileY;
        }
        //Used for instantiating definite tiles (tiles decided by the designer not by the algorithm)
        public WFCTile(WFCTileType type)
        {
            collapsePossibilities = new List<CollapsedTile> { new CollapsedTile(type) };
        }
        public int CalculateEntropy()
        {
            if (IsCollapsed())
            {
                //Collapsed tiles should no longer be considered for collapsing
                return 100;
            }
            return collapsePossibilities.Count;
        }
        public void Collapse(ref Unity.Mathematics.Random randomEngine)
        {
            if (IsCollapsed()) return;
            int collapseResultIndex = randomEngine.NextInt(0, collapsePossibilities.Count);
            collapsePossibilities = new List<CollapsedTile> { collapsePossibilities[collapseResultIndex] };
        }
        public bool IsCollapsed()
        {
            return collapsePossibilities.Count == 1;
        }
        //Update the possible neighbours
        // rules - the ruleset with which to update the possibilities
        // neighbours, the 4 adjacent WFCTiles, in order Up, Left, Down, Right. Null for tiles outside the range
        //returns true if only one possibility remains (I.E tile was collapsed by rules of adjacent neighbours rather than by random chance)
        public bool UpdatePossibilities(WFCRuleSet rules, WFCTile[] neighbours)
        {
            collapsePossibilities = rules.GetPossibilities(neighbours, collapsePossibilities);
            if (IsCollapsed()) return true;
            return false;
        }
        public Tile GetCollapsedTile()
        {
            if (!IsCollapsed())
            {
                return new Tile(x, y, 0, 0, 0, new Stone());
            }
            if (collapsePossibilities[0].type == WFCTileType.Entrance)
            {
                return new Tile(x, y, 0, 0, 0, new Grass());
            }
            if (collapsePossibilities[0].type == WFCTileType.None)
            {
                return new Tile(x,y,0,0,0,new Grass(), new TallGrass(), new EmptyObject());
            }
            return new Tile(x, y, 0, 0, 0, new Stone());
        }
        public List<WFCTileType> GetPossibilities()
        {
            List<WFCTileType> tiles = new List<WFCTileType>();
            foreach (var tile in collapsePossibilities) tiles.Add(tile.type);
            return tiles;
        }
    }
    private class WFCRuleSet
    {
        private class WFCConstraint
        {
            WFCTileType type1;
            WFCTileType type2;
            bool isHorizontal;//If horizontal, type 1 is left and type 2 is right, if vertical, type 1 is top and type 2 is bottom
            public WFCConstraint(Color p1, Color p2, bool isHorizontal)
            {
                type1 = PixelColorToTileType(p1);
                type2 = PixelColorToTileType(p2);
            }
            public bool IsAllowed(WFCTileType t1, WFCTileType t2, int neighbourIndex)
            {
                //neighbourIndex: up = 0 left = 1 down = 2 right = 3
                if(isHorizontal) {
                    switch (neighbourIndex)
                    {
                        case 0:
                        case 2:
                            return false;
                        case 1:
                            return t1 == type2 && t2 == type1;
                        case 3:
                            return t1 == type1 && t2 == type2;
                    }
                }
                else
                {
                    switch (neighbourIndex)
                    {
                        case 1:
                        case 3:
                            return false;
                        case 0:
                            return t1 == type2 && t2 == type1;
                        case 2:
                            return t1 == type1 && t2 == type2;
                    }
                }
                return false;
            }
        }
        List<WFCConstraint> constraints;
        public WFCRuleSet(Color[,] inputImagePixels)
        {
            constraints = new List<WFCConstraint>();
            //For this example we will use a 2d image to represent our floor, each pixel will represent one tile
            ProcessConstraints(inputImagePixels, inputImagePixels.GetLength(0), inputImagePixels.GetLength(1), ref constraints);
        }
        private void ProcessConstraints(Color[,] inputImagePixels, int inputImageWidth, int inputImageHeight, ref List<WFCConstraint> constraints)
        {
            //Horizontal constraints first

            for (int y = 0; y < inputImageHeight; y++)
            {
                for(int x = 0;x < inputImageWidth - 1; x++)
                {
                    WFCConstraint newConstraint = new WFCConstraint(inputImagePixels[x, y], inputImagePixels[x + 1, y], true);
                    if (!constraints.Contains(newConstraint))
                    {
                        constraints.Add(newConstraint);
                    }
                }
            }
            //Then vertical
            for (int x = 0; x < inputImageWidth; x++)
            {
                for (int y = 0; y < inputImageHeight - 1; y++)
                {
                    WFCConstraint newConstraint = new WFCConstraint(inputImagePixels[x, y], inputImagePixels[x, y + 1], false);
                    if (!constraints.Contains(newConstraint))
                    {
                        constraints.Add(newConstraint);
                    }
                }
            }
        }
        public List<CollapsedTile> GetPossibilities(WFCTile[] neighbours, List<CollapsedTile> currentPossibilities)
        {
            List<CollapsedTile> possibilities = new List<CollapsedTile>();
            //neighbours are in order up left down right
            //TODO iterate through each enum type, and each neighbour, ensure it is allowed
            foreach(CollapsedTile t in currentPossibilities)
            {
                WFCTileType tileType = t.type;
                bool tileIsAllowed = true;
                int neighbourIndex = 0;
                foreach (WFCTile neighbour in neighbours)
                {
                    foreach(WFCTileType neighbourType in neighbour.GetPossibilities())
                    {
                        if (!IsPairAllowed(tileType, neighbourType, neighbourIndex))
                        {
                            tileIsAllowed = false;
                        }

                    }
                    neighbourIndex++;
                }
                if (tileIsAllowed)
                {
                    possibilities.Add(new CollapsedTile(tileType));
                }
            }
            return possibilities;
        }
        private static WFCTileType PixelColorToTileType(Color pixelColor)
        {
            switch (pixelColor)
            {
                case Color { r:0,g:0,b:0}:
                    return WFCTileType.Entrance;
                default:
                    return WFCTileType.None;
            }
        }
        private bool IsPairAllowed(WFCTileType centre, WFCTileType neighbour, int neighbourIndex)
        {
            //if any one constraint determines the pair allowed then we return true
            //otherwise false
            foreach(WFCConstraint c in constraints)
            {
                if(c.IsAllowed(centre, neighbour, neighbourIndex))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
