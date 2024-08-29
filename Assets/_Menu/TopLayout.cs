using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TopLayout : MonoBehaviour
{
    [SerializeField] private Button addCoins;
    [SerializeField] private TextMeshProUGUI moneyCounter;

    [SerializeField] private Button settingsButton;
    [SerializeField] private Screen settingsScreen;

    private int coinsAmount = 0;

    public async void Setup(MenuHandler menu)
    {
        moneyCounter.text = "Loading...";

        coinsAmount = await menu.GetCurrentCoins();

        moneyCounter.text = $"<sprite=0>{coinsAmount}";
    }

    public void UpdateMoneyCount(int amount)
    {
        StartCoroutine(CounterAnimation(amount));
    }

    private IEnumerator CounterAnimation(int totalCoins)
    {
        moneyCounter.text = $"<sprite=0>{coinsAmount}";

        yield return new WaitForSeconds(1);

        float t = 0;
        float duration = 1;

        float currentCoins = coinsAmount;

        while (t < duration)
        {
            currentCoins = (int)Mathf.Lerp(coinsAmount, coinsAmount + totalCoins, t / duration);
            moneyCounter.text = $"<sprite=0>{currentCoins}";

            t += Time.deltaTime;

            yield return null;
        }

        moneyCounter.text = $"<sprite=0>{ coinsAmount + totalCoins}";
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