using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
public enum TileTypes
{
    Grass
}
[System.Serializable]
public struct Tile
{
    public int X;
    public int Y;
    public int Height;
    public TileTypes Type;
    public bool initialized;
    public Tile(int PX, int PY, int PHeight)
    {
        X = PX;
        Y = PY;
        Height = PHeight;
        Type = TileTypes.Grass;
        initialized = true;
    }
    public Tile(int PX, int PY) : this(PX, PY, 1)
    {
    }
    public void ApplyTileProperties(GameObject TileObject)
    {
        //TODO add texture assignment and height differences
        if (!initialized)
        {
            TileObject.SetActive(false);
            return;
        }
        TileObject.SetActive(true);
        TileObject.transform.SetPositionAndRotation(new Vector3(X, Height, Y), Quaternion.identity);
    }
}
