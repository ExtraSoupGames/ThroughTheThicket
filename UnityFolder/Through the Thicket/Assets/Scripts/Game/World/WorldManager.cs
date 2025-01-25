using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public WorldLoading worldLoading;
    public Transform playerTransform;
    private Vector3 LastPlayerPos;
    [SerializeField] private int maxReloadDistance;
    // Start is called before the first frame update
    void Start()
    {
        worldLoading.InitializeLoader();
        worldLoading.CreateTestingWorld();
    }

    // Update is called once per frame
    void Update()
    {
        if((LastPlayerPos - playerTransform.position).magnitude > maxReloadDistance)
        {
            UpdateWorld();
        }
    }
    private void UpdateWorld()
    {
        worldLoading.WorldUpdateBegin();
        LastPlayerPos = playerTransform.position;
        worldLoading.WorldUpdateEnd();
    }
    public void ForceWorldUpdate()
    {
        UpdateWorld();
    }
}
