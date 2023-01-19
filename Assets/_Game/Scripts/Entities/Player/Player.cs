using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PunchAnimation))]
public class Player : MonoBehaviour
{
    [SerializeField] private Renderer mesh;
    [SerializeField] private PlayerData data;
    [SerializeField] private float smoothSpeed = 5f;

    private Grid _grid;
    private Tile _tile;
    private PunchAnimation _punch;
    private List<Tile> _currentWalkableTiles = new List<Tile>();

    public Action OnWalk;
    public Tile Tile { get => _tile; }
    public Target targer { get; set; }
    public Renderer Renderer { get => mesh; }

    private int _currentHealth;
    public int CurrentHealth
    {
        get => _currentHealth; private set
        {
            _currentHealth = value;
            GetPlayerHud().UpdateLife(value, targer == Target.Player2);
        }
    }
    private int _currentAttack;
    public int CurrentAttack
    {
        get => _currentAttack;
        private set
        {
            _currentAttack = value;
            GetPlayerHud().UpdateAttack(value, targer == Target.Player2);
        }
    }

    private void Start()
    {
        _punch = GetComponent<PunchAnimation>();
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
        if (CurrentHealth - damage > data.Health)
        {
            CurrentHealth = data.Health;
            return;
        }

        if (CurrentHealth - damage < CurrentHealth)
        {
            AudioManager.Instance.PlaySoundEffect(2);
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

    private HUDCanvas.PlayerHUD GetPlayerHud()
    {
        if (targer == Target.Player1)
            return HUDCanvas.Instance.Player1;
        return HUDCanvas.Instance.Player2;
    }
}
