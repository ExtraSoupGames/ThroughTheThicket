using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Chanterelle : IItem, ITileSegmentObjectLayer
{
    public IItem Clone()
    {
        return new Chanterelle();
    }

    public LayerContents GetContentsEnum()
    {
        return LayerContents.Chanterelle;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("Chanterelle");
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption> { new TileInteractionOption("Pick up chanterelle", new TileDestruction(tile, Layers.Object)) };
    }

    public Items GetItemType()
    {
        return Items.Chanterelle;
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
