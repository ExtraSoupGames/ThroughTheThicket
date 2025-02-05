using System;
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
    public int ChunkX;
    public int ChunkY;
    public int Height;
    public TileTypes Type;
    public bool initialized;
    public Tile(int PX, int PY, int PHeight, int PChunkX, int PChunkY, TileTypes tileType)
    {
        X = PX;
        Y = PY;
        ChunkX = PChunkX;
        ChunkY = PChunkY;
        Height = PHeight;
        Type = tileType;
        initialized = true;
    }
    //unsure if these chunk coordinate calculations work or not
    public Tile(int PX, int PY, int PHeight, TileTypes tileType) : this(PX, PY, PHeight, (int)(PX / 16), (int)(PY / 16), tileType)
    {

    }
    public Tile(int PX, int PY, int PHeight, int PChunkX, int PChunkY) : this(PX, PY, PHeight, PChunkX, PChunkY, TileTypes.Grass)
    {

    }
    public Tile(int PX, int PY, int PChunkX, int PChunkY) : this(PX, PY, 1, PChunkX, PChunkY)
    {
    }
}
public struct ProcessedTileData
{
    public int X;
    public int Y;
    public int ChunkX;
    public int ChunkY;
    public int Height;
    public int TravelCost;
    //ID to retrieve texture from a big dictionary somewhere
    public int textureID;

    public ProcessedTileData(Tile tile)
    {
        X = tile.X;
        Y = tile.Y;
        ChunkX = tile.ChunkX;
        ChunkY = tile.ChunkY;
        Height = tile.Height;
        textureID = 1;
        TravelCost = 1;
    }

    public void ApplyTileProperties(GameObject TileObject)
    {
        TileObject.SetActive(true);
        TileObject.transform.SetPositionAndRotation(new Vector3(X, Height, Y), Quaternion.identity);
    }

    public bool IsInPlayerRange(float playerX, float playerY, float maxDist)
    {
        float dX = playerX - X;
        float dY = playerY - Y;
        float distance = MathF.Sqrt((dX * dX) + (dY * dY));
        return distance < maxDist;
    }
}
