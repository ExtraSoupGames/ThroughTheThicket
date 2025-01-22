using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TileTypes
{
    Grass
}
[System.Serializable]
public class Tile
{
    public int X;
    public int Y;
    public TileTypes Type;
    public Tile()
    {
        X = 5;
        Y = 10;
    }
}
