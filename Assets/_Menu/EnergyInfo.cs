using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyInfo : MonoBehaviour
{
    private int currentEnergy = 1000;

    [SerializeField] private TextMeshProUGUI text;

    private RectTransform _rectTransform;

    public RectTransform GetRect
    {
        get => _rectTransform;
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        text.text = "Loading...";
    }

    public async void UpdateEnergy(int amount)
    {
        currentEnergy = amount;
        text.text = $"{currentEnergy} / 1000";
        text.DOFade(1, 0.1f);

        await UniTask.Delay(1500);


        text.DOFade(0, 0.1f);
        GetRect.DOSizeDelta(new Vector2(0, 100), 0.3f);
        GetRect.DOAnchorPosX(0, 0.3f).OnComplete(() =>
        {
            GetRect.sizeDelta = new Vector2(0, 100);

            // GetRect.anchoredPosition += new Vector2(300, 0);
        });
    }
}