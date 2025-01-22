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
        X = 0;
        Y = 5;
    }
    public Tile(int PX, int PY)
    {
        X = PX;
        Y = PY;
    }
    public void ApplyTileProperties(GameObject TileObject)
    {
        //TODO add texture assignment and height differences
        TileObject.transform.SetPositionAndRotation(new Vector3(X, 0, Y), Quaternion.identity);

    }
}
