using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

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

            // esperar até o fim da batalha caso estaja ocorrendo
        }
    }

    #region Turn player 1 and 2 can move three times
    private async void CountPlayer1Moves()
    {
        await StartBattleCheck(player1);
        player1Moves++;
        if (player1Moves <= 2)
        {
            player1.GetWalkableTiles();
        }
    }
    private async void CountPlayer2Moves()
    {
        var canContinue = await StartBattleCheck(player2);
        if (!canContinue) return;

        player2Moves++;
        if (player2Moves <= 2)
        {
            player2.GetWalkableTiles();
        }
    }
    #endregion

    public async Task<bool> StartBattleCheck(Player currentPlayer)
    {
        var tiles = _grid.GetOrthogonallyNeighboringTiles(currentPlayer.Tile);
        var foundPlayer = tiles.Find(item => 
            item.occupant != null && 
            item.occupant.CompareTag("Player") && 
            item.occupant != currentPlayer.gameObject);

        if (foundPlayer)
            return await StartBattle();
        return true;
    }

    public async Task<bool> StartBattle()
    {
        var player1Dices = new List<int>();
        var player2Dices = new List<int>();

        for (int i = 0; i < 3; i++)
        {
            player1Dices.Add(Random.Range(1, 7));
            player2Dices.Add(Random.Range(1, 7));
        }

        player1Dices.Sort((a, b) => b.CompareTo(a));
        player2Dices.Sort((a, b) => b.CompareTo(a));

        int player1Wins = 0, player2Wins = 0;
        for (int i = 0; i < 3; i++)
        {
            if (player1Dices[i] == player2Dices[i])
            {
                if (player1.enabled == true)
                    player1Wins++;
                else player2Wins++;
            }
            else if (player1Dices[i] > player2Dices[i])
            {
                player1Wins++;
            }
            else player2Wins++;
            
        }

        Debug.Log($"Battle in progress");
        await Task.Delay(1000);

        if (player1Wins > player2Wins)
        {
            player2.TakeDamage(player1.CurrentAttack);
            Debug.Log($"Player 1 Win Battle {player1Wins}x{player2Wins}");
        }
        else
        {
            player1.TakeDamage(player2.CurrentAttack);
            Debug.Log($"Player 2 Win Battle {player1Wins}x{player2Wins}");
        }

        if (player1.CurrentHealth < 1 || player2.CurrentHealth < 1)
        {
            Debug.Log($"Game Over");
            StopCoroutine(TurnControl());
            return false;
        }

        return true;
    }
}
