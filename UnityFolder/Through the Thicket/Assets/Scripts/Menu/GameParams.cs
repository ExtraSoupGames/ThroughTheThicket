using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameParams
{
    public static bool openingNewWorld;
    public static int worldSave;
    public static int worldSeed;
    public static int GetActiveWorldSeed()
    {
        if (openingNewWorld) return worldSeed;
        //TODO read world seed from file
        //TODO save world seed to file
        //TODO verify files ect
        //TODO show saves
        return 0;
    }
}
