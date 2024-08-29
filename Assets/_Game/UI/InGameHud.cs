using DG.Tweening;
using TMPro;
using UnityEngine;

public class InGameHud : MonoBehaviour
{
    [SerializeField] private RectTransform scoreLayout;

    [SerializeField] private TextMeshProUGUI scoreAmount, multiAmount, maxScoreText, coinsAmount;

    [SerializeField] private Player player;

    private MenuHandler menu;

    private int maxScore;
    private int multi;


    public async void ShowHud(MenuHandler menu = null)
    {
        scoreLayout.anchoredPosition = new Vector2(-1000, -210);

        if (menu != null)
            this.menu = menu;

        gameObject.SetActive(true);
        maxScore = await menu.GetMaxScore();
        multi = await menu.GetCurrentMultiplier();


        multiAmount.text = multi.ToString();
        maxScoreText.text = "record: "+maxScore.ToString();
        scoreLayout.DOAnchorPosX(0, 1f);
    }

    private void Update()
    {
        scoreAmount.text = ((int)player.transform.position.z * multi).ToString();
        coinsAmount.text = $"<sprite=0> {player.AddCoin}";
    }

    public void HideHud()
    {
        scoreLayout.DOAnchorPosX(-1000, 1f).OnComplete(() => gameObject.SetActive(false));
    }
}