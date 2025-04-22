using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonExit : ITileSegmentObjectLayer
{
    public int dungeonID;
    public LayerContents GetContentsEnum()
    {
        return LayerContents.DungeonExit;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("DungeonExit");
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        int X = tile.GetComponent<TileDataHolder>().thisTileData.X;
        int Y = tile.GetComponent<TileDataHolder>().thisTileData.Y;
        return new List<TileInteractionOption>() { new TileInteractionOption("Exit Dungeon", new ExitDungeonOption()) };
    }

    public int GetTravelCost()
    {
        return 100;
    }
}