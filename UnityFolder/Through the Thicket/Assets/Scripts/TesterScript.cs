using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TesterScript : MonoBehaviour
{
    public WorldLoading worldLoading;
    // Start is called before the first frame update
    void Start()
    {
        worldLoading.InitializeLoader();
        worldLoading.Testing();
    }

    // Update is called once per frame
    void Update()
    {
        worldLoading.WorldUpdate();
    }
}
