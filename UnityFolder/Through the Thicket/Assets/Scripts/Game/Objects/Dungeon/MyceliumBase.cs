using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyceliumBase : ITileSegmentBaseLayer
{
    public LayerContents GetContentsEnum()
    {
        return LayerContents.MyceliumBase;
    }

    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter)
    {
        LayerDisplayProperties displayProperties = displayGetter.GetDisplayProperties("MyceliumBase");
        Debug.Log("Display properties for mycelium: " + displayProperties.LayerMesh.ToString() + " -> " + displayProperties.LayerMats.Length.ToString());
        return displayGetter.GetDisplayProperties("MyceliumBase");
    }

    public List<TileInteractionOption> GetInteractionOptions(GameObject tile)
    {
        return new List<TileInteractionOption> { new TileInteractionOption("Testing", new TileInteractionExit())};
    }

    public int GetTravelCost()
    {
        return 0;
    }
}
