using System.Collections;
using UnityEngine;
using TMPro;

namespace Game.Generics
{
    public class HighlightText : MonoBehaviour
    {
        public Color highlightColor = Color.yellow;
        public float duration = 0.8f;

        private TextMeshProUGUI _text;
        private Color _originalColor;

        public string text
        {
            get => _text.text;
            set
            {
                _text.text = value;
                Highlight();
            }
        }

        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _originalColor = _text.color;
        }

        public void Highlight()
        {
            StopAllCoroutines();
            StartCoroutine(HighlightCoroutine());
        }

        private IEnumerator HighlightCoroutine()
        {
            _text.color = highlightColor;
            float timer = 0.0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                _text.color = Color.Lerp(highlightColor, _originalColor, timer / duration);
                yield return null;
            }
        }
    }    
}
