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
        return GetInteractionOptions(tile, this);
    }
    private List<TileInteractionOption> GetInteractionOptions(GameObject tile, MyceliumTopper topper)
    {
        int X = tile.GetComponent<TileDataHolder>().thisTileData.X;
        int Y = tile.GetComponent<TileDataHolder>().thisTileData.Y;
        return new List<TileInteractionOption> { new TileInteractionOption("Get Mushroom Boost!", new MyceliumBoostOption(X, Y)) };
    }

    public int GetTravelCost()
    {
        return 0;
    }
}
