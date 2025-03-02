using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TileInteractionOption
{
    string displayText;
    public TileInteractionOption(string displayText)
    { 
        this.displayText = displayText; 
    }
    public string GetDisplay()
    {
        return displayText;
    }

}
public class TileInteractionMenu
{
    List<TileInteractionOption> options;
    public TileInteractionMenu()
    {
        options = new List<TileInteractionOption>();
    }
    public void AddOptions(List<TileInteractionOption> newOptions)
    {
        options.AddRange(newOptions);
    }
    public List<TileInteractionOption> GetOptions()
    {
        return options;
    }
}
