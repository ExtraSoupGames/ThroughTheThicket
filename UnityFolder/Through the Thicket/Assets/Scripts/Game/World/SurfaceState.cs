using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceState : IWorldState
{
    [SerializeField] private Transform player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TileSelector tileSelector;
    [SerializeField] private Pathfinder pathFinder;
    [SerializeField] private SurfaceManager surfaceManager;
    private Vector3 playerPosition;
    private Quaternion playerRotation;
    private GameManager gameManager;
    public override void Close()
    {
        playerPosition = playerController.transform.position;
        playerRotation = playerController.transform.rotation;
        surfaceManager.HideWorld();
        base.Close();
    }

    public override void Initialize(GameManager manager)
    {
        surfaceManager.HideWorld();
        gameManager = manager;
    }

    public override void Open()
    {
        playerController.SetToWorld(tileSelector, pathFinder);
        surfaceManager.ShowWorld();
        player.transform.SetParent(this.transform);
        player.transform.SetPositionAndRotation(playerPosition, playerRotation);
        base.Open();
    }

    public override void Pause()
    {
        base.Pause();
    }

    public override void Play()
    {
        surfaceManager.RefreshTilePool();
        base.Play();
    }

    public override void SetTile(int x, int y, Layers layer, ITileSegment segment)
    {
        surfaceManager.SetTile(x, y, layer, segment);
    }

    public override void TakeInput(Inputs input)
    {
        if (input == Inputs.Debug1)
        {
            gameManager.OpenState("Dungeon");
        }
        if (input == Inputs.UIToggle)
        {
            gameManager.OpenState("Inventory");
        }
        if (input == Inputs.Debug2)
        {
            gameManager.OpenState("Combat");
        }
        if (input == Inputs.LClick)
        {
            tileSelector.LClick();
        }
        if(input == Inputs.RClick)
        {
            tileSelector.RClick();
        }
    }

    public override void UpdateWhenOpen()
    {
        surfaceManager.QueueManage();
        tileSelector.UpdateWhenOpen();
    }

}
