using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public struct ThreadedTileGetter : IJob
{
    public NativeArray<Tile> tiles;
    public int PlayerX, PlayerY;
    public int BiomeRange;
    public int BiomeSize;
    public int TilePoolSize;
    public NativeArray<BiomeData> activeBiomes;
    public NativeArray<char> persistentDataPath;
    public void Execute()
    {
        int tileIndex = 0;
        for (int i = 0; i < activeBiomes.Length; i++)
        {
            if (activeBiomes[i].initialized == false)
            {
                break;
            }
            Biome loadingBiome = LoadBiome(activeBiomes[i].X, activeBiomes[i].Y);
            List<Tile> newTiles = (loadingBiome.GetActiveTiles(PlayerX, PlayerY, BiomeRange));
            foreach(Tile t in newTiles)
            {
                if(tileIndex < tiles.Length)
                {
                    tiles[tileIndex] = t;
                    tileIndex++;
                }
                else
                {
                    break;
                }
            }
        }
    }
    private Biome LoadBiome(int X, int Y)
    {
        string dataPathString = new string(persistentDataPath.ToArray());
        if (!File.Exists(dataPathString + "/biomes/biome" + X + "-" + Y + ".json"))
        {
            UnityEngine.Debug.Log("Invalid biome attempted to be loaded");
            throw new Exception("Invalid biome load attempt made");
        }
        string biomeAsJSON = File.ReadAllText(dataPathString + "/biomes/biome" + X + "-" + Y + ".json");
        Biome biome = JsonUtility.FromJson<Biome>(biomeAsJSON);
        return biome;
    }
}
