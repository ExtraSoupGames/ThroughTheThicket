using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonState : IWorldState
{
    [SerializeField] private Transform player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TileSelector tileSelector;
    [SerializeField] private Pathfinder pathFinder;
    [SerializeField] private DungeonManager dungeonManager;
    private GameManager gameManager;
    public override void Close()
    {
        dungeonManager.HideWorld();
        base.Close();
    }

    public override void Initialize(GameManager manager)
    {
        dungeonManager.OtherTests();
        dungeonManager.HideWorld();
        gameManager = manager;
    }

    public override void Open()
    {
        playerController.SetToWorld(tileSelector, pathFinder);
        dungeonManager.ShowWorld();
        player.transform.SetParent(this.transform);
        base.Open();
    }

    public override void Pause()
    {
        base.Pause();
    }

    public override void Play()
    {
        dungeonManager.RefreshTilePool();
        base.Play();
    }

    public override void SetTile(int x, int y, Layers layer, ITileSegment segment)
    {
        dungeonManager.SetTile(x, y, layer, segment);
    }

    public override void TakeInput(Inputs input)
    {
        if (input == Inputs.Debug1)
        {
            gameManager.CloseState("Dungeon");
        }
        if(input == Inputs.Debug2)
        {
            gameManager.OpenState("Combat");
        }
        if (input == Inputs.UIToggle)
        {
            gameManager.OpenState("Inventory");
        }
        if (input == Inputs.LClick)
        {
            tileSelector.LClick();
        }
        if (input == Inputs.RClick)
        {
            tileSelector.RClick();
        }
    }


    public override void UpdateWhenOpen()
    {
        dungeonManager.QueueManage();
    }
}
