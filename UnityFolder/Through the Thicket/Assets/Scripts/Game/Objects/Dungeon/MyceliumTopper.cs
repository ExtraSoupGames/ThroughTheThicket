using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyceliumTopper : ITileSegmentFoliageLayer
{
    public LayerContents GetContentsEnum()
    {
        return LayerContents.MyceliumTopper;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("MyceliumTopper");
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption> { new TileInteractionOption("Get Mushroom Boost!", new MyceliumBoostOption(tile.GetComponent<TileDataHolder>().thisTileData.X, tile.GetComponent<TileDataHolder>().thisTileData.Y)) };
    }

    public int GetTravelCost()
    {
        return 0;
    }
}
