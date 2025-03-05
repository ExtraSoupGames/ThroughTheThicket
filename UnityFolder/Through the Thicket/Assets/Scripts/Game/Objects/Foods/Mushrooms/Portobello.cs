using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Portobello : IItem, ITileSegmentObjectLayer
{
    public IItem Clone()
    {
        return new Portobello();
    }

    public LayerContents GetContentsEnum()
    {
        return LayerContents.Portobello;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("Portobello");
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption> { new TileInteractionOption("Pick up portobello", new TileDestruction(tile, Layers.Object)) };
    }

    public Items GetItemType()
    {
        return Items.Portobello;
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
