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
    public int Height;
    public TileTypes Type;
    public Tile(int PX, int PY, int PHeight)
    {
        X = PX;
        Y = PY;
        Height = PHeight;
    }
    public Tile(int PX, int PY) : this(PX, PY, 1)
    {
    }
    public Tile() : this(0, 0, 1)
    {
    }
    public void ApplyTileProperties(GameObject TileObject)
    {
        //TODO add texture assignment and height differences
        TileObject.transform.SetPositionAndRotation(new Vector3(X, Height, Y), Quaternion.identity);
    }
}
