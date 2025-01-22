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
    //the type of biome
    public BiomeType Type { get; }
    //the default biome constructor to load a biome from a string of data
    public Biome(string biomeData)
    {
        //TODO add loading biome from data
    }
    public Biome() : this("DefaultBiomeDataGoesHere") // TODO add default biome data for testing
    {

    }
}
[System.Serializable]
public class BiomeData
{
    public List<Tile> Tiles { get; set; }
}