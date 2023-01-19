using Game.Generics;
using UnityEngine;

public class FollowTargetCamera : Singleton<FollowTargetCamera>
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float followSpeed = 5.0f;

    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 newPosition = target.position + offset;
        newPosition.y = transform.position.y + offset.x;
        newPosition.z = target.position.z + offset.z;
        transform.position = Vector3.Lerp(transform.position, newPosition, followSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
