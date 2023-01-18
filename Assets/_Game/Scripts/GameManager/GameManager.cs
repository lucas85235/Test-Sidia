using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;

    private Grid _grid;

    private void Start()
    {
        _grid = GetComponent<Grid>();
        _grid.Create();

        var rows = _grid.Tiles.FirstOrDefault().Length;
        var spawPoint = _grid.Tiles[0][Mathf.CeilToInt(rows / 2)];

        var spawPlayer = Instantiate(player, spawPoint.transform.position + (Vector3.up * 1.25f), Quaternion.identity) as Player;
        spawPlayer.Init(_grid, spawPoint);
    }
}
