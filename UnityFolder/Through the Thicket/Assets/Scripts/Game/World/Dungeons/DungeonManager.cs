using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DungeonManager : ChunkManager
{
    protected override string GetChunkPath()
    {
        return Path.Combine(Application.persistentDataPath, "World", "DungeonData", "Chunks");
    }
    public void OtherTests()
    {
        DeleteAllChunks();
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Chunk chunk = new Chunk(i, j, true);
                SaveChunk(chunk);
            }
        }
    }
}
