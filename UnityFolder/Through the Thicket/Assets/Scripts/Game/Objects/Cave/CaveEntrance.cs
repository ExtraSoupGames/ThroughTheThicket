using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveEntrance : ITileSegmentObjectLayer
{
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
        return new List<TileInteractionOption>() { new TileInteractionOption("Enter Cave", new EnterCaveOption()) };
    }

    public int GetTravelCost()
    {
        return 100;
    }
}
