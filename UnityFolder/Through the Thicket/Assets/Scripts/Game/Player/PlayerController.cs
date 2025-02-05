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
    private Path path;
    public void Awake()
    {
        moveTimer = 0;
        pathIndex = 0;
        pathLength = 0;
    }
    public void StartMovingPlayer()
    {
        if(pathIndex < pathLength)
        {
            //dont move if moving
            return;
        }
        Vector2Int playerPosition = new Vector2Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.z);
        Vector2Int targetPosition = new Vector2Int(tileSelector.GetSelectedTile().X, tileSelector.GetSelectedTile().Y);
        path = pathFinder.FindPath(playerPosition, targetPosition);
        pathIndex = 0;
        moveTimer = 0.5f;
        pathLength = path.path.Count;
    }
    public void FixedUpdate()
    {
        if (pathIndex >= pathLength)
        {
            if(path != null)
            {
                gameObject.transform.position = new Vector3(path.path[pathLength - 1].X, 1, path.path[pathLength - 1].Y);
            }
            return;
        }
        moveTimer -= Time.fixedDeltaTime;
        if (moveTimer > 0)
        {
            return;
        }
        moveTimer += 0.1f;
        gameObject.transform.position = new Vector3(path.path[pathIndex].X, 1, path.path[pathIndex].Y);
        pathIndex++;
    }
}
