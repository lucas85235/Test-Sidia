using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Transform target;
    [SerializeField] private Tile currentTile;

    public void PathLog()
    {
        var list = FindPath();
        foreach (var item in list)
        {
            Debug.Log($"{item.gameObject.name}");
        }
    }

    public List<Tile> FindPath()
    {
        // var startTile = GetStartTile();
        var startTile = currentTile;
        var queue = new Queue<Tile>();
        var visited = new HashSet<Tile>();

        queue.Enqueue(startTile);
        visited.Add(startTile);

        while (queue.Count > 0)
        {
            var currentTile = queue.Dequeue();

            if (Vector3.Distance(currentTile.transform.position, target.position) < 3)
            // if (currentTile.occupant == target)
                return ReconstructPath(currentTile);

            foreach (var neighbor in grid.GetNeighboringTiles(currentTile))
            {
                if (!visited.Contains(neighbor) && neighbor.occupant == null || !neighbor.occupant.CompareTag("Player"))
                {
                    neighbor.Previous = currentTile;
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return null;
    }

    private Tile GetStartTile()
    {
        // Find the starting tile based on the current position of the AI
        // For example, by finding the tile with the closest position to the AI's position

        return null;
    }

    private List<Tile> ReconstructPath(Tile targetTile)
    {
        var path = new List<Tile>();
        var currentTile = targetTile;

        while (currentTile != null)
        {
            path.Add(currentTile);
            currentTile = currentTile.Previous;
        }

        path.Reverse();
        return path;
    }
}
