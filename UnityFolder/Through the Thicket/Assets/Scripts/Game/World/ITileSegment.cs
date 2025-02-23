using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileSegment
{
    public LayerDisplayProperties GetDisplayProperties(TileDisplayGetter displayGetter);
    public int GetTravelCost();
    public LayerContents GetContentsEnum();
}
public interface ITileSegmentBaseLayer : ITileSegment
{

}
public interface ITileSegmentFoliageLayer : ITileSegment
{

}
public interface ITileSegmentObjectLayer : ITileSegment
{

}
