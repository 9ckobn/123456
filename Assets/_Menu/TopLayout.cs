using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TopLayout : MonoBehaviour
{
    [SerializeField] private Button addCoins;
    [SerializeField] private TextMeshProUGUI moneyCounter, multiCounter;

    [SerializeField] private Button settingsButton;
    [SerializeField] private DefaultMenuScreen settingsScreen;

    [SerializeField] private Button showEnergyInfoButton;
    [SerializeField] private EnergyInfo energyInfo;

    private int coinsAmount = 0;

    public void Setup(MenuHandler menu)
    {
        moneyCounter.text = "Loading...";
        multiCounter.text = "Loading...";

        settingsButton.onClick = () => menu.OpenScreen(settingsScreen);

        energyInfo.GetRect.sizeDelta = new Vector2(0, 100);

        showEnergyInfoButton.onClick = async () =>
        {
            energyInfo.GetRect.sizeDelta = new Vector2(0, 100);
            energyInfo.GetRect.anchoredPosition = Vector2.zero;
            energyInfo.GetRect.DOSizeDelta(new Vector2(600, 100), 0.3f);
            energyInfo.GetRect.DOAnchorPosX(300, 0.3f);

            energyInfo.UpdateEnergy(menu.GetUserEnergy);
            showEnergyInfoButton.interactable = false;
            await UniTask.Delay(1600);
            showEnergyInfoButton.interactable = true;
        };

        coinsAmount = menu.GetUserCoins;

        UpdateMulti(menu.GetUserMulti);
        moneyCounter.text = $"{coinsAmount} <sprite=0>";
    }

    public void UpdateMoneyCount(int amount)
    {
        StartCoroutine(CounterAnimation(amount));
    }

    public void UpdateMulti(int amount)
    {
        multiCounter.text = $"X{amount}";
    }

    private IEnumerator CounterAnimation(int totalCoins)
    {
        moneyCounter.text = $"{coinsAmount} <sprite=0>";

        yield return new WaitForSeconds(1);

        float t = 0;
        float duration = 1;

        float currentCoins = coinsAmount;

        while (t < duration)
        {
            currentCoins = (int)Mathf.Lerp(coinsAmount, coinsAmount + totalCoins, t / duration);
            moneyCounter.text = $"{currentCoins} <sprite=0>";

            t += Time.deltaTime;

            yield return null;
        }

        moneyCounter.text = $"{coinsAmount + totalCoins} <sprite=0>";
        coinsAmount += totalCoins;
    }

    public void ShowLayout()
    {
        GetComponent<RectTransform>().DOAnchorPosY(0, 0.5f);
    }

    public void HideLayout()
    {
        GetComponent<RectTransform>().DOAnchorPosY(3000, 0.5f);
    }
}