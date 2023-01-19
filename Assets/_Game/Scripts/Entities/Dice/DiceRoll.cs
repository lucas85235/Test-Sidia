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

        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f), Random.Range(-1f, 1f));
        var randomForce = randomDirection * Random.Range(300, 600);

        rb.AddForce(randomForce);
        rb.AddTorque(randomForce);
        Invoke(nameof(GetRollValue), 2f);
        AudioManager.Instance.PlaySoundEffect(1);
    }

    private void GetRollValue()
    {
        Vector3 euler = transform.eulerAngles;
        euler.x = Mathf.Round(euler.x / 90) * 90;
        euler.y = Mathf.Round(euler.y / 90) * 90;
        euler.z = Mathf.Round(euler.z / 90) * 90;

        rollValue = (int)(euler.y / 90) + 1;
        button.interactable = false;
        OnRoll?.Invoke(rollValue);
    }
}
