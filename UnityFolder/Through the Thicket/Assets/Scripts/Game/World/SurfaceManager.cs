using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SurfaceManager : ChunkManager
{
    protected override string GetChunkPath()
    {
        return Path.Combine(Application.persistentDataPath, "World", "SurfaceData", "Chunks");
    }
}
