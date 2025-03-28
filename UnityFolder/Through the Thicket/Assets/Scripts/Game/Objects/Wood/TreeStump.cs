using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TreeStump : ITileSegmentFoliageLayer 
{
    public LayerContents GetContentsEnum()
    {
        return LayerContents.TreeStump;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("TreeStump");
    }

    public int GetTravelCost()
    {
        return 100;
    }
    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption> { };
    }
}
