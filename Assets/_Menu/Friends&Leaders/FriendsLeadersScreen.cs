using UnityEngine;

public class FriendsLeadersScreen : DefaultMenuScreen
{
    [SerializeField] private FriendsScreen friendsScreen;

    [SerializeField] private Button inviteFriendsButton;

    [SerializeField] private LeaderScreen coinLeaders, scoreLeaders;

    [SerializeField] private SwitchCategoryButton friendSwitch, coinLeaderSwitch, scoreLeaderSwitch;

    private ICategoryScreen activeScreen = null;

    private enum screenType
    {
        friends,
        coin,
        score
    }

    public override void OpenScreenLazy()
    {
        SwitchScreen(screenType.friends);

        friendSwitch.onClick = () => SwitchScreen(screenType.friends);
        coinLeaderSwitch.onClick = () => SwitchScreen(screenType.coin);
        scoreLeaderSwitch.onClick = () => SwitchScreen(screenType.score);

        base.OpenScreenLazy();
    }

    private void SwitchScreen(screenType screen)
    {
        if (activeScreen != null)
            activeScreen.CloseScreen();

        friendSwitch.interactable = true;
        coinLeaderSwitch.interactable = true;
        scoreLeaderSwitch.interactable = true;

        switch (screen)
        {
            case screenType.friends:
                activeScreen = friendsScreen;
                inviteFriendsButton.onClick = menu.OpenInviteFriends;
                friendSwitch.interactable = false;
                activeScreen.EnableScreen();
                break;
            case screenType.coin:
                activeScreen = coinLeaders;
                coinLeaderSwitch.interactable = false;
                activeScreen.EnableScreen("coins");
                break;
            case screenType.score:
                activeScreen = scoreLeaders;
                scoreLeaderSwitch.interactable = false;
                activeScreen.EnableScreen("maxScore");
                break;
        }
    }
}