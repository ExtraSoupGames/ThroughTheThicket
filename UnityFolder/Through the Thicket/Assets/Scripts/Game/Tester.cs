using System.Security.Cryptography;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public ChunkManager chunkManager;
    public UIRenderer inventoryRenderer;

    void Start()
    {
        chunkManager.Tests();

        UIHolder holder = new UIHolder();
        inventoryRenderer.ApplyUIHolder(holder);
        inventoryRenderer.StartManipulator();
        inventoryRenderer.Refresh();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        chunkManager.QueueManage();
    }
}
