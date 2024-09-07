using DG.Tweening;
using TMPro;
using UnityEngine;

public class InGameHud : MonoBehaviour
{
    [SerializeField] private RectTransform scoreLayout;

    [SerializeField] private TextMeshProUGUI scoreAmount, maxScoreText, coinsAmount;

    [SerializeField] private Player player;

    private MenuHandler menu;

    private int maxScore;
    private int multi;


    public async void ShowHud(MenuHandler menu = null)
    {
        scoreLayout.anchoredPosition = new Vector2(-1300, -300);

        if (menu != null)
            this.menu = menu;

        gameObject.SetActive(true);
        maxScore = await menu.GetMaxScore();
        multi = menu.GetUserMulti;

        maxScoreText.text = "record: " + maxScore.ToString();
        scoreLayout.DOAnchorPosX(0, 1f);
    }

    private void LateUpdate()
    {
        scoreAmount.text = $"{Mathf.FloorToInt(player.transform.position.z * multi)}";
        coinsAmount.text = $"{player.AddCoin}";
    }

    public void HideHud()
    {
        scoreLayout.DOAnchorPosX(-1300, 1f).OnComplete(() => gameObject.SetActive(false));
    }
}