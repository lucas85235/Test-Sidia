using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDCanvas : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private PlayerHUD player1;
    [SerializeField] private PlayerHUD player2;
    [SerializeField] private HighlightText turnText;
    [SerializeField] private HighlightText turnAttacksText;

    [Header("Game Over")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private Popup gameOverPopup;

    [Header("Other")]
    [SerializeField] private FloatingTextRect alertText;
    [SerializeField] private Popup pausePopup;

    public PlayerHUD Player1 { get => player1; }
    public PlayerHUD Player2 { get => player2; }

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
        gameOverPopup.Open();
    }

    public void RestartButton()
    {
        SceneLoader.LoadScene("Game");
    }

    public void MenuButton()
    {
        SceneLoader.LoadScene("MainMenu");
    }

    public void UpdateTurn(int turn)
    {
        turnText.text = $"Turn: {turn}";
    }

    public void UpdateTurnAttacks(int turnAttacks)
    {
        turnAttacksText.text = $"Attacks in turn: {turnAttacks}";
    }

    public void AlertText(string message)
    {
        alertText.Init(message);
    }

    public void PauseMenu()
    {
        pausePopup.Open();
    }

    [System.Serializable]
    public class PlayerHUD
    {
        [SerializeField] private HighlightText life;
        [SerializeField] private HighlightText attack;
        [SerializeField] private HighlightText moves;
        [SerializeField] private HighlightText dices;

        public void UpdateLife(int lifeAmount, bool invert = false)
        {
            life.text = !invert ? $"Life: {lifeAmount}" : $"{lifeAmount} :Life";
        }

        public void UpdateAttack(int attackAmount, bool invert = false)
        {
            attack.text = !invert ? $"Attack: {attackAmount}" : $"{attackAmount} :Attack";
        }

        public void UpdateMoves(int movesAmount, int maxMoves, bool invert = false)
        {
            moves.text = !invert ? $"Moves: {movesAmount}/{maxMoves}" : $"{movesAmount}/{maxMoves} :Moves";
        }

        public void UpdateDices(int dicesAmount, bool invert = false)
        {
            dices.text = !invert ? $"Dices: {dicesAmount}" : $"{dicesAmount} :Dices";
        }
    }
}
