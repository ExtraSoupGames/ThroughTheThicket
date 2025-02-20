using UnityEditor;
using UnityEngine;

public class MeshExtractor
{
    [MenuItem("Tools/Extract Meshes from FBX")]
    static void ExtractMeshes()
    {
        string folderPath = "Assets/ExtractedMeshes"; // Change this to your desired folder
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "ExtractedMeshes");
        }

        foreach (Object obj in Selection.objects)
        {
            if (obj is GameObject)
            {
                MeshFilter[] meshFilters = ((GameObject)obj).GetComponentsInChildren<MeshFilter>();
                foreach (MeshFilter mf in meshFilters)
                {
                    Mesh mesh = mf.sharedMesh;
                    if (mesh != null)
                    {
                        string path = $"{folderPath}/{mesh.name}.asset";
                        AssetDatabase.CreateAsset(Object.Instantiate(mesh), path);
                        Debug.Log($"Mesh {mesh.name} extracted to {path}");
                    }
                }
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
