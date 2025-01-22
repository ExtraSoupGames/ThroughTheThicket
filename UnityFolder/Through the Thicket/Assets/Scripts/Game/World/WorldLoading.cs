using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
public class WorldLoading : MonoBehaviour
{
    //Prefab of empty tile to generate
    public GameObject TilePrefab;
    //list of gameobjects instantiated when the world is loaded
    private List<GameObject> _tileObjects;
    //all loaded biomes
    private List<Biome> _loadedBiomes;
    //the range in which biomes are loaded around the player
    private int biomeRange;
    //the maximum distance from a biome's centre where a tile can be
    private int biomeSize;
    public WorldLoading() {
        _tileObjects = new List<GameObject>();
        _loadedBiomes = new List<Biome>();
        biomeRange = 10;
        biomeSize = 5;
    }
    public void Testing()
    {
        Biome biome = new Biome();
        SaveBiome(biome);
        LoadBiomeAt(0, 5);
        InstantiateTiles();
    }
    //create game objects for tiles for all loaded biomes
    private void InstantiateTiles()
    {
        _tileObjects.Clear();
        foreach(Biome biome in _loadedBiomes)
        {
            foreach(Tile tile in biome.Data.Tiles)
            {
                GameObject newTile = Instantiate(TilePrefab);
                tile.ConfigureTileObject(newTile);
                _tileObjects.Add(newTile);
            }
        }
    }
    //load and generate any biomes that are needed to be shown
    private void AddNeededBiomes(int PlayerX, int PlayerY)
    {
        //TODO search for tiles in range
        //TODO Load required biomes
        for(int xOffset = -biomeRange; xOffset <= biomeRange; xOffset++)
        {
            for (int yOffset = -biomeRange; yOffset <= biomeRange; yOffset++)
            {
                //TODO Check if tile is in loaded biomes
                //TODO if not then call LoadBiomeAt()
                //check if tile is loaded at X: PlayerX + xOffset and Y: PlayerY + yOffset
            }
        }
    }
    private void LoadBiomeAt(int X, int Y)
    {
        string[] allBiomes = Directory.GetFiles(Application.persistentDataPath + "/biomes");
        bool foundBiome = false;
        foreach(string biome in allBiomes)
        {
            //removes the path from the filename
            string biomeLocation = biome.Substring(biome.IndexOf("biomes\\biome") + 12);
            //split the coordinates into an array of strings, and remove the .json from the end
            string[] biomeCoordinates = biomeLocation.Substring(0, biomeLocation.IndexOf(".")).Split("-");
            int biomeX = int.Parse(biomeCoordinates[0]);
            int biomeY = int.Parse(biomeCoordinates[1]);
            if (IsInRange(biomeX, biomeY, X, Y))
            {
                Biome biomeCandidate = LoadBiome(biomeX, biomeY);
                if(biomeCandidate.ContainsTile(X, Y))
                {
                    foundBiome = true;
                    _loadedBiomes.Add(biomeCandidate);
                    break;
                }
            }
        }
        if (!foundBiome)
        {
            //TODO generate new biome that contains tile at X Y
            Debug.Log("Unloaded world region reached, world generation not yet implemented");
        }
    }
    private bool IsInRange(int biomeCentreX, int biomeCentreY, int TileX, int TileY)
    {
        float distance = MathsHelper.FindDistance(biomeCentreX, biomeCentreY, TileX, TileY);
        return distance < biomeSize + biomeRange;
    }
    //unload and save biomes that are too far away to be needed right now
    private void TrimLoadedBiomes(int playerX, int playerY)
    {
        foreach(Biome loadedBiome in _loadedBiomes)
        {
            if(MathsHelper.FindDistance(loadedBiome.Data.CentreX, loadedBiome.Data.CentreY, playerX, playerY) < biomeRange)
            {
                loadedBiome.MarkedForDeletion = true;
            }
        }
        _loadedBiomes.RemoveAll(biome => biome.MarkedForDeletion);
    }
    //save a biome
    private void SaveBiome(Biome biome)
    {
        string biomeAsJSON = JsonUtility.ToJson(biome.Data);
        string fileName = "/biomes/biome" + biome.Data.CentreX + "-" + biome.Data.CentreY;
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
}
