using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceState : IWorldState
{
    [SerializeField] private Transform player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TileSelector tileSelector;
    [SerializeField] private Pathfinder pathFinder;
    [SerializeField] private ChunkManager surfaceManager;
    public override void Close()
    {
        surfaceManager.HideWorld();
        base.Close();
    }

    public override void Initialize(GameManager manager)
    {
        surfaceManager.Tests();
        surfaceManager.HideWorld();
    }

    public override void Open()
    {
        playerController.SetToWorld(tileSelector, pathFinder);
        surfaceManager.ShowWorld();
        player.transform.SetParent(this.transform);
        base.Open();
    }

    public override void Pause()
    {
        playerController.SetTakingInput(false);
        base.Pause();
    }

    public override void Play()
    {
        playerController.SetTakingInput(true);
        base.Play();
    }

    public override void TakeInput(int input)
    {
        if (input == 1)
        {
            playerController.DebugPressedDungeon();
        }
        if (input == 2)
        {
            playerController.OpenInventoryPressed();
        }
    }

    public override void UpdateWhenOpen()
    {
        surfaceManager.QueueManage();
    }

}
