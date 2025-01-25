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
public struct Biome
{
    public List<Tile> Tiles;
    public int CentreX;
    public int CentreY;
    public bool MarkedForDeletion { get; set; }
    //the type of biome
    public BiomeType Type { get; }
    //the default biome constructor to generate a biome with requirements about location and size, as well as anything else
    /*
    public Biome(string requirements)
    {
        //TODO process
    }
    */
    //JUST FOR TESTING TODO remove this
    public Biome(int x, int y, int biomeSpacing)
    {
        //initiate tile list
        Tiles = new List<Tile>();
        for (int i = 0; i < biomeSpacing; i++)
        {
            for (int j = 0; j < biomeSpacing; j++)
            {
                int biomeCentreOffset = biomeSpacing / 2;
                Tiles.Add(new Tile(x + i - biomeCentreOffset, y + j - biomeCentreOffset, UnityEngine.Random.Range(0, 2)));
            }
        }
        CentreX = x;
        CentreY = y;
        Type = BiomeType.Forest;
        MarkedForDeletion = false;
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
    public void SetMarkedForDeletion()
    {
        MarkedForDeletion = true;
    }
}