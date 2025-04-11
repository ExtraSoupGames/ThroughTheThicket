using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

public class WaveFunctionCollapse : MonoBehaviour
{
    private static int2[] NeighbourOffsets()
    {
        return new int2[] { new int2(-1, 0), new int2(1, 0), new int2(0, 1), new int2(0,-1)};
    }
    public static void GenerateDungeon(int ID, int size, string filePath, int seed)
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
        //TODO get rules
        WFCRuleSet rules = new WFCRuleSet();
        //Just for testing we iterate a fixed number of times
        //Eventually there will be a win condition
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
            Debug.Log($"Collapsing a tile 0 is min, {collapsePossibilities.Count} is max, and {collapseResultIndex} is result");
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
            collapsePossibilities = rules.GetPossibilities(neighbours);
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
    }
    private class WFCRuleSet
    {
        public WFCRuleSet()
        {
            //TODO implement ruleset generation from input bitmap/tilemap
        }
        public List<CollapsedTile> GetPossibilities(WFCTile[] neighbours)
        {
            //TODO implement a sample ruleset
            return new List<CollapsedTile> { new CollapsedTile(WFCTileType.Entrance),new CollapsedTile(WFCTileType.None)};
        }
    }
}
