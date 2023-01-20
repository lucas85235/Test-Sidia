using Game.Generics;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button charactersButton;
    [SerializeField] private Button backToMainButton;

    [Header("Popups")]
    [SerializeField] private Popup options;

    [Header("Screens")]
    [SerializeField] private GameObject mainScreen;
    [SerializeField] private GameObject charactersScreen;

    private void Awake()
    {
        startButton.onClick.AddListener(StartButton);
        quitButton.onClick.AddListener(QuitButton);
        optionsButton.onClick.AddListener(OptionsButton);
        charactersButton.onClick.AddListener(CharactersButton);
        backToMainButton.onClick.AddListener(BackToMainButton);
    }

    private void StartButton()
    {
        SceneLoader.LoadScene("Game");
    }

    private void QuitButton()
    {
        SceneLoader.QuitGame();
    }

    private void OptionsButton()
    {
        options.Open();
    }

    private void CharactersButton()
    {
        mainScreen.SetActive(false);
        charactersScreen.SetActive(true);
    }

    private void BackToMainButton()
    {
        mainScreen.SetActive(true);
        charactersScreen.SetActive(false);
    }
}
