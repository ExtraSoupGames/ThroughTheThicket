using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Flint : IItem, ITileSegmentObjectLayer
{
    public IItem Clone()
    {
        return new Flint();
    }

    public LayerContents GetContentsEnum()
    {
        return LayerContents.Flint;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("Flint");
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption> { new TileInteractionOption("Pick up flint", new TileDestruction(tile, Layers.Object)) };
    }

    public Items GetItemType()
    {
        return Items.Flint;
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
