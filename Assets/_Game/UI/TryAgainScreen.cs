using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TryAgainScreen : Screen
{
    public Action onTimeEnded;
    public Image timerImage;

    public Button saveMe;

    public override void OpenScreenLazy()
    {
        OpenScreen();
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