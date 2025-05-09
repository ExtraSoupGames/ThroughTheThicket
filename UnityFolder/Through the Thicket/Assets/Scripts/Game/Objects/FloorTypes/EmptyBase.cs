using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyBase : ITileSegmentBaseLayer
{
    public LayerContents GetContentsEnum()
    {
        return LayerContents.None;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("Empty");
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption>();
    }

    public int GetTravelCost()
    {
        return 99999999;
    }
}
