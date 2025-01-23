using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BiomeType
{
    Forest,
    DyingForest
}
//A biome is a section of land
[System.Serializable]
public class Biome
{
    public List<Tile> Tiles;
    public int CentreX;
    public int CentreY;
    public bool MarkedForDeletion { get; set; }
    //the type of biome
    public BiomeType Type { get; }
    //the default biome constructor to load a biome from a string of data
    public Biome(string biomeData)
    {
        //initiate tile list
        Tiles = new List<Tile>();
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Tiles.Add(new Tile(i, j));
            }
        }
        CentreX = 5;
        CentreY = 5;
        Type = BiomeType.Forest;
    }
    //JUST FOR TESTING TODO remove this
    public Biome(int x, int y)
    {
        //initiate tile list
        Tiles = new List<Tile>();
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Tiles.Add(new Tile(x + i, y + j));
            }
        }
        CentreX = x;
        CentreY = y;
        Type = BiomeType.Forest;
    }
    public Biome() : this("DefaultBiomeDataGoesHere") // TODO add default biome data for testing
    {

    }
    //check to see if a tile with given coordinates is contained within this biome
    public bool ContainsTile(int X, int Y)
    {
        bool tileIsContained = false;
        foreach(Tile t in Tiles)
        {
            if(t.X == X && t.Y == Y)
            {
                tileIsContained = true;
            }
        }
        return tileIsContained;
    }
    public List<Tile> GetActiveTiles(int PlayerX, int PlayerY, int biomeRange)
    {
        List<Tile > activeTiles = new List<Tile>();
        foreach (Tile tile in Tiles)
        {
            if(MathsHelper.FindDistance(tile.X, tile.Y, PlayerX, PlayerY) < biomeRange)
            {
                activeTiles.Add(tile);
            }
        }
        return activeTiles;
    }
}