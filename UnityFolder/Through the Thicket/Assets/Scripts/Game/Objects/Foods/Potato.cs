using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Potato : IItem, ITileSegmentFoliageLayer, ICollectable
{
    public IItem Clone()
    {
        return new Potato();
    }

    public LayerContents GetContentsEnum()
    {
        return LayerContents.Potato;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("Potato");
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption> { new TileInteractionOption("Pick up potatoes", new TileDestruction(tile, Layers.Foliage)) };
    }

    public Items GetItemType()
    {
        return Items.Potato;
    }

    public int GetMaxStackCount()
    {
        return 10;
    }

    public Texture2D GetTexture()
    {
        return Resources.Load<Sprite>("TestSprite").texture;
    }

    public ToolLevelRequirement GetToolLevelRequirement()
    {
        throw new System.NotImplementedException();
    }

    public int GetTravelCost()
    {
        return 2;
    }
}
