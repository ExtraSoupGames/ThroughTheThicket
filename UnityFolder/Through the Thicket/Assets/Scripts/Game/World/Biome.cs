using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BiomeType
{
    Forest,
    DyingForest
}
//A biome is a section of land
public class Biome
{
    public bool MarkedForDeletion { get; set; }
    //the type of biome
    public BiomeType Type { get; }
    //the serializable data of the biome
    public BiomeData Data;
    //the default biome constructor to load a biome from a string of data
    public Biome(string biomeData)
    {
        //TODO add loading biome from data
        Data = new BiomeData();
    }
    public Biome() : this("DefaultBiomeDataGoesHere") // TODO add default biome data for testing
    {

    }
    //check to see if a tile with given coordinates is contained within this biome
    public bool ContainsTile(int X, int Y)
    {
        bool tileIsContained = false;
        foreach(Tile t in Data.Tiles)
        {
            if(t.X == X && t.Y == Y)
            {
                tileIsContained = true;
            }
        }
        return tileIsContained;
    }
}
[System.Serializable]
public class BiomeData
{
    public List<Tile> Tiles;
    public int CentreX;
    public int CentreY;
    public BiomeData()
    {
        Tiles = new List<Tile>();
        for(int i = 0; i < 10; i++)
        {
            for(int j = 0;j < 10; j++)
            {
                Tiles.Add(new Tile(i,j));
            }
        }
        CentreX = 5;
        CentreY = 5;
    }
}