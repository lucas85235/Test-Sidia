using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDCanvas : MonoBehaviour
{
    [Header("Game Over")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private GameObject gameOverPopup;

    public static HUDCanvas Instance;

    private void Awake()
    {
        Instance = this;
        restartButton.onClick.AddListener(RestartButton);
        menuButton.onClick.AddListener(MenuButton);
    }

    public void OpenGameOverPopup(string winnerMsg)
    {
        winnerText.text = winnerMsg;
        gameOverPopup.SetActive(true);
    }

    private void RestartButton()
    {
        SceneLoader.LoadScene("Game");
    }

    private void MenuButton()
    {
        SceneLoader.LoadScene("MainMenu");
    }
}
