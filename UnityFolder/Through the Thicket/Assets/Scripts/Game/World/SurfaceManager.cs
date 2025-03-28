using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Timeline;
using UnityEngine;

public class SurfaceManager : ChunkManager
{
    protected override string GetChunkPath()
    {
        return Path.Combine(Application.persistentDataPath, "World", "SurfaceData", "Chunks");
    }
    protected override bool UseSurfaceGenerator()
    {
        return true;
    }
}
