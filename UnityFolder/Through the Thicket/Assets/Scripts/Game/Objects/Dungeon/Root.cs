using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : ITileSegmentBaseLayer
{
    public LayerContents GetContentsEnum()
    {
        return LayerContents.Root;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("Root");
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption>();
    }

    public int GetTravelCost()
    {
        return 0;
    }
}
