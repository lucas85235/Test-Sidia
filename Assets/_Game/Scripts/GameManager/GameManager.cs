using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;

    [Header("View Only")]
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;

    private Grid _grid;
    private int player1Moves = 0;
    private int player2Moves = 0;

    private void Start()
    {
        _grid = GetComponent<Grid>();
        _grid.Create();

        var spawPointPlayer1 = _grid.Tiles.FirstOrDefault()[Mathf.CeilToInt(_grid.Columns / 2)];
        var spawPointPlayer2 = _grid.Tiles[_grid.Rows - 1][Mathf.CeilToInt(_grid.Columns / 2)];

        player1 = Instantiate(player, spawPointPlayer1.transform.position + (Vector3.up * 1.25f), Quaternion.identity) as Player;
        player1.name = "Player 1";
        player1.Init(_grid, spawPointPlayer1);
        player1.OnWalk += CountPlayer1Moves;

        player2 = Instantiate(player, spawPointPlayer2.transform.position + (Vector3.up * 1.25f), Quaternion.identity) as Player;
        player2.name = "Player 2";
        player2.Init(_grid, spawPointPlayer2);
        player2.OnWalk += CountPlayer2Moves;

        StartCoroutine(TurnControl());
    }

    private IEnumerator TurnControl()
    {
        yield return new WaitForSeconds(1f);

        while (player1.CurrentHealth > 0 && player2.CurrentHealth > 0)
        {
            // Init Player 1 Can Walk
            player1.GetWalkableTiles();
            player1.enabled = true;
            player2.enabled = false;
            yield return new WaitUntil(() => player1Moves > 2);

            // Init Player 2 Can Walk
            player2.GetWalkableTiles();
            player2.enabled = true;
            player1.enabled = false;
            yield return new WaitUntil(() => player2Moves > 2);

            // Reset Counters
            player1Moves = 0;
            player2Moves = 0;
            player2.enabled = false;
        }
    }

    // Turn player 1 and 2 can move three times

    private void CountPlayer1Moves()
    {
        player1Moves++;
        if (player1Moves <= 2)
        {
            player1.GetWalkableTiles();
        }
    }
    private void CountPlayer2Moves()
    {
        player2Moves++;
        if (player2Moves <= 2)
        {
            player2.GetWalkableTiles();
        }
    }
}
