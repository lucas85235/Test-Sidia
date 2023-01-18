using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]

public class PlayerData : ScriptableObject
{
    [SerializeField] private int health = 10;
    [SerializeField] private int attack = 3;

    public int Health { get => health; }
    public int Attack { get => attack; }
}
