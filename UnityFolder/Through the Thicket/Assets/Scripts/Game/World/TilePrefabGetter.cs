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
    public LayerDisplayProperties(BaseTypes baseType)
    {
        isEmpty = false;
        LayerMesh = Resources.Load<Mesh>("Meshes/Grass");
        LayerMats = new Material[1];
        LayerMats[0] = (Resources.Load<Material>("Materials/Foliage1"));
    }
    public LayerDisplayProperties(FoliageTypes foliageType)
    {
        isEmpty = false;
        LayerMesh = Resources.Load<Mesh>("Meshes/Foliage");
        LayerMats = new Material[3];
        LayerMats[0] = (Resources.Load<Material>("Materials/Foliage1"));
        LayerMats[1] = (Resources.Load<Material>("Materials/Foliage2"));
        LayerMats[2] = (Resources.Load<Material>("Materials/Foliage3"));
    }
    public LayerDisplayProperties(ObjectTypes objectType)
    {
        isEmpty = false;
        //TODO Edit
        LayerMesh = Resources.Load<Mesh>("Meshes/Foliage");
        LayerMats = new Material[3];
        LayerMats[0] = (Resources.Load<Material>("Materials/Foliage1"));
        LayerMats[1] = (Resources.Load<Material>("Materials/Foliage2"));
        LayerMats[2] = (Resources.Load<Material>("Materials/Foliage3"));
    }
}
public class TileDisplayGetter
{
    public Dictionary<BaseTypes, LayerDisplayProperties> baseMeshes;
    public Dictionary<FoliageTypes, LayerDisplayProperties> foliageMeshes;
    public Dictionary<ObjectTypes, LayerDisplayProperties> objectMeshes;
    public TileDisplayGetter()
    {
        baseMeshes = new Dictionary<BaseTypes, LayerDisplayProperties>();
        baseMeshes.Add(BaseTypes.None, new LayerDisplayProperties());
        baseMeshes.Add(BaseTypes.Grass, new LayerDisplayProperties(BaseTypes.Grass));
        baseMeshes.Add(BaseTypes.Stone, new LayerDisplayProperties(BaseTypes.Stone));
        foliageMeshes = new Dictionary<FoliageTypes, LayerDisplayProperties>();
        foliageMeshes.Add(FoliageTypes.None, new LayerDisplayProperties());
        foliageMeshes.Add(FoliageTypes.DefaultFoliage, new LayerDisplayProperties(FoliageTypes.DefaultFoliage));
        objectMeshes = new Dictionary<ObjectTypes, LayerDisplayProperties>();
        objectMeshes.Add(ObjectTypes.None, new LayerDisplayProperties());
    }
    public LayerDisplayProperties GetDisplayProperties(Layers layer, int typeInLayer)
    {
        LayerDisplayProperties returnVal;
        switch (layer)
        {
            case Layers.Base:
                if(!baseMeshes.TryGetValue((BaseTypes)(typeInLayer), out returnVal))
                {
                    Debug.Log("Error Getting base Display Properties");
                }
                break;
            case Layers.Foliage:
                if (!foliageMeshes.TryGetValue((FoliageTypes)(typeInLayer), out returnVal))
                {
                    Debug.Log("Error Getting foliage Display Properties");
                }
                break;
            case Layers.Object:
                if (!objectMeshes.TryGetValue((ObjectTypes)(typeInLayer), out returnVal))
                {
                    Debug.Log("Error Getting object Display Properties");
                }
                break;
            default:
                returnVal = null;
                break;

        }
        return returnVal;
    }
}
