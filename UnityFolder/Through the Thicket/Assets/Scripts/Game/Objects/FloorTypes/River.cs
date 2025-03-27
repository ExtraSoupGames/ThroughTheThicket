using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River : ITileSegmentBaseLayer
{
    public int GetTravelCost()
    {
        return 15;
    }
    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("River");
    }

    public LayerContents GetContentsEnum()
    {
        return LayerContents.River;
    }
    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption>();
    }
}
