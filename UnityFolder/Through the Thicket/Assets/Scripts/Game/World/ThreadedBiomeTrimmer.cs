using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
public struct BiomeData
{
    public int X;
    public int Y;
    public bool initialized;
}
public struct ThreadedBiomeTrimmer : IJob
{
    public NativeArray<BiomeData> activeBiomes;
    public int PlayerX, PlayerY;
    public int BiomeSize, BiomeRange;
    public NativeArray<char> persistentDataPath;
    public void Execute()
    {

        FindNeededBiomes(PlayerX, PlayerY, BiomeSize, BiomeRange);
    }
    //load and generate any biomes that are needed to be shown
    private void FindNeededBiomes(int PlayerX, int PlayerY, int biomeSize, int biomeRange)
    {
        int biomeIndex = 0;
        string dataPathString = new string(persistentDataPath.ToArray());
        string[] allBiomes = Directory.GetFiles(dataPathString + "/biomes");
        foreach (string biome in allBiomes)
        {
            //removes the path from the filename
            string biomeLocation = biome.Substring(biome.IndexOf("biomes\\biome") + 12);
            //split the coordinates into an array of strings, and remove the .json from the end
            string[] biomeCoordinates = biomeLocation.Substring(0, biomeLocation.IndexOf(".")).Split("-");
            int biomeX = int.Parse(biomeCoordinates[0]);
            int biomeY = int.Parse(biomeCoordinates[1]);
            //check if is in range, add a few to prevent any tiles not appearing if the biome is juuuuussssttt out of range
            if (!IsInRange(biomeX, biomeY, PlayerX, PlayerY, biomeSize + biomeRange + 1))
            {
                continue;
            }
            activeBiomes[biomeIndex] = new BiomeData { X = biomeX, Y = biomeY, initialized = true };
            biomeIndex++;
        }
    }
    private bool IsInRange(int biomeCentreX, int biomeCentreY, int TileX, int TileY, int Range)
    {
        float distance = MathsHelper.FindDistance(biomeCentreX, biomeCentreY, TileX, TileY);
        return distance < Range;
    }
}
