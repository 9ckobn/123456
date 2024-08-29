using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LoseScreen : Screen
{
    [SerializeField] private Camera uiCamera;
    private Camera mainCamera;

    [SerializeField] private UnityEngine.UI.Button goToStartMenuButton;

    [SerializeField] private TextMeshProUGUI scoreText, coinsText;

    private int coinsAmount, scoreAmount;

    [SerializeField] private LoadingScreen loadingScreen;

    public override void OpenScreenLazy()
    {
        OpenScreen();
    }

    public override void CloseScreen()
    {
        scoreText.text = $"{0}";
        coinsText.text = $"{0}";
        gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        uiCamera.gameObject.SetActive(false);
    }

    public void SetupScreen(int coins, int score, Action onClick)
    {
        mainCamera = Camera.main;
        
        coinsAmount = coins;
        scoreAmount = score;

        loadingScreen.instantOpen = false;
        goToStartMenuButton.onClick.AddListener(() => loadingScreen.OpenScreen(onClick));
    }

    public override Screen OpenScreen()
    {
        transform.localScale = Vector3.zero;
        mainCamera.gameObject.SetActive(false);
        uiCamera.gameObject.SetActive(true);

        gameObject.SetActive(true);

        transform.DOScale(1, 0.3f);

        StartCoroutine(CounterAnimation());

        return this;
    }

    private IEnumerator CounterAnimation()
    {
        scoreText.text = $"{0}";
        coinsText.text = $"{0}";

        yield return new WaitForSeconds(1);

        float t = 0;
        float duration = 1;

        float currentCoins = 0;
        float currentScore = 0;

        while (t < duration)
        {
            currentCoins = (int)Mathf.Lerp(0, coinsAmount, t / duration);
            currentScore = (int)Mathf.Lerp(0, scoreAmount, t / duration);

            scoreText.text = $"{currentScore}";
            coinsText.text = $"{currentCoins}";

            t += Time.deltaTime;

            yield return null;
        }

        scoreText.text = $"{scoreAmount}";
        coinsText.text = $"{coinsAmount}";
    }
}