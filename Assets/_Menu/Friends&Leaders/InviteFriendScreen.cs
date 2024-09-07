using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class InviteFriendScreen : DefaultMenuScreen
{
    [SerializeField] private Button InviteFreeFriend, InvitePremiumFriend;

    private void OnEnable()
    {
        CheckMyReferrals();

        InviteFreeFriend.onClick = SetAndShareInviteUrl;
        InvitePremiumFriend.onClick = SetAndShareInviteUrl;
    }

    private async void CheckMyReferrals()
    {
        var client = new API();
        List<ReferralFriend> currentReferrals = await client.GetReferralFriendsAsync(menu.GetUID);
        int noRewardedReferrals = 0;

        List<ReferralFriend> updatedReferrals = new List<ReferralFriend>();

        foreach (var friend in currentReferrals)
        {
            if (!friend.WasRewarded)
            {
                var updatedFriend = friend;

                updatedFriend.WasRewarded = true;
                updatedReferrals.Add(updatedFriend);
                noRewardedReferrals++;
            }
        }

        if (noRewardedReferrals != 0)
        {
            Debug.Log($"{noRewardedReferrals} times you get your reward! Updating list...");
            await client.AddReferralFriendsListAsync(menu.GetUID, updatedReferrals);
        }
    }

    private void SetAndShareInviteUrl()
    {
        string referralUrl = $"https://t.me/CatFlipPlay_Bot/gameapp?startapp=referral_{menu.GetUID}";
        string welcomeMessage =
            "\ud83d\ude3b Catflip: Unleash, Play, Earn - Where Every Game Leads to an Airdrop Adventure!\n\ud83d\ude80 Let's play-to-earn airdrop right now!";
        string encodedMessage = Uri.EscapeDataString(welcomeMessage);
        string url = $"https://t.me/share/url?url={referralUrl}&text={encodedMessage}";

#if UNITY_WEBGL && !UNITY_EDITOR
        ShareInviteLink(url);
#else
        Application.OpenURL(url);
#endif
    }

    [DllImport("__Internal")]
    private static extern void ShareInviteLink(string url);
}