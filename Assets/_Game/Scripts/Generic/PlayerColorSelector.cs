using UnityEngine;

public class PlayerColorSelector : MonoBehaviour
{
    public bool useSecondaryColor = false;
    public Renderer playerRenderer;
    public Color[] playerColors;
    private static int currentColorIndex = 0;

    private const string SELECTED_COLOR_INDEX_KEY = "selected_color_index";

    private void Awake()
    {
        LoadSelectedColorIndex();
        if (playerRenderer != null)
            playerRenderer.material.color = !useSecondaryColor ? GetCurrentColor() : GetAnalogColor();
    }

    public void NextColor()
    {
        currentColorIndex++;
        if (currentColorIndex >= playerColors.Length)
            currentColorIndex = 0;

        playerRenderer.material.color = !useSecondaryColor ? GetCurrentColor() : GetAnalogColor();
        SaveSelectedColorIndex();
    }

    public void PreviousColor()
    {
        currentColorIndex--;
        if (currentColorIndex < 0)
            currentColorIndex = playerColors.Length - 1;

        playerRenderer.material.color = !useSecondaryColor ? GetCurrentColor() : GetAnalogColor();
        SaveSelectedColorIndex();
    }

    public Color GetCurrentColor()
    {
        return playerColors[currentColorIndex];
    }

    public Color GetAnalogColor()
    {
        int analogColorIndex = currentColorIndex + playerColors.Length / 2;
        if (analogColorIndex >= playerColors.Length)
            analogColorIndex -= playerColors.Length;

        return playerColors[analogColorIndex];
    }

    private void SaveSelectedColorIndex()
    {
        PlayerPrefs.SetInt(SELECTED_COLOR_INDEX_KEY, currentColorIndex);
    }

    private void LoadSelectedColorIndex()
    {
        if (PlayerPrefs.HasKey(SELECTED_COLOR_INDEX_KEY))
        {
            currentColorIndex = PlayerPrefs.GetInt(SELECTED_COLOR_INDEX_KEY);
        }
    }
}
