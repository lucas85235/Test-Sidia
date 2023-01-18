using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;

    private void Start()
    {
        startButton.onClick.AddListener(StartButton);
    }

    private void StartButton()
    {
        SceneLoader.LoadScene("Game");
    }
}
