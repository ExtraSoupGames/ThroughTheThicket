using System.Security.Cryptography;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public ChunkManager chunkManager;

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
