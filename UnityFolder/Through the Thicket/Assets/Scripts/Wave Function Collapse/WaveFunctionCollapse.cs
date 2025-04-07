using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour
{
    public static void GenerateDungeon(int ID)
    {
        
    }
    private enum WFCTileType
    {
        None,
    }
    private class CollapsedTile
    {
        public WFCTileType type;
        public CollapsedTile(WFCTileType t)
        {
            type = t;
        }
    }
    private class WFCTile
    {
        public int x;
        public int y;
        private List<CollapsedTile> collapsePossibilities;
        private WFCTile()
        {
            foreach(WFCTileType type in Enum.GetValues(typeof(WFCTileType)))
            {
                collapsePossibilities.Add(new CollapsedTile(type));
            }
        }
    }
}
