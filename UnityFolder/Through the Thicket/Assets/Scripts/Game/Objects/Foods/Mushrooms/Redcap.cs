using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Redcap : IItem, ITileSegmentFoliageLayer, ICollectable
{
    public IItem Clone()
    {
        return new Redcap();
    }

    public LayerContents GetContentsEnum()
    {
        return LayerContents.Redcap;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("Redcap");
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption> { new TileInteractionOption("Pick up redcaps", new TileDestruction(tile, Layers.Foliage)) };
    }

    public Items GetItemType()
    {
        return Items.Redcap;
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
