using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [Header("Grid Size")]
    [SerializeField][Min(2)] private int x = 6;
    [SerializeField][Min(2)] private int y = 6;

    [Header("Settings")]
    [SerializeField] private Tile tile;
    [SerializeField] private Material tileMaterial1;
    [SerializeField] private Material tileMaterial2;

    private Dictionary<int, List<Tile>> _tilesInGrid = new Dictionary<int, List<Tile>>();
    public Dictionary<int, List<Tile>> Tiles { get => _tilesInGrid; }

    public void Create()
    {
        if (Tiles.Any()) return;

        var initialPosition = Vector3.zero;
        var tilesContainer = new GameObject("Tiles Container");

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                var tilePosition = new Vector3(
                    initialPosition.x + j,
                    initialPosition.y,
                    initialPosition.z + i
                );

                var obj = Instantiate(tile, tilePosition, Quaternion.identity, tilesContainer.transform);
                obj.name = $"Tile ({j}, {i})";

                // Change tile colors interspersing between tileMaterial1 and tileMaterial2
                var renderer = obj.GetComponent<Renderer>();
                if ((j + i) % 2 == 0)
                    renderer.sharedMaterial = tileMaterial1;
                else renderer.sharedMaterial = tileMaterial2;

                if (!_tilesInGrid.ContainsKey(x))
                {
                    _tilesInGrid.Add(x, new List<Tile>());
                }
                _tilesInGrid[x].Add(obj.GetComponent<Tile>());
            }
        }
    }
}
