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
        string biomeAsJSON = JsonUtility.ToJson(biome);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", biomeAsJSON);
    }
}
