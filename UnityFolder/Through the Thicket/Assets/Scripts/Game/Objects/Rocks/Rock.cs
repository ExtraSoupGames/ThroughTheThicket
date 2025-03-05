using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Rock : IItem, ITileSegmentObjectLayer
{
    public IItem Clone()
    {
        return new Rock();
    }

    public LayerContents GetContentsEnum()
    {
        return LayerContents.Rock;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("Rock");
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption> { new TileInteractionOption("Pick up Rock", new TileDestruction(tile, Layers.Object)) };
    }

    public Items GetItemType()
    {
        return Items.Rock;
    }

    public int GetMaxStackCount()
    {
        return 10;
    }

    public Texture2D GetTexture()
    {
        return Resources.Load<Sprite>("TestSprite").texture;
    }

    public int GetTravelCost()
    {
        return 1;
    }
}
