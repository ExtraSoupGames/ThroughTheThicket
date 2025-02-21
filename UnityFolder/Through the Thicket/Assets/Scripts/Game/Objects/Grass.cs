using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : ITileSegmentBaseLayer
{
    public int GetTravelCost()
    {
        return 3;
    }
    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("Grass");
    }

    public LayerContents GetContentsEnum()
    {
        return LayerContents.Grass;
    }
}
