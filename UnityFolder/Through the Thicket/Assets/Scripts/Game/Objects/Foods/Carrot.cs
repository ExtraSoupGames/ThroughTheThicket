using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Carrot : IItem, ITileSegmentFoliageLayer, ICollectable
{
    public IItem Clone()
    {
        return new Carrot();
    }

    public LayerContents GetContentsEnum()
    {
        return LayerContents.Carrot;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("Carrot");
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption> { new TileInteractionOption("Pick up carrots", new TileDestruction(tile, Layers.Foliage)) };
    }

    public Items GetItemType()
    {
        return Items.Carrot;
    }

    public int GetMaxStackCount()
    {
        return 10;
    }

    public Texture2D GetTexture()
    {
        return Resources.Load<Sprite>("Icons/Carrot").texture;
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
