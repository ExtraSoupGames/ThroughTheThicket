using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
public class Path
{
    public List<TileTravelValue> path;
    public Path(List<TileTravelValue> path)
    {
        this.path = path;
    }
}
public class TileTravelValue
{
    public int X;
    public int Y;
    public int TravelCost;
    public TileTravelValue(int X, int Y, int TileTravelCost)
    {
        this.X = X;
        this.Y = Y;
        TravelCost = TileTravelCost;
    }
}
public struct PathContext
{
    public List<TileTravelValue> surroundingTileCosts;
    public PathContext(List<TileTravelValue> tileCosts)
    {
        surroundingTileCosts = tileCosts;
    }
    public override string ToString()
    {
        string output = "";
        foreach(TileTravelValue tile in surroundingTileCosts)
        {
            output = output + "Tile at: " + tile.X.ToString() + " - " + tile.Y.ToString() + " ";
        }
        return output;
    }
}
public class Pathfinder : MonoBehaviour
{
    [SerializeField] private ChunkManager chunkManager;
    private PathContext? currentContext;
    public void Awake()
    {
        currentContext = null;
    }
    public Path FindPath(Vector2Int start, Vector2Int end)
    {
        if (currentContext == null)
        {
            RefreshContext();
        }
        RefreshContext();
        // Priority queue for A*
        PriorityQueue<TileTravelValue> openSet = new PriorityQueue<TileTravelValue>();
        Dictionary<TileTravelValue, int> gCosts = new Dictionary<TileTravelValue, int>();
        Dictionary<TileTravelValue, TileTravelValue> cameFrom = new Dictionary<TileTravelValue, TileTravelValue>();

        TileTravelValue startTile = FindTileAt(start, currentContext.Value.surroundingTileCosts);
        TileTravelValue endTile = FindTileAt(end, currentContext.Value.surroundingTileCosts);

        openSet.Enqueue(startTile, 0);
        gCosts[startTile] = 0;

        while (openSet.Count > 0)
        {
            TileTravelValue current = openSet.Dequeue();

            if (current.Equals(endTile))
            {
                return ReconstructPath(cameFrom, current);
            }

            foreach (TileTravelValue neighbor in GetNeighbors(current))
            {
                int tentativeGCost = gCosts[current] + neighbor.TravelCost;

                if (!gCosts.ContainsKey(neighbor) || tentativeGCost < gCosts[neighbor])
                {
                    gCosts[neighbor] = tentativeGCost;
                    int fCost = tentativeGCost + Heuristic(neighbor, endTile);
                    openSet.Enqueue(neighbor, fCost);
                    cameFrom[neighbor] = current;
                }
            }
        }

        // If no path found, return null or handle it accordingly
        return null;
    }

    private int Heuristic(TileTravelValue a, TileTravelValue b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y); // Manhattan distance
    }

    private List<TileTravelValue> GetNeighbors(TileTravelValue tile)
    {
        List<TileTravelValue> neighbors = new List<TileTravelValue>
        {
            FindTileAt(new Vector2Int(tile.X + 1, tile.Y), currentContext.Value.surroundingTileCosts),
            FindTileAt(new Vector2Int(tile.X - 1, tile.Y), currentContext.Value.surroundingTileCosts),
            FindTileAt(new Vector2Int(tile.X, tile.Y + 1), currentContext.Value.surroundingTileCosts),
            FindTileAt(new Vector2Int(tile.X, tile.Y - 1), currentContext.Value.surroundingTileCosts)
        };

        return neighbors.FindAll(n => n != null); // Remove null tiles
    }

    private Path ReconstructPath(Dictionary<TileTravelValue, TileTravelValue> cameFrom, TileTravelValue current)
    {
        List<TileTravelValue> path = new List<TileTravelValue> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();
        return new Path(path);
    }
    private TileTravelValue FindTileAt(Vector2Int location, List<TileTravelValue> tilesToSearch)
    {
        if (tilesToSearch == null)
        {
            throw new ArgumentNullException(nameof(tilesToSearch), "Tile list cannot be null");
        }

        TileTravelValue tile = tilesToSearch.Find(t => t.X == location.x && t.Y == location.y);

        if (tile == null)
        {
            Debug.Log($"Tile at {location} not found in path context.");
        }

        return tile;
    }
    public void RefreshContext()
    {
        currentContext = chunkManager.GrabPathContext();
    }
}
public class PriorityQueue<T>
{
    private List<(T item, int priority)> heap = new List<(T, int)>();

    public int Count => heap.Count;

    public void Enqueue(T item, int priority)
    {
        heap.Add((item, priority));
        int i = heap.Count - 1;
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (heap[i].priority >= heap[parent].priority) break;
            (heap[i], heap[parent]) = (heap[parent], heap[i]); // Swap
            i = parent;
        }
    }

    public T Dequeue()
    {
        if (heap.Count == 0) throw new InvalidOperationException("Priority queue is empty.");

        T result = heap[0].item;
        heap[0] = heap[^1]; // Move last element to root
        heap.RemoveAt(heap.Count - 1);

        int i = 0;
        while (true)
        {
            int left = 2 * i + 1, right = 2 * i + 2, smallest = i;
            if (left < heap.Count && heap[left].priority < heap[smallest].priority) smallest = left;
            if (right < heap.Count && heap[right].priority < heap[smallest].priority) smallest = right;
            if (smallest == i) break;
            (heap[i], heap[smallest]) = (heap[smallest], heap[i]); // Swap
            i = smallest;
        }

        return result;
    }
}