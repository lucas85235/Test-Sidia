using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DiceRoll : MonoBehaviour
{
    public Button button;
    public int sides = 6;

    private int rollValue;
    private Rigidbody rb;

    public Action<int> OnRoll;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        button.onClick.AddListener(Roll);
        AtiveRoolButton(false);
    }

    public void AtiveRoolButton(bool active)
    {
        button.interactable = active;
    }

    public void Roll()
    {
        button.interactable = false;

        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        var randomForce = randomDirection * Random.Range(200, 600);

        rb.AddForce(randomForce);
        rb.AddTorque(randomForce);
        Invoke(nameof(GetRollValue), 2f);
    }

    private void GetRollValue()
    {
        rollValue = (int)transform.eulerAngles.x % sides + 1;
        OnRoll?.Invoke(rollValue);
    }
}
