using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DungeonManager : ChunkManager
{
    int dungeonID;
    protected override string GetChunkPath()
    {
        return Path.Combine(Application.persistentDataPath, "World", "DungeonData", "Chunks", dungeonID.ToString());
    }
    protected override bool UseSurfaceGenerator()
    {
        return false;
    }
    public void SetID(int ID)
    {
        dungeonID = ID;
    }
    public void ClearDungeon()
    {
        RemoveTilesFromChunk(new ChunkPos(0, 0));
        ClearActiveChunks();
        UpdateRequiredChunks();
    }
    protected override int GetSeedModifier()
    {
        return dungeonID;
    }
}
