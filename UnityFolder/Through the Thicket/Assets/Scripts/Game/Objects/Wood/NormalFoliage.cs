using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Twigs : ITileSegmentFoliageLayer, ICollectable
{
    public LayerContents GetContentsEnum()
    {
        return LayerContents.Foliage;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("Foliage");
    }

    public int GetTravelCost()
    {
        return 0;
    }
    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption> { new TileInteractionOption("Collect twigs", new TileDestruction(tile, Layers.Foliage)) };
    }

    public ToolLevelRequirement GetToolLevelRequirement()
    {
        return ToolLevelRequirement.None;
    }

    public Items GetItemType()
    {
        return Items.Twigs;
    }

    public Texture2D GetTexture()
    {
        return Resources.Load<Sprite>("Shaped10").texture;
    }

    public int GetMaxStackCount()
    {
        return 10;
    }

    public IItem Clone()
    {
        return new Twigs();
    }
}
