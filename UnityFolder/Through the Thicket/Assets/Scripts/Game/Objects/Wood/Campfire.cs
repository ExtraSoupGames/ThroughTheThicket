using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampFire : IItem, ITileSegmentObjectLayer, ICollectable, IPlacable
{
    public IItem Clone()
    {
        return new CampFire();
    }

    public LayerContents GetContentsEnum()
    {
        return LayerContents.Campfire;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        return displayGetter.GetDisplayProperties("Carrot");
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption> { new TileInteractionOption("Pick up campfire", new TileDestruction(tile, Layers.Object)) };
    }

    public Items GetItemType()
    {
        return Items.Campfire;
    }

    public Layers GetLayer()
    {
        return Layers.Object;
    }

    public int GetMaxStackCount()
    {
        return 20;
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
        return 1;
    }
}
