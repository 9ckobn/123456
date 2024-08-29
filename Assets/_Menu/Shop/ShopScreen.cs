using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ShopScreen : DefaultMenuScreen
{
    [SerializeField] private Button RestoreEnergy;

    [SerializeField] private TextMeshProUGUI statusText;

    public override void OpenScreenLazy()
    {
        statusText.color = new Color(1, 1, 1, 0);
        statusText.rectTransform.anchoredPosition = Vector2.zero;

        base.OpenScreenLazy();
        RestoreEnergy.onClick = () => Buy(menu.RestoreEnergy);
    }

    private async void Buy(Action onBuyComplete)
    {
        int coinsAmount = await menu.GetCurrentCoins();

        if (coinsAmount - 500 < 0)
        {
            ShowStatus(false);
            return;
        }

        menu.DecreaseCoins(500);

        onBuyComplete?.Invoke();
        ShowStatus(true);
    }

    private void ShowStatus(bool status)
    {
        statusText.text = status? "Successful" : "Error...";
        StatusAnimation();
    }

    private void StatusAnimation()
    {
        var seq = DOTween.Sequence();
        
        statusText.rectTransform.anchoredPosition = Vector2.zero;

        seq.Append(statusText.DOFade(1, 1f));
        seq.Insert(0, statusText.rectTransform.DOAnchorPosY(375, 2f)).OnComplete(() => statusText.enabled = false);
        seq.Insert(1, statusText.DOFade(0, 1f));


        statusText.enabled = true;
        seq.Play();
    }
}