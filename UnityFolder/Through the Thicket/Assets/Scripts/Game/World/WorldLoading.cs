using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class WorldLoading : MonoBehaviour
{
    //all loaded biomes
    private List<Biome> _loadedBiomes;
    public WorldLoading() {
    }
    public void Testing()
    {
        Biome biome = new Biome();
        SaveBiome(biome);
    }
    //load and generate any biomes that are needed to be shown
    private void AddNeededBiomes()
    {

    }
    //unload and save biomes that are too far away to be needed right now
    private void TrimLoadedBiomes()
    {

    }
    //save a biome
    private void SaveBiome(Biome biome)
    {
        string biomeAsJSON = JsonUtility.ToJson(biome.Data);
        File.WriteAllText(Application.persistentDataPath + "/biome.json", biomeAsJSON);
    }
    private Biome LoadBiome()
    {
        string biomeAsJSON = File.ReadAllText(Application.persistentDataPath + "/biome.json");
        Biome biome = JsonUtility.FromJson<Biome>(biomeAsJSON);
        return biome;
    }
}
