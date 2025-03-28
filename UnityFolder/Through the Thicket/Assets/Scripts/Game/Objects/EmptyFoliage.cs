using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyFoliage : ITileSegmentFoliageLayer
{
    public LayerContents GetContentsEnum()
    {
        return LayerContents.None;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("Empty");
    }

    public int GetTravelCost()
    {
        return 0;
    }
    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption>();
    }
}
