using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileHelper 
{
    //ensure all required directories exists
    public static void DirectoryCheck()
    {
        List<string>[] requiredDirectories = { 
            new List<string>{ "World", "SurfaceData", "Chunks" }, 
            new List<string>{ "World", "DungeonData", "Chunks"},
            new List<string>{ "World", "Inventory"} };
        for (int i = 0; i < requiredDirectories.Length; i++)
        {
            string path = Application.persistentDataPath;
            for(int j = 0; j < requiredDirectories[i].Count; j++)
            {
                path = Path.Combine(path, requiredDirectories[i][j]);
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log($"Created directory: {path}");
            }
        }
    }
    public static void DirectoryCheckChunk(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
    }
    public static bool DirectoryCheckGlobal()
    {
        if(!File.Exists(Path.Combine(Application.persistentDataPath, "World", "SaveData.json")))
        {
            File.Create(Path.Combine(Application.persistentDataPath, "World", "SaveData.json"));
            return false;
        }
        return true;
    }
}
