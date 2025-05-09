using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : MonoBehaviour { 
    public void EnableFogCave()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.gray;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 10;
        RenderSettings.fogEndDistance = 15;
        Debug.Log("Fog enabled for cave");
    }
    public void EnableFogSurface()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.gray;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 10;
        RenderSettings.fogEndDistance = 15;
        Debug.Log("Fog enable for surface");
    }
    public void DisableFog()
    {
        RenderSettings.fog = false;
    }
}