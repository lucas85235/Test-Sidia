using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentObjectRotator : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 30, 0);

    private void FixedUpdate()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).Rotate(rotationSpeed * Time.fixedDeltaTime, Space.Self);
        }
    }
}
