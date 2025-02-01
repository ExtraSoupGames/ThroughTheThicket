using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public ChunkManager chunkManager;
    // Start is called before the first frame update
    void Start()
    {
        chunkManager.Tests();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        chunkManager.QueueManage();
    }
}
