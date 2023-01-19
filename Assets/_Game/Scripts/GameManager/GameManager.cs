using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Collectable[] collectables;

    private Grid _grid;
    private Player _player1;
    private Player _player2;
    private GameObject _collectablesContainer;
    private List<Collectable> spawnedCollectables = new List<Collectable>();
    private int collectablesAmount;
    private int player1Moves = 0;
    private int player2Moves = 0;

    // Turn control variables
    private int player1TurnMoves = 2;
    private int player2TurnMoves = 2;
    private int player1ExtraTurnDices = 0;
    private int player2ExtraTurnDices = 0;
    private int turnAttacks = 1;

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _collectablesContainer = new GameObject("Power Ups");
        _collectablesContainer.AddComponent<SinAnimation>();
        _grid = GetComponent<Grid>();
        _grid.Create();

        var spawPointPlayer1 = _grid.Tiles.FirstOrDefault()[Mathf.CeilToInt(_grid.Columns / 2)];
        var spawPointPlayer2 = _grid.Tiles[_grid.Rows - 1][Mathf.CeilToInt(_grid.Columns / 2)];

        _player1 = Instantiate(player, spawPointPlayer1.transform.position + (Vector3.up * 1.25f), Quaternion.identity) as Player;
        _player1.name = "Player 1";
        _player1.targer = Target.Player1;
        _player1.Init(_grid, spawPointPlayer1);
        _player1.OnWalk += CountPlayer1Moves;

        _player2 = Instantiate(player, spawPointPlayer2.transform.position + (Vector3.up * 1.25f), Quaternion.identity) as Player;
        _player2.name = "Player 2";
        _player2.targer = Target.Player2;
        _player2.Init(_grid, spawPointPlayer2);
        _player2.OnWalk += CountPlayer2Moves;

        // Spaw Items
        SpawCollectables();
        StartCoroutine(TurnControl());
    }

    public void SpawCollectablesCheck()
    {
        spawnedCollectables.RemoveAll(item => item == null);

        if (spawnedCollectables.Count <= collectablesAmount * 0.1f)
        {
            SpawCollectables();
        }
    }

    public void SpawCollectables()
    {
        spawnedCollectables.RemoveAll(item => item == null);

        foreach (var row in _grid.Tiles)
        {
            foreach (var item in row)
            {
                if (item.occupant == null)
                {
                    // Spaw Collectable
                    var obj = Instantiate(collectables[Random.Range(0, collectables.Length)], _collectablesContainer.transform);
                    obj.transform.position += item.transform.position + Vector3.up * 1.5f;
                    item.occupant = obj.gameObject;
                    spawnedCollectables.Add(obj);
                }
            }
        }

        collectablesAmount = spawnedCollectables.Count;
    }

    private IEnumerator TurnControl()
    {
        yield return new WaitForSeconds(1f);

        while (_player1.CurrentHealth > 0 && _player2.CurrentHealth > 0)
        {
            // Init Player 1 Can Walk
            _player1.GetWalkableTiles();
            _player1.enabled = true;
            _player2.enabled = false;
            FollowTargetCamera.Instance.SetTarget(_player1.transform);
            yield return new WaitUntil(() => player1Moves > player1TurnMoves);

            // Init Player 2 Can Walk
            _player2.GetWalkableTiles();
            _player2.enabled = true;
            _player1.enabled = false;
            FollowTargetCamera.Instance.SetTarget(_player2.transform);
            yield return new WaitUntil(() => player2Moves > player2TurnMoves);

            // Reset Counters
            player1Moves = 0;
            player2Moves = 0;
            player1TurnMoves = 2;
            player2TurnMoves = 2;
            player1ExtraTurnDices = 0;
            player2ExtraTurnDices = 0;
            turnAttacks = 1;
            _player2.enabled = false;
        }
    }

    #region Turn player 1 and 2 can move three times
    private async void CountPlayer1Moves()
    {
        if (turnAttacks > 0)
        {
            var canContinue = await StartBattleCheck(_player1);
            if (!canContinue) return;
        }

        player1Moves++;
        if (player1Moves <= player1TurnMoves)
        {
            _player1.GetWalkableTiles();
        }
    }
    private async void CountPlayer2Moves()
    {
        if (turnAttacks > 0)
        {
            var canContinue = await StartBattleCheck(_player2);
            if (!canContinue) return;
        }

        player2Moves++;
        if (player2Moves <= player2TurnMoves)
        {
            _player2.GetWalkableTiles();
        }
    }
    #endregion

    private async Task<bool> StartBattleCheck(Player currentPlayer)
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

    private async Task<bool> StartBattle()
    {
        turnAttacks--;

        var player1Dices = new List<int>();
        var player2Dices = new List<int>();

        for (int i = 0; i < 3; i++)
        {
            player1Dices.Add(Random.Range(1, 7));
            player2Dices.Add(Random.Range(1, 7));
        }

        for (int i = 0; i < player1ExtraTurnDices; i++)
            player1Dices.Add(Random.Range(1, 7));
        for (int i = 0; i < player2ExtraTurnDices; i++)
            player2Dices.Add(Random.Range(1, 7));

        player1Dices.Sort((a, b) => b.CompareTo(a));
        player2Dices.Sort((a, b) => b.CompareTo(a));

        int player1Wins = 0, player2Wins = 0;
        for (int i = 0; i < 3; i++)
        {
            if (player1Dices[i] == player2Dices[i])
            {
                if (_player1.enabled == true)
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
            _player2.TakeDamage(_player1.CurrentAttack);
            Debug.Log($"Player 1 Win Battle {player1Wins}x{player2Wins}");
        }
        else
        {
            _player1.TakeDamage(_player2.CurrentAttack);
            Debug.Log($"Player 2 Win Battle {player1Wins}x{player2Wins}");
        }

        if (_player1.CurrentHealth < 1 || _player2.CurrentHealth < 1)
        {
            StopCoroutine(TurnControl());

            if (_player1.CurrentHealth > 0)
                HUDCanvas.Instance.OpenGameOverPopup("Player 1 Win");
            else HUDCanvas.Instance.OpenGameOverPopup("Player 2 Win");

            return false;
        }

        return true;
    }

    #region Collectables Actions
    public void GainExtraTurnAttack()
    {
        turnAttacks++;
    }

    public void GainExtraTurnMove(Target playerTarget)
    {
        switch (playerTarget)
        {
            case Target.Player1:
                player1TurnMoves++;
                break;
            case Target.Player2:
                player2TurnMoves++;
                break;
        }
    }

    public void GainExtraTurnDice(Target playerTarget)
    {
        switch (playerTarget)
        {
            case Target.Player1:
                player1ExtraTurnDices++;
                break;
            case Target.Player2:
                player2ExtraTurnDices++;
                break;
        }
    }

    public void RestoreSomeHealth(Target playerTarget)
    {
        switch (playerTarget)
        {
            case Target.Player1:
                _player1.TakeDamage(-Random.Range(1, 3));
                break;
            case Target.Player2:
                _player2.TakeDamage(-Random.Range(1, 3));
                break;
        }
    }
    #endregion
}

public enum Target
{
    Player1,
    Player2,
}
