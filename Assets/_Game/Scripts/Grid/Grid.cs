using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [Header("Grid Size")]
    [SerializeField][Min(2)] private int rows = 16;
    [SerializeField][Min(2)] private int columns = 16;

    [Header("Settings")]
    [SerializeField] private Tile tile;
    [SerializeField] private Material tileMaterial1;
    [SerializeField] private Material tileMaterial2;

    public Tile[][] Tiles { get; private set; }
    public int Rows { get => rows; }
    public int Columns { get => columns; }

    public void Create()
    {
        if (Tiles != null && Tiles.Length > 0) return;

        Tiles = new Tile[rows][];
        var initialPosition = Vector3.zero;
        var tilesContainer = new GameObject("Tiles Container");

        for (int iRow = 0; iRow < rows; iRow++)
        {
            for (int iColumn = 0; iColumn < columns; iColumn++)
            {
                var tilePosition = new Vector3(
                    initialPosition.x + iColumn,
                    initialPosition.y,
                    initialPosition.z + iRow
                );

                var obj = Instantiate(tile, tilePosition, Quaternion.identity, tilesContainer.transform) as Tile;
                obj.name = $"Tile ({iColumn}, {iRow})";
                obj.row = iRow;
                obj.column = iColumn;

                // Change tile colors interspersing between tileMaterial1 and tileMaterial2
                var renderer = obj.GetComponent<Renderer>();
                if ((iColumn + iRow) % 2 == 0)
                    renderer.sharedMaterial = tileMaterial1;
                else renderer.sharedMaterial = tileMaterial2;

                if (Tiles[iRow] == null)
                    Tiles[iRow] = new Tile[columns];

                Tiles[iRow][iColumn] = obj;
            }
        }
    }

    public List<Tile> GetNeighboringTiles(Tile tile)
    {
        var neighboringTiles = new List<Tile>();

        // Get the coordinates of the tile
        var tileRow = tile.row;
        var tileColumn = tile.column;

        // Check if the neighboring tiles exist in the grid
        if (tileRow > 0)
        {
            neighboringTiles.Add(Tiles[tileRow - 1][tileColumn]);
            if (tileColumn > 0)
                neighboringTiles.Add(Tiles[tileRow - 1][tileColumn - 1]);
            if (tileColumn < columns - 1)
                neighboringTiles.Add(Tiles[tileRow - 1][tileColumn + 1]);
        }

        if (tileRow < rows - 1)
        {
            neighboringTiles.Add(Tiles[tileRow + 1][tileColumn]);
            if (tileColumn > 0)
                neighboringTiles.Add(Tiles[tileRow + 1][tileColumn - 1]);
            if (tileColumn < columns - 1)
                neighboringTiles.Add(Tiles[tileRow + 1][tileColumn + 1]);
        }

        if (tileColumn > 0)
            neighboringTiles.Add(Tiles[tileRow][tileColumn - 1]);
        if (tileColumn < columns - 1)
            neighboringTiles.Add(Tiles[tileRow][tileColumn + 1]);

        return neighboringTiles;
    }

    public List<Tile> GetOrthogonallyNeighboringTiles(Tile tile)
    {
        var neighboringTiles = new List<Tile>();

        // Get the coordinates of the tile
        var tileRow = tile.row;
        var tileColumn = tile.column;

        if (tileRow > 0)
            neighboringTiles.Add(Tiles[tileRow - 1][tileColumn]);
        if (tileRow < rows - 1)
            neighboringTiles.Add(Tiles[tileRow + 1][tileColumn]);

        if (tileColumn > 0)
            neighboringTiles.Add(Tiles[tileRow][tileColumn - 1]);
        if (tileColumn < columns - 1)
            neighboringTiles.Add(Tiles[tileRow][tileColumn + 1]);

        return neighboringTiles;
    }
}
