using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private TileSelector tileSelector;
    [SerializeField] private Pathfinder pathFinder;
    private int pathIndex;
    private int pathLength;
    private float moveTimer;
    private Queue<ProcessedTileData> path;
    private bool takingInput;
    private GameManager gameManager;
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
        gameObject.transform.position = new Vector3(travelToTile.X, travelToTile.Height, travelToTile.Y);
    }
    public void OpenInventoryPressed()
    {
        if(IsTakingInput())
        gameManager.OpenInventory();
    }
    public void SetIsTakingInput(bool value)
    {
        takingInput = value;
    }
    public bool IsTakingInput()
    {
        return takingInput;
    }
}
