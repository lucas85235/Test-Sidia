using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class Popup : MonoBehaviour
{
    [SerializeField] private Ease openingEase = Ease.OutBack;
    [SerializeField] private Ease closingEase = Ease.InBack;
    [SerializeField] private Image blackBackground;
    [SerializeField] private GameObject popup;
    [SerializeField] private bool hasBlackBackground;

    private AppSettings _settings;

    private void Awake()
    {
        if (_settings == null)
        {
            _settings = Instantiate(Resources.Load<AppSettings>("AppSettings"));
        }
    }

    public void Open()
    {
        popup.SetActive(true);
        Scale(false);

        if (hasBlackBackground)
        {
            blackBackground.color = Color.clear;
            blackBackground.gameObject.SetActive(true);
            StartCoroutine(LerpColor(blackBackground, blackBackground.color, _settings.blackBackgroundColor, 0.1f, true));
        }
    }

    public void Close()
    {
        if (hasBlackBackground)
        {
            StartCoroutine(LerpColor(blackBackground, blackBackground.color, Color.clear, 0.1f, false));
        }

        Scale(true);
    }

    private void DisableBackground()
    {
        blackBackground.gameObject.SetActive(false);
    }

    private void Scale(bool closing)
    {
        popup.SetActive(true);
        popup.transform.localScale = closing ? Vector3.one : Vector3.zero;
        var targetScale = closing ? Vector3.zero : Vector3.one;
        var easeing = closing ? closingEase : openingEase;
        var animationDuration = closing ? _settings.popupCloseAnimationTime : _settings.popupOpenAnimationTime;
        popup.transform.DOScale(targetScale, animationDuration).SetEase(easeing).OnComplete(() =>
        {
            if (closing)
            {
                popup.SetActive(false);
                popup.transform.localScale = Vector3.one;
            }
        });
    }

    private IEnumerator LerpColor(Image targetImage, Color fromColor, Color toColor, float duration, bool keepActive)
    {
        float counter = 0;
        targetImage.color = fromColor;
        while (counter < duration)
        {
            counter += Time.unscaledDeltaTime;
            targetImage.color = Color.Lerp(fromColor, toColor, counter / duration);
            yield return null;
        }
        if (!keepActive)
        {
            targetImage.gameObject.SetActive(false);
        }
    }
}
