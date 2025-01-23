using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
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
    private List<Biome> activeBiomes;
    //the range in which biomes are loaded around the player
    private int biomeRange;
    //the maximum distance from a biome's centre where a tile can be
    private int biomeSize;
    public WorldLoading() {
    }
    //initialize world loader variables
    public void InitializeLoader()
    {
        activeBiomes = new List<Biome>();
        biomeRange = 10;
        biomeSize = 7;
        InitializeTilePool();
    }
    public void Testing()
    {
        for(int i = 0;i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Debug.Log("Generating biome at: " + i + " - " + j);
                Biome newBiome = new Biome(i * 10, j * 10);
                SaveBiome(newBiome);
            }
        }
        WorldUpdate();
    }
    //do a full biome update remove unused tiles, update new ones
    public void WorldUpdate()
    {
        AddNeededBiomes();
        TrimLoadedBiomes();
        UpdateTilePool();
    }
    #region tileManagement
    //create game objects for tiles for all loaded biomes
    private void InitializeTilePool()
    {
        //find area of circle and add a few to be safe
        tilePoolSize = (int)(((float)biomeRange * (float)biomeRange * Mathf.PI) + 5);
        Debug.Log("Tile pool size: " + tilePoolSize);
        tilePool = new List<GameObject>();
        for (int i = 0; i < tilePoolSize; i++)
        {
            GameObject newTile = Instantiate(TilePrefab, TileParent.transform);
            tilePool.Add(newTile);
        }
    }
    //update the tile pool to display tiles from active biomes that are in range
    public void UpdateTilePool()
    {
        int PlayerX = (int)player.position.x;
        int PlayerY = (int)player.position.z;
        List<Tile> requiredTiles = new List<Tile>();
        foreach (Biome biome in activeBiomes)
        {
            requiredTiles.AddRange(biome.GetActiveTiles(PlayerX, PlayerY, biomeRange));
        }
        for(int tileIndex = 0;tileIndex < tilePoolSize; tileIndex++)
        {
            if(requiredTiles.Count <= tileIndex)
            {
                tilePool[tileIndex].SetActive(false);
            }
            else
            {
                tilePool[tileIndex].SetActive(true);
                requiredTiles[tileIndex].ApplyTileProperties(tilePool[tileIndex]);
            }
        }
        if(tilePoolSize < requiredTiles.Count)
        {
            Debug.Log("Insufficient tile pool size to display loaded biome tiles");
        }
    }
    #endregion tileManagement
    #region activeBiomeCleaning
    //load and generate any biomes that are needed to be shown
    private void AddNeededBiomes()
    {
        int PlayerX = (int)player.position.x;
        int PlayerY = (int)player.position.y;
        string[] allBiomes = Directory.GetFiles(Application.persistentDataPath + "/biomes");
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
            if (activeBiomes.FindIndex(b => b.CentreX == biomeX && b.CentreY == biomeY) != -1)
            {
                continue;
            }
            activeBiomes.Add(LoadBiome(biomeX, biomeY));


        }
        activeBiomes.RemoveAll(biome => biome.MarkedForDeletion);
    }
    //unload and save biomes that are too far away to be needed right now
    private void TrimLoadedBiomes()
    {
        int PlayerX = (int)player.position.x;
        int PlayerY = (int)player.position.y;
        foreach(Biome loadedBiome in activeBiomes)
        {
            if(!IsInRange(loadedBiome.CentreX, loadedBiome.CentreY, PlayerX, PlayerY, biomeSize + biomeRange))
            {
                loadedBiome.MarkedForDeletion = true;
            }
        }
        activeBiomes.RemoveAll(biome => biome.MarkedForDeletion);
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
    private Biome LoadBiome(int X, int Y)
    {
        if (!File.Exists(Application.persistentDataPath + "/biomes/biome" + X + "-" + Y + ".json"))
        {
            Debug.Log("Invalid biome attempted to be loaded");
            return null;
        }
        string biomeAsJSON = File.ReadAllText(Application.persistentDataPath + "/biomes/biome" + X + "-" + Y + ".json");
        Biome biome = JsonUtility.FromJson<Biome>(biomeAsJSON);
        return biome;
    }
    #endregion fileInteraction
    #region helpers
    private bool IsInRange(int biomeCentreX, int biomeCentreY, int TileX, int TileY, int Range)
    {
        float distance = MathsHelper.FindDistance(biomeCentreX, biomeCentreY, TileX, TileY);
        return distance < Range;
    }
    #endregion helpers
}
