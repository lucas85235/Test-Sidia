using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData data;
    [SerializeField] private float smoothSpeed = 5f;

    private Grid _grid;
    private Tile _tile;
    private List<Tile> _currentWalkableTiles = new List<Tile>();

    public Action OnWalk;
    public Tile Tile { get => _tile; }
    public Target targer { get; set; }
    public int CurrentHealth { get; private set; }
    public int CurrentAttack { get; private set; }

    private void Awake()
    {
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
            if (selectedTile != null)
            {
                foreach (var item in _currentWalkableTiles)
                    item.SelectableMode(false);
                WalkTo(selectedTile);
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
        CurrentHealth -= damage;

        if (CurrentAttack > data.Health)
            CurrentHealth = data.Health;
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

    private void WalkTo(Tile tile)
    {
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
}
