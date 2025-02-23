using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlacable : IItem, ITileSegment
{
    public Layers GetLayer();
}
