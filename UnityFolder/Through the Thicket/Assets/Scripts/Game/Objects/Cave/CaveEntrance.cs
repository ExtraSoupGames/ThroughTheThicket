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
        return new List<TileInteractionOption>() { new TileInteractionOption("Enter Cave", new EnterCaveOption(caveID, ref entrance)) };
    }

    public int GetTravelCost()
    {
        return 100;
    }
}
