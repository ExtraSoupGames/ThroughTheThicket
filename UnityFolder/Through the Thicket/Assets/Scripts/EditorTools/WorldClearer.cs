using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class WorldClearer : MonoBehaviour
{
    [MenuItem("Tools/ClearWorldFile")]
    static void ClearWorldFile()
    {
        string worldFolderPath = Path.Combine(Application.persistentDataPath, "World");

        if (Directory.Exists(worldFolderPath))
        {
            Directory.Delete(worldFolderPath, true);
            Debug.Log($"Deleted World folder at: {worldFolderPath}");
        }
        else
        {
            Debug.LogWarning($"World folder not found at: {worldFolderPath}");
        }

    }
}
