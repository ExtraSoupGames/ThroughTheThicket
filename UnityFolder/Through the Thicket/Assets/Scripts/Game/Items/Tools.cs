using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ToolLevelRequirement
{
    None,
    Wood,
    Stone,
    Metal
}
public enum ToolType
{
    MushroomTool,
    TreeTool,
}
public interface ITool : IItem
{
    public ToolType GetToolType();
    public ToolLevelRequirement GetLevelRequirement();
}
