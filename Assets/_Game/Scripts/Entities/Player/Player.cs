using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData data;
    
    public int CurrentHealth { get; set; }
    public int CurrentAttack { get; set; }
    
    private void Awake()
    {
        CurrentHealth = data.Health;
        CurrentAttack = data.Attack;
    }
}
