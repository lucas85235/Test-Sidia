using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private DiceRoll _dice;
    [SerializeField] private PlayerColorSelector _colorSelector;
    [SerializeField] private Collectable[] collectables;

    private Grid _grid;
    private Player _player1;
    private Player _player2;
    private Target _currentTarget;
    private GameObject _collectablesContainer;
    private List<Collectable> spawnedCollectables = new List<Collectable>();
    private int collectablesAmount;
    private int turnCounter;
    private bool isPaused = false;

    // Turn control variables
    private int player1Moves = 0;
    private int Player1Moves
    {
        get => player1Moves;
        set
        {
            player1Moves = value;
            HUDCanvas.Instance.Player1.UpdateMoves(player1Moves, player1TurnMoves + 1);
        }
    }
    private int player2Moves = 0;
    private int Player2Moves
    {
        get => player2Moves;
        set
        {
            player2Moves = value;
            HUDCanvas.Instance.Player2.UpdateMoves(player2Moves, player2TurnMoves + 1, true);
        }
    }

    private int player1TurnMoves = 2;
    private int Player1TurnMoves
    {
        get => player1TurnMoves;
        set
        {
            player1TurnMoves = value;
            HUDCanvas.Instance.Player1.UpdateMoves(player1Moves, player1TurnMoves + 1);
        }
    }
    private int player2TurnMoves = 2;
    private int Player2TurnMoves
    {
        get => player2TurnMoves;
        set
        {
            player2TurnMoves = value;
            HUDCanvas.Instance.Player2.UpdateMoves(player2Moves, player2TurnMoves + 1, true);
        }
    }
    private int player1ExtraTurnDices = 0;
    private int Player1ExtraTurnDices
    {
        get => player1ExtraTurnDices;
        set
        {
            player1ExtraTurnDices = value;
            HUDCanvas.Instance.Player1.UpdateDices(value + 3);
        }
    }
    private int player2ExtraTurnDices = 0;
    private int Player2ExtraTurnDices
    {
        get => player2ExtraTurnDices;
        set
        {
            player2ExtraTurnDices = value;
            HUDCanvas.Instance.Player2.UpdateDices(value + 3, true);
        }
    }

    private int turnAttacks = 1;
    private int TurnAttacks
    {
        get => turnAttacks;
        set
        {
            turnAttacks = value;
            HUDCanvas.Instance.UpdateTurnAttacks(value);
        }
    }

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
        _dice.OnRoll += OnRoll;

        var spawPointPlayer1 = _grid.Tiles.FirstOrDefault()[Mathf.CeilToInt(_grid.Columns / 2)];
        var spawPointPlayer2 = _grid.Tiles[_grid.Rows - 1][Mathf.CeilToInt(_grid.Columns / 2)];

        _player1 = Instantiate(player, spawPointPlayer1.transform.position + (Vector3.up * 1.25f), Quaternion.identity) as Player;
        _player1.name = "Player 1";
        _player1.targer = Target.Player1;
        _player1.Init(_grid, spawPointPlayer1);
        _player1.Renderer.material.color = _colorSelector.GetCurrentColor();
        _player1.OnWalk += CountPlayer1Moves;

        _player2 = Instantiate(player, spawPointPlayer2.transform.position + (Vector3.up * 1.25f), Quaternion.identity) as Player;
        _player2.name = "Player 2";
        _player2.targer = Target.Player2;
        _player2.Init(_grid, spawPointPlayer2);
        _player2.Renderer.material.color = _colorSelector.GetAnalogColor();
        _player2.OnWalk += CountPlayer2Moves;

        // Spaw Items
        SpawCollectables();
        StartCoroutine(TurnControl());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HUDCanvas.Instance.PauseMenu();
            Pause();
        }
    }

    public void Pause()
    {
        isPaused = !isPaused;
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
            turnCounter++;
            HUDCanvas.Instance.UpdateTurn(turnCounter);

            // Reset Control Variables
            TurnAttacks = 1;

            Player1ExtraTurnDices = 0;
            Player2ExtraTurnDices = 0;
            Player1Moves = 0;
            Player2Moves = 0;
            Player1TurnMoves = 2;
            Player2TurnMoves = 2;

            // Init Player 1 Can Walk
            _currentTarget = Target.Player1;
            _player1.GetWalkableTiles();
            _player1.enabled = true;
            _player2.enabled = false;
            FollowTargetCamera.Instance.SetTarget(_player1.transform);
            yield return new WaitUntil(() => Player1Moves > Player1TurnMoves);

            // Init Player 2 Can Walk
            _currentTarget = Target.Player2;
            _player2.GetWalkableTiles();
            _player2.enabled = true;
            _player1.enabled = false;
            FollowTargetCamera.Instance.SetTarget(_player2.transform);
            yield return new WaitUntil(() => Player2Moves > Player2TurnMoves);


            _player2.enabled = false;
        }
    }

    #region Turn player 1 and 2 can move three times
    private async void CountPlayer1Moves()
    {
        if (TurnAttacks > 0)
        {
            var canContinue = await StartBattleCheck(_player1);
            if (!canContinue) return;
        }

        Player1Moves++;
        if (Player1Moves <= Player1TurnMoves)
        {
            _player1.GetWalkableTiles();
        }
    }
    private async void CountPlayer2Moves()
    {
        if (TurnAttacks > 0)
        {
            var canContinue = await StartBattleCheck(_player2);
            if (!canContinue) return;
        }

        Player2Moves++;
        if (Player2Moves <= Player2TurnMoves)
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
        TurnAttacks--;

        player1Dices = new List<int>();
        player2Dices = new List<int>();
        _player1TurnDices = Player1ExtraTurnDices + 3;
        _player2TurnDices = Player2ExtraTurnDices + 3;
        _dice.AtiveRoolButton(true);

        HUDCanvas.Instance.AlertText("Roll the dice player 1");
        _currentTargetInBattle = Target.Player1;
        while (_player1TurnDices > player1Dices.Count)
            await Task.Delay(500);

        HUDCanvas.Instance.AlertText("Roll the dice player 2");
        _currentTargetInBattle = Target.Player2;
        while (_player2TurnDices > player2Dices.Count)
            await Task.Delay(500);

        _dice.AtiveRoolButton(false);

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

        await Task.Delay(1000);
        if (player1Wins > player2Wins)
        {
            _player2.TakeDamage(_player1.CurrentAttack);
            _player1.PunchAnimation(_player2.transform);
            HUDCanvas.Instance.AlertText($"Player 1 won the battle");
        }
        else
        {
            _player1.TakeDamage(_player2.CurrentAttack);
            _player2.PunchAnimation(_player1.transform);
            HUDCanvas.Instance.AlertText($"Player 2 won the battle");
        }

        if (_player1.CurrentHealth < 1 || _player2.CurrentHealth < 1)
        {
            StopCoroutine(TurnControl());

            if (_player1.CurrentHealth > 0)
                HUDCanvas.Instance.OpenGameOverPopup("Player 1 Win");
            else HUDCanvas.Instance.OpenGameOverPopup("Player 2 Win");

            return false;
        }

        await Task.Delay(1000);
        return true;
    }

    private Target _currentTargetInBattle;
    private List<int> player1Dices;
    private List<int> player2Dices;
    private int _player1TurnDices;
    private int _player2TurnDices;
    private void OnRoll(int rollValue)
    {
        Transform spawPoint;
        if (_currentTargetInBattle == Target.Player1)
        {
            player1Dices.Add(rollValue);
            spawPoint = _player1.transform;
        }
        else
        {
            player2Dices.Add(rollValue);
            spawPoint = _player2.transform;
        }

        GameAssets.CreateRollText(spawPoint.position + Vector3.up, rollValue);
        _dice.AtiveRoolButton(true);
    }

    #region Collectables Actions
    public void GainExtraTurnAttack()
    {
        TurnAttacks++;
    }

    public void GainExtraTurnMove(Target playerTarget)
    {
        switch (playerTarget)
        {
            case Target.Player1:
                Player1TurnMoves++;
                break;
            case Target.Player2:
                Player2TurnMoves++;
                break;
        }
    }

    public void GainExtraTurnDice(Target playerTarget)
    {
        switch (playerTarget)
        {
            case Target.Player1:
                Player1ExtraTurnDices++;
                break;
            case Target.Player2:
                Player2ExtraTurnDices++;
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
