using System.Collections;
using UnityEngine;

namespace Game.Generics
{
    public class PunchAnimation : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float shakeAmount = 0.25f;
        [SerializeField] private float shakeDuration = 0.15f;

        private Vector3 _startPos;
        private Vector3 _shakePos;
        private Vector3 _shakeStartPos;

        public void Punch(Transform target)
        {
            StopAllCoroutines();
            StartCoroutine(PunchCoroutine(target));
        }

        private IEnumerator PunchCoroutine(Transform target)
        {
            _startPos = transform.position;
            _shakeStartPos = target.position;

            float timer = 0.0f;
            while (timer < 1.0f)
            {
                timer += Time.deltaTime * speed;
                transform.position = Vector3.Lerp(_startPos, target.position, timer);
                _shakePos = target.position + Random.insideUnitSphere * shakeAmount;
                target.position = Vector3.Lerp(target.position, _shakePos, Time.deltaTime * speed);
                yield return null;
            }
            timer = 0.0f;
            while (timer < 1.0f)
            {
                timer += Time.deltaTime * speed;
                transform.position = Vector3.Lerp(target.position, _startPos, timer);
                _shakePos = target.position + Random.insideUnitSphere * shakeAmount;
                target.position = Vector3.Lerp(target.position, _shakePos, Time.deltaTime * speed);
                yield return null;
            }

            target.position = _shakeStartPos;
        }
    }
}
