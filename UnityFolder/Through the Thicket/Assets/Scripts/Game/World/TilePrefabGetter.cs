using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Contains all the data needed to display a layer of a tile
public class LayerDisplayProperties
{
    public bool isEmpty;
    public Mesh LayerMesh;
    public Material[] LayerMats;
    public LayerDisplayProperties()
    {
        isEmpty = true;

    }
    //TODO make these all switch based on the type provided and fetch the required mesh and materials
    public LayerDisplayProperties(string meshName, string[] materialNames)
    {
        isEmpty = false;
        LayerMesh = Resources.Load<Mesh>("Meshes/" + meshName);
        int materialCount = materialNames.Length;
        LayerMats = new Material[materialCount];
        for(int i = 0; i < materialCount; i++)
        {
            LayerMats[i] = (Resources.Load<Material>("Materials/" + materialNames[i]));
        }
    }
}
public class TileDisplayGetter
{
    public Dictionary<string, LayerDisplayProperties> allMeshes;
    public TileDisplayGetter()
    {
        allMeshes = new Dictionary<string, LayerDisplayProperties>();
        allMeshes.Add("Empty", new LayerDisplayProperties());
        allMeshes.Add("Grass", new LayerDisplayProperties("Grass", new string[] { "Foliage1"}));
        allMeshes.Add("Stone", new LayerDisplayProperties("Grass", new string[] { "Foliage2"}));
        allMeshes.Add("Foliage", new LayerDisplayProperties("Foliage", new string[] { "Foliage1", "Foliage2", "Foliage3"}));
        allMeshes.Add("Potato", new LayerDisplayProperties("Potato", new string[] { "Potato"}));
        allMeshes.Add("Carrot", new LayerDisplayProperties("Carrot", new string[] { "Carrot"}));
        allMeshes.Add("Redcap", new LayerDisplayProperties("Redcap", new string[] { "Redcap1", "Redcap2"}));
        allMeshes.Add("Pebble", new LayerDisplayProperties("Pebble", new string[] { "Pebble"}));
        allMeshes.Add("River", new LayerDisplayProperties("Grass", new string[] { "River" }));
    }
    public LayerDisplayProperties GetDisplayProperties(string layerName)
    {
        return allMeshes[layerName];
    }
}
