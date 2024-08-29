using System;
using System.Collections.Generic;

[Serializable]
public class User
{
    public long UID { get; private set; }

    public string Nickname { get; private set; }

    public UserInventory Inventory { get; private set; }

    public int maxScore { get; set; }
    public int coins { get; set; }

    public int lastSessionScore;
    public int lastSessionCoins;

    public int multiplier = 1;

    public CurrentTasks CurrentTasks;

    public int currentEnergy = 1000;

    public List<ReferralFriend> ReferralFriends = new();
    public List<string> AlreadyDoneUrls = new();

    public bool IsPremium;

    public bool CanBeReferral = true;

    public User(long uid, string nickname, bool isPremium, bool canBeReferral)
    {
        UID = uid;
        Nickname = nickname;
        IsPremium = isPremium;
        CanBeReferral = canBeReferral;
        
        Inventory = new();
    }
}

[Serializable]
public struct ReferralFriend
{
    public ReferralFriend(long uid, bool isPremium)
    {
        UID = uid;
        WasRewarded = false;
        IsPremium = isPremium;
    }

    public long UID;
    public bool WasRewarded;
    public bool IsPremium;
}

[Serializable]
public struct UserInventory
{
    public int hoverBoard;
    public int x2;
}

[Serializable]
public struct CurrentTasks
{
    public bool task1;
    public bool task2;
    public bool task3;
}