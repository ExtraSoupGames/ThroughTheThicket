using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
public enum Layers
{
    Base,
    Foliage,
    Object
}
public enum LayerContents
{
    None,
    Grass,
    Foliage,
    Stone,
    Pebble,
    Rock,
    Carrot,
    Potato,
    Redcap,
    Morel,
    Chanterelle,
    Portobello,
    Flint,
    Campfire,
    River,
    TallGrass,
    TreeStump,
    DungeonEntrance,
    DungeonExit,
    Moss,
    MyceliumBase,
    MyceliumTopper,
    Root,
    MuddyRoot
}
[System.Serializable]
public struct TileSegmentDataHolder
{
    public LayerContents baseType;
    public LayerContents foliageType;
    public LayerContents objectType;
    public TileSegmentDataHolder(LayerContents baseLayerType, LayerContents FoliageLayerType, LayerContents ObjectLayerType)
    {
        baseType = baseLayerType;
        foliageType = FoliageLayerType;
        objectType = ObjectLayerType;
    }
    public ITileSegmentBaseLayer ConstructBase()
    {
        switch (baseType)
        {
            case LayerContents.Grass:
                return new Grass();
            case LayerContents.Stone:
                return new Stone();
            case LayerContents.River:
                return new River();
            case LayerContents.MyceliumBase:
                return new MyceliumBase();
            case LayerContents.MuddyRoot:
                return new MuddyRoot();
            case LayerContents.Root:
                return new Root();
            default:
                return new EmptyBase();
        }
    }
    public ITileSegmentFoliageLayer ConstructFoliage()
    {
        switch (foliageType)
        {
            case LayerContents.Foliage:
                return new Twigs();
            case LayerContents.Redcap:
                return new Redcap();
            case LayerContents.Potato:
                return new Potato();
            case LayerContents.Carrot:
                return new Carrot();
            case LayerContents.TallGrass:
                return new TallGrass();
            case LayerContents.TreeStump:
                return new TreeStump();
            case LayerContents.Moss:
                return new Moss();
            case LayerContents.MyceliumTopper:
                return new MyceliumTopper();
            default:
                return new EmptyFoliage();
        }
    }
    public ITileSegmentObjectLayer ConstructObject()
    {
        switch (objectType)
        {
            case LayerContents.Pebble:
                return new Pebble();
            case LayerContents.Morel:
                return new Morel();
            case LayerContents.Portobello:
                return new Portobello();
            case LayerContents.Chanterelle:
                return new Chanterelle();
            case LayerContents.Flint:
                return new Flint();
            case LayerContents.Campfire:
                return new CampFire();
            case LayerContents.DungeonEntrance:
                return new DungeonEntrance();
            case LayerContents.DungeonExit:
                return new DungeonExit();
            default:
                return new EmptyObject();
        }
    }
}
[System.Serializable]
public struct Tile
{
    public int X;
    public int Y;
    public int ChunkX;
    public int ChunkY;
    public int Height;
    public TileSegmentDataHolder Layers;
    public bool initialized;
    public Tile(int PX, int PY, int PHeight, int PChunkX, int PChunkY, TileSegmentDataHolder PLayers)
    {
        X = PX;
        Y = PY;
        ChunkX = PChunkX;
        ChunkY = PChunkY;
        Height = PHeight;
        Layers = PLayers;
        initialized = true;
    }
    public Tile(int PX, int PY, int PHeight, int PChunkX, int PChunkY, ITileSegmentBaseLayer baseTile, ITileSegmentFoliageLayer foliageLayer, ITileSegmentObjectLayer objectLayer)
: this(PX, PY, PHeight, PChunkX, PChunkY, new TileSegmentDataHolder(baseTile.GetContentsEnum(), foliageLayer.GetContentsEnum(), objectLayer.GetContentsEnum()))
    {
    }
    public Tile(int PX, int PY, int PHeight, int PChunkX, int PChunkY, ITileSegmentBaseLayer baseTile)
    : this(PX, PY, PHeight, PChunkX, PChunkY, new TileSegmentDataHolder(baseTile.GetContentsEnum(), LayerContents.None, LayerContents.None))
    {
    }
    //unsure if these chunk coordinate calculations work or not
    public Tile(int PX, int PY, int PHeight, ITileSegmentBaseLayer baseTile) : this(PX, PY, PHeight, (int)(PX / 16), (int)(PY / 16), baseTile)
    {

    }
    public Tile(int PX, int PY, int PHeight, int PChunkX, int PChunkY) : this(PX, PY, PHeight, PChunkX, PChunkY, new Grass())
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
    public ITileSegmentBaseLayer BaseType;
    public ITileSegmentFoliageLayer FoliageType;
    public ITileSegmentObjectLayer ObjectType;

    public ProcessedTileData(Tile tile)
    {
        if (!tile.initialized)
        {
            Debug.LogError("Tried to process a tile which was never initialized");
        }
        X = tile.X;
        Y = tile.Y;
        ChunkX = tile.ChunkX;
        ChunkY = tile.ChunkY;
        Height = tile.Height;
        BaseType = tile.Layers.ConstructBase();
        FoliageType = tile.Layers.ConstructFoliage();
        ObjectType = tile.Layers.ConstructObject();
        TravelCost = BaseType.GetTravelCost() + FoliageType.GetTravelCost() + ObjectType.GetTravelCost();
    }

    public void ApplyTileProperties(GameObject TileObject, TileDisplayGetter displayGetter)
    {
        TileObject.SetActive(true);
        GameObject baseLayer = TileObject.transform.GetChild(0).gameObject;
        LayerDisplayProperties baseDisplay = BaseType.GetDisplayProperties(displayGetter);
        GameObject foliageLayer = TileObject.transform.GetChild(1).gameObject;
        LayerDisplayProperties foliageDisplay = FoliageType.GetDisplayProperties(displayGetter);
        GameObject objectLayer = TileObject.transform.GetChild(2).gameObject;
        LayerDisplayProperties objectDisplay = ObjectType.GetDisplayProperties(displayGetter);
        TileObject.GetComponent<TileDataHolder>().thisTileData = this;
        TileObject.GetComponent<TileDataHolder>().initialized = true;
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
    public TileInteractionMenu GetInteractionOptions(GameObject tile)
    {
        TileInteractionMenu menu = new TileInteractionMenu();
        menu.AddOption(new TileInteractionOption("Exit", new TileInteractionExit()));
        menu.AddOptions(BaseType.GetInteractionOptions(tile));
        menu.AddOptions(FoliageType.GetInteractionOptions(tile));
        menu.AddOptions(ObjectType.GetInteractionOptions(tile));
        return menu;
    }
}
