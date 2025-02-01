using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileHelper 
{
    //ensure all required directories exists
    public static void DirectoryCheck()
    {
        string[] requiredDirectories = { "chunks" };
        foreach (string dir in requiredDirectories)
        {
            if(!Directory.Exists(Application.persistentDataPath + "/" + dir))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + dir);
            }
        }
    }
}
