using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEntrance : ITileSegmentObjectLayer
{
    public int dungeonID;
    public LayerContents GetContentsEnum()
    {
        return LayerContents.DungeonEntrance;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("DungeonEntrance");
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return GetInteractionOptions(tile, this);
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile, DungeonEntrance entrance)
    {
        int X = tile.GetComponent<TileDataHolder>().thisTileData.X;
        int Y = tile.GetComponent<TileDataHolder>().thisTileData.Y;
        return new List<TileInteractionOption>() { new TileInteractionOption("Enter Dungeon", new EnterDungeonOption(dungeonID, ref entrance, X, Y)) };
    }

    public int GetTravelCost()
    {
        return 100;
    }
}
