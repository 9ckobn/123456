using TMPro;
using UnityEngine;

public class ProfileScreen : DefaultMenuScreen
{
    [SerializeField] private TextMeshProUGUI maxScoreTmp, coinsAmountTmp, multiplierTmp;

    [SerializeField] private Button openFriendsInviteScreen;


    public override async void OpenScreenLazy()
    {
        openFriendsInviteScreen.onClick = menu.OpenInviteFriends;

        base.OpenScreenLazy();

        maxScoreTmp.text = $"max score\n loading...";
        coinsAmountTmp.text = $"loading...";
        multiplierTmp.text = $"loading...";

        int multi = await menu.GetCurrentMultiplier();
        int maxScore = await menu.GetMaxScore();
        int coins = await menu.GetCurrentCoins();

        maxScoreTmp.text = $"max score\n{maxScore}";
        coinsAmountTmp.text = $"{coins}";
        multiplierTmp.text = $" Level x{multi}";
    }
}