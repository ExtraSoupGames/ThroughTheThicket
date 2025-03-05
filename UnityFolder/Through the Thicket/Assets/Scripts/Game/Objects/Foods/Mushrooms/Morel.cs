using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Morel : IItem, ITileSegmentObjectLayer
{
    public IItem Clone()
    {
        return new Morel();
    }

    public LayerContents GetContentsEnum()
    {
        return LayerContents.Morel;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("Morel");
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption> { new TileInteractionOption("Pick up morel", new TileDestruction(tile, Layers.Object)) };
    }

    public Items GetItemType()
    {
        return Items.Morel;
    }

    public int GetMaxStackCount()
    {
        return 5;
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
