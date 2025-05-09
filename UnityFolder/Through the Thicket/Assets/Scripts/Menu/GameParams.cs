using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public static class GameParams
{
    public static bool openingNewWorld;
    public static int worldSeed;
    public static int GetActiveWorldSeed()
    {
        if (openingNewWorld) return worldSeed;
        return FileHelper.GetSaveSeed();
    }
}
