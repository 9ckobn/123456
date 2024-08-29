using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FriendsLeadersScreen : DefaultMenuScreen
{
    [SerializeField] private LeaderScreen friendsInactive, friendsActive;

    [SerializeField] private Button inviteFriendsButton;

    [SerializeField] private LeaderScreen coinLeaders, scoreLeaders;

    [SerializeField] private SwitchButton friendSwitch, coinLeaderSwitch, scoreLeaderSwitch;

    private LeaderScreen activeScreen = null;

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
                activeScreen = friendsInactive;
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