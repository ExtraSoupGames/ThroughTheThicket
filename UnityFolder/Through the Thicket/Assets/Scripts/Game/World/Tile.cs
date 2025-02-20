using System;
using UnityEngine;
public enum Layers
{
    Base,
    Foliage,
    Object
}
public enum ObjectTypes
{
    None,
}
public enum FoliageTypes
{
    None,
    DefaultFoliage,
}
public enum BaseTypes
{
    None,
    Grass,
    Stone,
}
[System.Serializable]
public struct Tile
{
    public int X;
    public int Y;
    public int ChunkX;
    public int ChunkY;
    public int Height;
    public BaseTypes BaseType;
    public FoliageTypes FoliageType;
    public ObjectTypes ObjectType;
    public bool initialized;
    public Tile(int PX, int PY, int PHeight, int PChunkX, int PChunkY, BaseTypes tileType)
    {
        X = PX;
        Y = PY;
        ChunkX = PChunkX;
        ChunkY = PChunkY;
        Height = PHeight;
        BaseType = tileType;
        FoliageType = PX % 2 == 1 ? FoliageTypes.None : FoliageTypes.DefaultFoliage;
        ObjectType = ObjectTypes.None;
        initialized = true;
    }
    //unsure if these chunk coordinate calculations work or not
    public Tile(int PX, int PY, int PHeight, BaseTypes tileType) : this(PX, PY, PHeight, (int)(PX / 16), (int)(PY / 16), tileType)
    {

    }
    public Tile(int PX, int PY, int PHeight, int PChunkX, int PChunkY) : this(PX, PY, PHeight, PChunkX, PChunkY, BaseTypes.Grass)
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
    public BaseTypes BaseType;
    public FoliageTypes FoliageType;
    public ObjectTypes ObjectType;

    public ProcessedTileData(Tile tile)
    {
        X = tile.X;
        Y = tile.Y;
        ChunkX = tile.ChunkX;
        ChunkY = tile.ChunkY;
        Height = tile.Height;
        TravelCost = tile.BaseType == BaseTypes.Grass ? 1 : 10;
        BaseType = tile.BaseType;
        FoliageType = tile.FoliageType;
        ObjectType = tile.ObjectType;
    }

    public void ApplyTileProperties(GameObject TileObject, TileDisplayGetter displayGetter)
    {
        TileObject.SetActive(true);
        GameObject baseLayer = TileObject.transform.GetChild(0).gameObject;
        LayerDisplayProperties baseDisplay = displayGetter.GetDisplayProperties(Layers.Base, (int)BaseType);
        GameObject foliageLayer = TileObject.transform.GetChild(1).gameObject;
        LayerDisplayProperties foliageDisplay = displayGetter.GetDisplayProperties(Layers.Foliage, (int)FoliageType);
        GameObject objectLayer = TileObject.transform.GetChild(2).gameObject;
        LayerDisplayProperties objectDisplay = displayGetter.GetDisplayProperties(Layers.Object, (int)ObjectType);
        if (baseDisplay.isEmpty)
        {
            baseLayer.SetActive(false);
        }
        else
        {
            baseLayer.SetActive(true);
            baseLayer.GetComponent<MeshFilter>().mesh = baseDisplay.LayerMesh;
            baseLayer.GetComponent<MeshRenderer>().materials = baseDisplay.LayerMats;
        }
        if (foliageDisplay.isEmpty)
        {
            foliageLayer.SetActive(false);
        }
        else
        {
            foliageLayer.SetActive(true);
            foliageLayer.GetComponent<MeshFilter>().mesh = foliageDisplay.LayerMesh;
            foliageLayer.GetComponent<MeshRenderer>().materials = foliageDisplay.LayerMats;
        }
        if (objectDisplay.isEmpty)
        {
            objectLayer.SetActive(false);
        }
        else
        {
            objectLayer.SetActive(true);
            objectLayer.GetComponent<MeshFilter>().mesh = objectDisplay.LayerMesh;
            objectLayer.GetComponent<MeshRenderer>().materials = objectDisplay.LayerMats;
        }
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
