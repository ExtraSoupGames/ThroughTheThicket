using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : ITileSegmentBaseLayer
{
    public int GetTravelCost()
    {
        return 3;
    }
    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("Stone");
    }

    public LayerContents GetContentsEnum()
    {
        return LayerContents.Stone;
    }
    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption>();
    }
}
