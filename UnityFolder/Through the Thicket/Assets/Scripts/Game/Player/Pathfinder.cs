using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
//Wrapper to make tile data nullable
public class TravelTile
{
    public ProcessedTileData tile;
    public int X => tile.X;
    public int Y => tile.Y;
    public int TravelCost => tile.TravelCost;
    public TravelTile(ProcessedTileData tile)
    {
        this.tile = tile;
    }

}
public class Pathfinder : MonoBehaviour
{
    [SerializeField] private ChunkManager chunkManager;
    private List<TravelTile> currentContext;
    public void Awake()
    {
        currentContext = null;
    }
    public List<TravelTile> FindPath(Vector2Int start, Vector2Int end)
    {
        if (currentContext == null)
        {
            RefreshContext();
        }
        RefreshContext();
        // Priority queue for A*
        PriorityQueue<TravelTile> openSet = new PriorityQueue<TravelTile>();
        Dictionary<TravelTile, int> gCosts = new Dictionary<TravelTile, int>();
        Dictionary<TravelTile, TravelTile> cameFrom = new Dictionary<TravelTile, TravelTile>();

        TravelTile startTile = FindTileAt(start, currentContext);
        TravelTile endTile = FindTileAt(end, currentContext);

        openSet.Enqueue(startTile, 0);
        gCosts[startTile] = 0;

        while (openSet.Count > 0)
        {
            TravelTile current = openSet.Dequeue();

            if (current.Equals(endTile))
            {
                return ReconstructPath(cameFrom, current);
            }

            foreach (TravelTile neighbor in GetNeighbors(current))
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

    private int Heuristic(TravelTile a, TravelTile b)
    {
        //uses manhattan distance
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    private List<TravelTile> GetNeighbors(TravelTile tile)
    {
        List<TravelTile> neighbors = new List<TravelTile>
        {
            FindTileAt(new Vector2Int(tile.X + 1, tile.Y), currentContext),
            FindTileAt(new Vector2Int(tile.X - 1, tile.Y), currentContext),
            FindTileAt(new Vector2Int(tile.X, tile.Y + 1), currentContext),
            FindTileAt(new Vector2Int(tile.X, tile.Y - 1), currentContext)
        };
        //some of these returned could be null so we filter them out
        return neighbors.FindAll(n => n != null);
    }

    private List<TravelTile> ReconstructPath(Dictionary<TravelTile, TravelTile> cameFrom, TravelTile current)
    {
        List<TravelTile> path = new List<TravelTile> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();
        //check if total travel cost is below maximum (100)
        int totalCost = 0;
        foreach (TravelTile t in path)
        {
            totalCost += t.TravelCost;
        }
        if(totalCost > 100)
        {
            return null;
        }
        return path;
    }
    private TravelTile FindTileAt(Vector2Int location, List<TravelTile> tilesToSearch)
    {
        if (tilesToSearch == null)
        {
            throw new ArgumentNullException(nameof(tilesToSearch), "Tile list cannot be null");
        }

        TravelTile tile = tilesToSearch.Find(t => t.X == location.x && t.Y == location.y);

        if (tile == null)
        {
            Debug.Log($"Tile at {location} not found in path context.");
        }

        return tile;
    }
    private void RefreshContext()
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