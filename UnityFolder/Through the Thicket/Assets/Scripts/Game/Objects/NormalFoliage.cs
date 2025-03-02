using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalFoliage : ITileSegmentFoliageLayer
{
    public LayerContents GetContentsEnum()
    {
        return LayerContents.Foliage;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("Foliage");
    }

    public int GetTravelCost()
    {
        return 0;
    }
    public List<TileInteractionOption> GetInteractionOptions()
    {
        return new List<TileInteractionOption>();
    }
}
