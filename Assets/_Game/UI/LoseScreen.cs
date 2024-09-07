using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class LoseScreen : Screen
{
    [SerializeField] private UnityEngine.UI.Button goToStartMenuButton;

    [SerializeField] private TextMeshProUGUI scoreText, coinsText;
    [SerializeField] private RectTransform playerIcon;

    private int coinsAmount, scoreAmount;

    [SerializeField] private LoadingScreen loadingScreen;

    public override void OpenScreenLazy()
    {
        OpenScreen();
    }

    public override void CloseScreen()
    {
        scoreText.text = $"Score: {0}";
        coinsText.text = $"Coins: {0}";
        gameObject.SetActive(false);
    }

    public void SetupScreen(int coins, int score, Action onClick)
    {
        coinsAmount = coins;
        scoreAmount = score;

        loadingScreen.instantOpen = false;
        goToStartMenuButton.onClick.AddListener(() =>
        {
            loadingScreen.OpenScreen(onClick);
            goToStartMenuButton.onClick.RemoveAllListeners();
        });

        playerIcon.anchoredPosition = new Vector2(-3000, 0);
        playerIcon.DOAnchorPosX(0, 2f);
    }

    public override Screen OpenScreen()
    {
        transform.localScale = Vector3.zero;

        gameObject.SetActive(true);

        transform.DOScale(1, 0.3f);

        StartCoroutine(CounterAnimation());

        return this;
    }

    private IEnumerator CounterAnimation()
    {
        scoreText.text = $"Score: {0}";
        coinsText.text = $"Coins: {0}";

        yield return new WaitForSeconds(1);

        float t = 0;
        float duration = 1;

        float currentCoins = 0;
        float currentScore = 0;

        while (t < duration)
        {
            currentCoins = (int)Mathf.Lerp(0, coinsAmount, t / duration);
            currentScore = (int)Mathf.Lerp(0, scoreAmount, t / duration);

            scoreText.text = $"Score: {currentScore}";
            coinsText.text = $"Coins: {currentCoins}";

            t += Time.deltaTime;

            yield return null;
        }

        scoreText.text = $"Score: {scoreAmount}";
        coinsText.text = $"Coins: {coinsAmount}";
    }
}