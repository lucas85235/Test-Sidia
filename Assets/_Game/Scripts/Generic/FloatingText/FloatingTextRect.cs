using UnityEngine;
using TMPro;

namespace Game.Generics
{
    public class FloatingTextRect : MonoBehaviour
    {
        [SerializeField] private float DISAPPEAR_TIMER_MAX = 1f;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private RectTransform _rectTransform;

        private Color _textColor;
        private Vector3 _moveVector;
        private Vector3 _initialScale;
        private float _disappearTime;

        public void Awake()
        {
            // _text = GetComponent<TextMeshProUGUI>();
            // _rectTransform = GetComponent<RectTransform>();
            _textColor = _text.color;
            _initialScale = _rectTransform.localScale;
        }

        private void OnDisable()
        {
            _textColor.a = 1f;
            _text.color = _textColor;
            _rectTransform.localScale = _initialScale;
        }

        public void Init(string damage)
        {
            if (_text == null)
            {
                Awake();
            }

            _text.SetText(damage.ToString());
            _disappearTime = DISAPPEAR_TIMER_MAX;
            _moveVector = new Vector3(0.1f, 0, 1f) * 5;
            gameObject.SetActive(true);
        }

        private void FixedUpdate()
        {
            _rectTransform.anchoredPosition += (Vector2)_moveVector * Time.fixedDeltaTime;
            _moveVector -= _moveVector * 8f * Time.fixedDeltaTime;

            if (_disappearTime > DISAPPEAR_TIMER_MAX * .75f)
            {
                if (_rectTransform.localScale.sqrMagnitude < (Vector3.one * 1.5f).sqrMagnitude)
                    _rectTransform.localScale += (Vector3.one * DISAPPEAR_TIMER_MAX) * Time.fixedDeltaTime;
            }

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
}
