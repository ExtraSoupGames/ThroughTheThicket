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
    public Tile(int PX, int PY, int PHeight, int PChunkX, int PChunkY)
    {
        X = PX;
        Y = PY;
        ChunkX = PChunkX;
        ChunkY = PChunkY;
        Height = PHeight;
        Type = TileTypes.Grass;
        initialized = true;
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
