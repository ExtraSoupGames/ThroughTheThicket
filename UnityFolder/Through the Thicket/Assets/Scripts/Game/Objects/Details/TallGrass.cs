using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TallGrass : ITileSegmentFoliageLayer
{

    public LayerContents GetContentsEnum()
    {
        return LayerContents.TallGrass;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("TallGrass");
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption> {  };
    }

    public int GetTravelCost()
    {
        return 1;
    }
}
