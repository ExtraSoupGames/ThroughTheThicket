using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Pebble : IItem, ITileSegmentObjectLayer, ICollectable
{
    public IItem Clone()
    {
        return new Pebble();
    }

    public LayerContents GetContentsEnum()
    {
        return LayerContents.Pebble;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("Pebble");
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption> { new TileInteractionOption("Pick up pebble", new TileDestruction(tile, Layers.Object)) };
    }

    public Items GetItemType()
    {
        return Items.Pebble;
    }

    public int GetMaxStackCount()
    {
        return 20;
    }

    public Texture2D GetTexture()
    {
        return Resources.Load<Sprite>("Icons/Pebbles").texture;
    }

    public ToolLevelRequirement GetToolLevelRequirement()
    {
        throw new System.NotImplementedException();
    }

    public int GetTravelCost()
    {
        return 1;
    }
}
