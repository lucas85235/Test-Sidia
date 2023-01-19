using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float shakeAmount = 0.25f;
    [SerializeField] private float shakeDuration = 0.15f;

    private Vector3 startPos;
    private Vector3 shakePos;
    private Vector3 shakeStartPos;

    public void Punch(Transform target)
    {
        StopAllCoroutines();
        StartCoroutine(PunchCoroutine(target));
    }

    private IEnumerator PunchCoroutine(Transform target)
    {
        startPos = transform.position;
        shakeStartPos = target.position;

        float timer = 0.0f;
        while (timer < 1.0f)
        {
            timer += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(startPos, target.position, timer);
            shakePos = target.position + Random.insideUnitSphere * shakeAmount;
            target.position = Vector3.Lerp(target.position, shakePos, Time.deltaTime * speed);
            yield return null;
        }

        timer = 0.0f;
        while (timer < 1.0f)
        {
            timer += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(target.position, startPos, timer);
            shakePos = target.position + Random.insideUnitSphere * shakeAmount;
            target.position = Vector3.Lerp(target.position, shakePos, Time.deltaTime * speed);
            yield return null;
        }

        target.position = shakeStartPos;
    }
}
