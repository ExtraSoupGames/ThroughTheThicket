using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moss : ITileSegmentFoliageLayer
{
    public LayerContents GetContentsEnum()
    {
        return LayerContents.Moss;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("Moss");
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption>();
    }

    public int GetTravelCost()
    {
        return 1;
    }
}
