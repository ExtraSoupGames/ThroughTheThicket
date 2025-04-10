using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

public class WaveFunctionCollapse : MonoBehaviour
{
    public static void GenerateDungeon(int ID, int size)
    {
        //Initialize a random number engine for collapsing tiles
        Unity.Mathematics.Random randomEngine = new Unity.Mathematics.Random();
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
                tiles[i, j] = new WFCTile();
                //Update the entropy grid accordingly
                entropies[i, j] = tiles[i, j].CalculateEntropy();
            }
        }
        //set the middle tile to an entrance tile
        tiles[size, size] = new WFCTile(WFCTileType.Entrance);
        //Update the entropy grid accordingly
        entropies[size, size] = tiles[size, size].CalculateEntropy();

        //Just for testing we iterate a fixed number of times
        //Eventually there will be a win condition
        for(int i = 0; i < 100; i++)
        {
            //Update all entropies and collapse the tile with the smallest entropy
            int2 lowestEntropyPosition = UpdateAllEntropies(tiles, entropies);
            int x = lowestEntropyPosition.x;
            int y = lowestEntropyPosition.y;
            tiles[x, y].Collapse(randomEngine);

            //TODO update possible tiles for surrounding tiles based on ruleset generated from input
        }
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
        public WFCTile()
        {
            foreach(WFCTileType type in Enum.GetValues(typeof(WFCTileType)))
            {
                collapsePossibilities.Add(new CollapsedTile(type));
            }
        }
        //Used for instantiating definite tiles (tiles decided by the designer not by the algorithm)
        public WFCTile(WFCTileType type)
        {
            collapsePossibilities.Add(new CollapsedTile(type));
        }
        public int CalculateEntropy()
        {
            return collapsePossibilities.Count;
        }
        public void Collapse(Unity.Mathematics.Random randomEngine)
        {
            int collapseResultIndex = randomEngine.NextInt(1, collapsePossibilities.Count);
            collapsePossibilities = new List<CollapsedTile> { collapsePossibilities[collapseResultIndex] };
        }
    }
}
