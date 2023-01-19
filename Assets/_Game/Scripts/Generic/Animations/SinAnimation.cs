using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinAnimation : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float distance = 0.2f;
    [SerializeField] private float speed = 2f;

    private void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, Mathf.Sin(Time.time * speed) * distance, transform.position.z);
    }
}
