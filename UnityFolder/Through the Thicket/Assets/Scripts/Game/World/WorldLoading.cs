using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using System;
using Unity.Collections;
using static UnityEditor.Experimental.GraphView.GraphView;
using Unity.Jobs;
using System.Diagnostics;
public class WorldLoading : MonoBehaviour
{
    //reference to player for coordinates
    public Transform player;
    //amount of tiles needed in tile pool to display all tiles in biome distance
    private int tilePoolSize;
    //Prefab of empty tile to generate
    public GameObject TilePrefab;
    //list of gameobjects instantiated when the world is loaded
    private List<GameObject> tilePool;
    //parent object for all tiles to be parented to
    [SerializeField] private GameObject TileParent;
    //all loaded biomes
    private NativeArray<BiomeData> loadedBiomes;
    //the range in which biomes are loaded around the player
    private int biomeRange;
    //the maximum distance from a biome's centre where a tile can be
    private int biomeSize;
    //the space inbetween biomes
    private int biomeSpacing;
    //the length of memory to dedicate to store loaded biomes, I.E. the max amount of biomes loaded at once
    private int loadedBiomeCap;
    //native array reference for the tiles generated each frame
    private NativeArray<Tile> tiles;
    //persisten data path to pass into jobs to allow file manipulation
    private NativeArray<char> persistentDataPath;
    //reference to keep track of currently running tile loader job
    private JobHandle tileJob;
    public WorldLoading() {
    }
    public void OnDestroy()
    {
        persistentDataPath.Dispose();
    }
    //initialize world loader variables
    public void InitializeLoader()
    {
        persistentDataPath = new NativeArray<char>(Application.persistentDataPath.ToCharArray(), Allocator.Persistent);
        biomeRange = 50;
        biomeSize = 10;
        biomeSpacing = 2 * biomeSize;
        UnityEngine.Debug.Log(biomeSpacing + " is the biomeSpacing");
        int loadedBiomeCapSqrt = Mathf.CeilToInt((biomeRange * 2 + biomeSpacing)  /  (biomeSpacing));
        loadedBiomeCap = (loadedBiomeCapSqrt * loadedBiomeCapSqrt);
        UnityEngine.Debug.Log("loadedBiomeCap is: " + loadedBiomeCap);

        InitializeTilePool();
    }
    public void CreateTestingWorld()
    {
        DeleteAllBiomes();
        UnityEngine.Debug.Log("biomeSpacing: " + biomeSpacing);
        int biomeCount = 10;
        for(int i = 0;i < biomeCount; i++)
        {
            for (int j = 0; j < biomeCount; j++)
            {
                UnityEngine.Debug.Log("Generating biome at: " + i * biomeSpacing + " - " + j * biomeSpacing);
                Biome newBiome = new Biome(i * biomeSpacing, j * biomeSpacing, biomeSpacing);
                SaveBiome(newBiome);
            }
        }
        WorldUpdateBegin();
        WorldUpdateEnd();
    }
    //do a full biome update remove unused tiles, update new ones
    public void WorldUpdateBegin()
    {
        JobHandle biomeJob = StartBiomeLoading();
        biomeJob.Complete();
        tileJob = StartUpdatingTiles();
    }
    public void WorldUpdateEnd()
    {
        UpdateTilePool(tileJob);
        loadedBiomes.Dispose();
    }
    #region tileManagement
    //create game objects for tiles for all loaded biomes
    private void InitializeTilePool()
    {
        //find area of circle and add a few to be safe
        tilePoolSize = (int)(((float)biomeRange * (float)biomeRange * Mathf.PI) + 5);
        UnityEngine.Debug.Log("Tile pool size: " + tilePoolSize);
        tilePool = new List<GameObject>();
        for (int i = 0; i < tilePoolSize; i++)
        {
            GameObject newTile = Instantiate(TilePrefab, TileParent.transform);
            tilePool.Add(newTile);
        }
    }
    private JobHandle StartUpdatingTiles()
    {
        int PlayerX = (int)player.position.x;
        int PlayerY = (int)player.position.z;
        tiles = new NativeArray<Tile>(tilePoolSize, Allocator.TempJob);
        ThreadedTileGetter tileGetter = new ThreadedTileGetter
        {
            tiles = tiles,
            PlayerX = PlayerX,
            PlayerY = PlayerY,
            BiomeRange = biomeRange,
            activeBiomes = loadedBiomes,
            persistentDataPath = persistentDataPath
        };
        return tileGetter.Schedule();
    }
    //update the tile pool to display tiles from active biomes that are in range
    private void UpdateTilePool(JobHandle tileGetterJob)
    {
        tileGetterJob.Complete();
        for(int tileIndex = 0;tileIndex < tilePoolSize; tileIndex++)
        {
            tiles[tileIndex].ApplyTileProperties(tilePool[tileIndex]);
        }
        tiles.Dispose();
    }
    #endregion tileManagement
    #region activeBiomeCleaning
    private JobHandle StartBiomeLoading()
    {
        int PlayerX = (int)player.position.x;
        int PlayerY = (int)player.position.z;
        loadedBiomes = new NativeArray<BiomeData>(loadedBiomeCap, Allocator.TempJob);
        ThreadedBiomeTrimmer biomeJob = new ThreadedBiomeTrimmer
        {
            PlayerX = PlayerX,
            PlayerY = PlayerY,
            BiomeSize = biomeSize,
            BiomeRange = biomeRange,
            activeBiomes = loadedBiomes,
            persistentDataPath = persistentDataPath
        };
        return biomeJob.Schedule();
    }
    #endregion activeBiomeCleaning
    #region fileInteraction
    //save a biome
    private void SaveBiome(Biome biome)
    {
        string biomeAsJSON = JsonUtility.ToJson(biome);
        string fileName = "/biomes/biome" + biome.CentreX + "-" + biome.CentreY;
        File.WriteAllText(Application.persistentDataPath + fileName + ".json", biomeAsJSON);
    }
    void DeleteAllBiomes()
    {
        string path = Application.persistentDataPath + "/biomes";
        // Delete all files
        string[] files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            File.Delete(file);
        }
    }
    #endregion fileInteraction
}
