using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : ITileSegmentBaseLayer
{
    public int GetTravelCost()
    {
        return 3;
    }
    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("Rock");
    }

    public LayerContents GetContentsEnum()
    {
        return LayerContents.Rock;
    }
}
