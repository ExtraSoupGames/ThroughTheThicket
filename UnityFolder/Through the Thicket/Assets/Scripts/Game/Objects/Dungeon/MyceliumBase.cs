using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyceliumBase : ITileSegmentBaseLayer
{
    public LayerContents GetContentsEnum()
    {
        return LayerContents.MyceliumBase;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("MyceliumBase");
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption> { new TileInteractionOption("Testing", new TileInteractionExit())};
    }

    public int GetTravelCost()
    {
        return 0;
    }
}
