using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float followSpeed = 5.0f;

    public static FollowTargetCamera Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 newPosition = target.position + offset;
        newPosition.y = transform.position.y;
        newPosition.z = target.position.z;
        transform.position = Vector3.Lerp(transform.position, newPosition, followSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
