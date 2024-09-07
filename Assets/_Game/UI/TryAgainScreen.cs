using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TryAgainScreen : Screen
{
    public Action onTimeEnded;
    public Image timerImage;

    [SerializeField] private TextMeshProUGUI scoreAmountTmpro, coinsAmountTmpro;

    public Button saveMe;

    public override void OpenScreenLazy()
    {
        OpenScreen();
    }

    public void SetupScreen(int coins, int score, Action onTimeEnded, Action<int> onDecreaseMoney, Action onPlayerSave)
    {
        scoreAmountTmpro.text = score.ToString();
        coinsAmountTmpro.text = coins.ToString();
        this.onTimeEnded = onTimeEnded;

        if (onDecreaseMoney == null)
        {
            CloseScreen();
            this.onTimeEnded?.Invoke();
        }
        else
        {
            OpenScreen();
            saveMe.onClick = () =>
            {
                onDecreaseMoney.Invoke(300);
                CloseScreen();
                onPlayerSave?.Invoke();
            };
        }
    }

    public override void CloseScreen()
    {
        gameObject.SetActive(false);
    }

    public override Screen OpenScreen()
    {
        transform.localScale = Vector3.zero;

        gameObject.SetActive(true);

        transform.DOScale(1, 0.3f);

        StartCoroutine(Timer());

        return this;
    }

    private IEnumerator Timer()
    {
        timerImage.fillAmount = 0;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / 3;

            timerImage.fillAmount = t;

            yield return null;
        }

        CloseScreen();
        onTimeEnded?.Invoke();
    }
}