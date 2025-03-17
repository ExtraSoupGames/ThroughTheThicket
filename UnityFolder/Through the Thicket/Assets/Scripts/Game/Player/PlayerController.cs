using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private TileSelector tileSelector;
    private Pathfinder pathFinder;
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
    //the player animator
    [SerializeField] private PlayerAnimator playerAnimator;
    public void Initialize(GameManager manager)
    {
        gameManager = manager;
        moveTimer = 0;
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
    public void SetToWorld(TileSelector tileSelector, Pathfinder pathFinder)
    {
        this.tileSelector = tileSelector;
        this.pathFinder = pathFinder;
    }
    public void FixedUpdate()
    {
        TryMoveAlongPath();
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


            playerAnimator.WalkAnimation(tileDirection);

        }
        playerMovementTransform.position = tilePos;
    }
}
