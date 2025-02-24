using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IGameState
{
    [SerializeField] private TileSelector tileSelector;
    [SerializeField] private Pathfinder pathFinder;
    [SerializeField] private ChunkManager surfaceManager;
    [SerializeField] private DungeonManager dungeonManager;
    private int pathIndex;
    private int pathLength;
    private float moveTimer;
    private Queue<ProcessedTileData> path;
    private bool takingInput;
    private GameManager gameManager;
    //this should be the transform with the whole player hierarchy, camera included
    [SerializeField] private Transform playerMovementTransform;
    //this should include only the player model, not the camera, otherwise entire view will move around like crazy
    [SerializeField] private Transform playerRotationTransform;
    //The chunkManager used when the player is on the surface
    public void Initialize(GameManager manager)
    {
        gameManager = manager;
        moveTimer = 0;
        surfaceManager.Tests();
    }
    public void StartMovingPlayer()
    {
        //empties the path queue if a new movement is selected
        path = new Queue<ProcessedTileData>();
        Vector2Int playerPosition = new Vector2Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.z);
        Vector2Int targetPosition = new Vector2Int(tileSelector.GetSelectedTile().X, tileSelector.GetSelectedTile().Y);
        List<TravelTile> pathFound = pathFinder.FindPath(playerPosition, targetPosition);
        foreach(TravelTile tile in pathFound)
        {
            path.Enqueue(tile.tile);
        }
        moveTimer = 0.5f;
    }
    public void UpdateWhenOpen()
    {
        TryMoveAlongPath();
        surfaceManager.QueueManage();
    }
    private void TryMoveAlongPath()
    {
        //TODO this should be fixed to work with animation system
        moveTimer -= Time.fixedDeltaTime;
        if (moveTimer > 0)
        {
            return;
        }
        moveTimer += 0.3f;
        //if the path queue is empty, then stop
        //TODO make this more readable
        if (path == null)
        {
            return;
        }
        if (path.Count == 0)
        {
            return;
        }
        ProcessedTileData travelToTile = path.Dequeue();
        //TODO get the tile height
        //Moves the player and rotates them
        //TODO animate player
        Vector3 tilePos = new Vector3(travelToTile.X, travelToTile.Height, travelToTile.Y);
        Vector3 tileDirection = tilePos - playerMovementTransform.position;
        tileDirection.y = 0;
        if (tileDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(tileDirection);
            playerRotationTransform.localRotation = targetRotation * Quaternion.Euler(-90, 0, 0);
        }
        playerMovementTransform.position = tilePos;
    }
    public void OpenInventoryPressed()
    {
        if (IsTakingInput())
        {
            gameManager.OpenState("Inventory");
        }
    }
    public void DebugPressed()
    {
        if (IsTakingInput()){
            gameManager.OpenState("Combat");
        }
    }
    public void Open()
    {
        takingInput = true;
    }
    public void Close()
    {
        takingInput = false;
    }
    public bool IsTakingInput()
    {
        return takingInput;
    }
}
