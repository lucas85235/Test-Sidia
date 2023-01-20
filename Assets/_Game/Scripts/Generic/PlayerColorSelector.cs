using UnityEngine;

namespace Game.Generics
{
    public class PlayerColorSelector : MonoBehaviour
    {
        [SerializeField] private bool useSecondaryColor = false;
        [SerializeField] private Renderer playerRenderer;
        [SerializeField] private Color[] playerColors;

        private int _currentColorIndex = 0;
        private const string SELECTED_COLOR_INDEX_KEY = "selected_color_index";

        private void Awake()
        {
            LoadSelectedColorIndex();
            if (playerRenderer != null)
                playerRenderer.material.color = !useSecondaryColor ? GetCurrentColor() : GetAnalogColor();
        }

        public void NextColor()
        {
            _currentColorIndex++;
            if (_currentColorIndex >= playerColors.Length)
                _currentColorIndex = 0;

            playerRenderer.material.color = !useSecondaryColor ? GetCurrentColor() : GetAnalogColor();
            SaveSelectedColorIndex();
            AudioManager.Instance.PlaySoundEffect(11);
        }
        public void PreviousColor()
        {
            _currentColorIndex--;
            if (_currentColorIndex < 0)
                _currentColorIndex = playerColors.Length - 1;

            playerRenderer.material.color = !useSecondaryColor ? GetCurrentColor() : GetAnalogColor();
            SaveSelectedColorIndex();
            AudioManager.Instance.PlaySoundEffect(11);
        }

        public Color GetCurrentColor()
        {
            return playerColors[_currentColorIndex];
        }
        public Color GetAnalogColor()
        {
            int analogColorIndex = _currentColorIndex + playerColors.Length / 2;
            if (analogColorIndex >= playerColors.Length)
                analogColorIndex -= playerColors.Length;

            return playerColors[analogColorIndex];
        }

        #region Save and Load
        private void SaveSelectedColorIndex()
        {
            PlayerPrefs.SetInt(SELECTED_COLOR_INDEX_KEY, _currentColorIndex);
        }
        private void LoadSelectedColorIndex()
        {
            if (PlayerPrefs.HasKey(SELECTED_COLOR_INDEX_KEY))
            {
                _currentColorIndex = PlayerPrefs.GetInt(SELECTED_COLOR_INDEX_KEY);
            }
        }
        #endregion
    }
}
