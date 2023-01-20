using Game.Generics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PunchAnimation))]
public class Player : MonoBehaviour
{
    [SerializeField] private Renderer mesh;
    [SerializeField] private ParticleSystem punchVfx;
    [SerializeField] private PlayerData data;
    [SerializeField] private float smoothSpeed = 5f;

    public Action OnWalk;
    public Tile Tile { get => _tile; }
    public Target targer { get; set; }
    public Renderer Renderer { get => mesh; }
    public int CurrentHealth
    {
        get => _currentHealth; private set
        {
            _currentHealth = value;
            GetPlayerHud().UpdateLife(value, targer == Target.Player2);
        }
    }
    public int CurrentAttack
    {
        get => _currentAttack;
        private set
        {
            _currentAttack = value;
            GetPlayerHud().UpdateAttack(value, targer == Target.Player2);
        }
    }

    private Grid _grid;
    private Tile _tile;
    private PunchAnimation _punch;
    private List<Tile> _currentWalkableTiles = new List<Tile>();
    private int _currentHealth;
    private int _currentAttack;

    private void Start()
    {
        _punch = GetComponent<PunchAnimation>();
        var allData = Resources.LoadAll<PlayerData>("PlayerData");
        data = allData[UnityEngine.Random.Range(0, allData.Length)];
        CurrentHealth = data.Health;
        CurrentAttack = data.Attack;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Check if has selected tile
            Tile selectedTile = null;
            foreach (var item in _currentWalkableTiles)
            {
                if (item.IsSelectad)
                {
                    selectedTile = item;
                    break;
                }
            }

            // Disable selected mode in Tiles
            if (selectedTile != null) WalkTo(selectedTile);
        }
        else if (targer == Target.Player1)
        {
            if (!_currentWalkableTiles.Any()) return;
            if (Input.GetKeyDown(KeyCode.W))
            {
                var tile = _grid.GetTopNeighboringTile(_tile);
                if (tile != null) WalkTo(tile);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                var tile = _grid.GetBottonNeighboringTile(_tile);
                if (tile != null) WalkTo(tile);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                var tile = _grid.GetRightNeighboringTile(_tile);
                if (tile != null) WalkTo(tile);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                var tile = _grid.GetLeftNeighboringTile(_tile);
                if (tile != null) WalkTo(tile);
            }
        }
        else
        {
            if (!_currentWalkableTiles.Any()) return;
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                var tile = _grid.GetTopNeighboringTile(_tile);
                if (tile != null) WalkTo(tile);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                var tile = _grid.GetBottonNeighboringTile(_tile);
                if (tile != null) WalkTo(tile);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                var tile = _grid.GetRightNeighboringTile(_tile);
                if (tile != null) WalkTo(tile);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                var tile = _grid.GetLeftNeighboringTile(_tile);
                if (tile != null) WalkTo(tile);
            }
        }
    }

    public void Init(Grid grid, Tile tile)
    {
        _grid = grid;
        _tile = tile;
        _tile.occupant = gameObject;
    }

    public void TakeDamage(int damage)
    {
        if (CurrentHealth - damage > data.Health)
        {
            CurrentHealth = data.Health;
            return;
        }

        if (CurrentHealth - damage < CurrentHealth)
        {
            AudioManager.Instance.PlaySoundEffect(2);
            punchVfx.Play();
            GameAssets.CreateDamageText(transform.position + Vector3.up, damage * -1);
        }
        else GameAssets.CreateHealthText(transform.position + Vector3.up, damage * -1);

        CurrentHealth -= damage;
    }

    public void GetWalkableTiles()
    {
        _currentWalkableTiles = _grid.GetOrthogonallyNeighboringTiles(_tile);
        var foundPlayers = _currentWalkableTiles.RemoveAll(item => item.occupant != null && item.occupant.CompareTag("Player") && item.occupant != gameObject);

        foreach (var item in _currentWalkableTiles)
        {
            item.SelectableMode(true);
        }
    }

    public void PunchAnimation(Transform target)
    {
        _punch.Punch(target);
    }

    private void WalkTo(Tile tile)
    {
        if (tile.occupant != null && tile.occupant.CompareTag("Player")) return;
        foreach (var item in _currentWalkableTiles) item.SelectableMode(false);
        _currentWalkableTiles = new List<Tile>();

        if (_tile != null)
            _tile.occupant = null;

        // Check if have collectable
        if (tile.occupant != null && tile.occupant.TryGetComponent(out Collectable collectable))
        {
            collectable.OnPick(targer);
            GameManager.Instance.SpawCollectablesCheck();
        }

        _tile = tile;
        _tile.occupant = gameObject;
        StartCoroutine(SmoothMovement(tile.transform.position + (Vector3.up * 1.25f)));
        AudioManager.Instance.PlaySoundEffect(0);
    }

    private IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > 0.001f)
        {
            Vector3 newPosition = Vector3.Lerp(transform.position, end, smoothSpeed * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return new WaitForEndOfFrame();
        }

        OnWalk?.Invoke();
    }

    private PlayerHUD GetPlayerHud()
    {
        if (targer == Target.Player1)
            return HUDCanvas.Instance.Player1;
        return HUDCanvas.Instance.Player2;
    }
}
