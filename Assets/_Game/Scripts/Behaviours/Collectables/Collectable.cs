using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Collectable : MonoBehaviour
{
    [SerializeField] private CollectablesTypes collectable;
    [SerializeField] private bool randomCollectable = false;

    // private void Start()
    // {
    //     if (randomCollectable)
    //     {
    //         collectable = (CollectablesTypes)Random.Range(0, Enum.GetValues(typeof(CollectablesTypes)).Length);
    //     }
    // }

    public void OnPick(Target player)
    {
        // Debug.Log($"Collected for {player} gain {collectable}");
        switch (collectable)
        {
            case CollectablesTypes.ExtraMove:
                GameManager.Instance.GainExtraTurnMove(player);
                break;
            case CollectablesTypes.ExtraAttack:
                GameManager.Instance.GainExtraTurnAttack();
                break;
            case CollectablesTypes.ExtraDice:
                GameManager.Instance.GainExtraTurnDice(player);
                break;
            case CollectablesTypes.RestoreHealth:
                GameManager.Instance.RestoreSomeHealth(player);
                break;
        }

        Destroy(gameObject);
    }

    public enum CollectablesTypes
    {
        ExtraMove,
        ExtraAttack,
        ExtraDice,
        RestoreHealth,
    }
}
