using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    private const float DISAPPEAR_TIMER_MAX = 1f;

    private TextMeshPro _text;
    private Color _textColor;
    private Vector3 _moveVector;
    private Vector3 _initialScale;
    private float _disappearTime;

    public void Awake()
    {
        _text = GetComponent<TextMeshPro>();
        _textColor = _text.color;
        _initialScale = transform.localScale;
    }

    private void OnDisable()
    {
        _textColor.a = 1f;
        _text.color = _textColor;
        transform.localScale = _initialScale;
    }

    public void Init(int damage)
    {
        _text.SetText(damage.ToString());
        _disappearTime = DISAPPEAR_TIMER_MAX;
        _moveVector = new Vector3(0.1f, 0, 1f) * 5;
    }

    private void FixedUpdate()
    {
        transform.position += _moveVector * Time.fixedDeltaTime;
        _moveVector -= _moveVector * 8f * Time.fixedDeltaTime;

        if (_disappearTime > DISAPPEAR_TIMER_MAX * .5f)
            transform.localScale += Vector3.one * Time.fixedDeltaTime;
        else transform.localScale -= Vector3.one * Time.fixedDeltaTime;

        _disappearTime -= Time.fixedDeltaTime;

        if (_disappearTime <= 0)
        {
            float disappearSpeed = 3f;
            _textColor.a -= disappearSpeed * Time.fixedDeltaTime;
            _text.color = _textColor;

            if (_textColor.a <= 0) gameObject.SetActive(false);
        }
    }
}
