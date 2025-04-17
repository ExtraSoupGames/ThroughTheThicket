using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveEntrance : ITileSegmentObjectLayer
{
    public int caveID;
    public LayerContents GetContentsEnum()
    {
        return LayerContents.CaveEntrance;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("CaveEntrance");
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return GetInteractionOptions(tile, this);
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile, CaveEntrance entrance)
    {
        int X = tile.GetComponent<TileDataHolder>().thisTileData.X;
        int Y = tile.GetComponent<TileDataHolder>().thisTileData.Y;
        return new List<TileInteractionOption>() { new TileInteractionOption("Enter Cave", new EnterCaveOption(caveID, ref entrance, X, Y)) };
    }

    public int GetTravelCost()
    {
        return 100;
    }
}
