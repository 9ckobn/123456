using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] private TopLayout topLayout;
    [SerializeField] private BottomLayout bottomLayout;

    [SerializeField] private DefaultMenuScreen inviteFriendsScreen;

    [SerializeField] private LevelHandler levelHandler;
    [SerializeField] private TextMeshProUGUI errorText;

    private Screen currentScreen, pastScreen;

    private User currentUser;
    private API client;

    [SerializeField] private Button playButton;
    public InGameHud GameHud;

    public long GetUID
    {
        get => currentUser.UID;
    }

    public int GetUserCoins
    {
        get => currentUser.coins;
    }

    public int GetUserMulti
    {
        get => currentUser.multiplier;
    }

    public int GetUserEnergy
    {
        get => currentUser.currentEnergy;
    }


    public async void StartMenu(User user = null)
    {
        playButton.gameObject.SetActive(true);
        playButton.onClick = Play;

        GameHud.HideHud();

        client = new();

        if (user != null)
        {
            currentUser = user;
        }
        else
        {
            await UpdateUserAsync(GetUID);
        }

        bottomLayout.Setup(this);
        topLayout.Setup(this);

        bottomLayout.ShowLayout();
        topLayout.ShowLayout();
    }


    public void OpenScreen(Screen screen)
    {
        if (currentScreen != null)
        {
            pastScreen = currentScreen;
            pastScreen.CloseScreen();
        }

        screen.OpenScreenLazy();
        currentScreen = screen;
    }

    public void OpenInviteFriends()
    {
        inviteFriendsScreen.SetupMenu(this);
        inviteFriendsScreen.SetupCloseScreen(null);
        inviteFriendsScreen.OpenScreen();
    }

    public async UniTask<User> UpdateUserAsync(long UID)
    {
        currentUser = await client.GetUserAsync(UID);

        return currentUser;
    }

    public async UniTask<CurrentTasks> GetActiveTasks()
    {
        return await client.GetUserCurrentTasksAsync(GetUID);
    }

    public async UniTask<int> GetCurrentMultiplier()
    {
        return await client.GetUserParamByNameAsync(GetUID, "multiplier");
    }

    public async UniTask<int> GetCurrentCoins()
    {
        return await client.GetUserParamByNameAsync(GetUID, "coins");
    }

    public async UniTask<int> GetMaxScore()
    {
        return await client.GetUserParamByNameAsync(GetUID, "maxScore");
    }

    public async UniTask<int> GetLastSessionScore()
    {
        return await client.GetUserParamByNameAsync(GetUID, "lastSessionScore");
    }

    public async UniTask<int> GetLastSessionCoins()
    {
        return await client.GetUserParamByNameAsync(GetUID, "lastSessionCoins");
    }

    public async UniTask<List<(User user, int rank)>> GetTopUsersByParamName(string paramName)
    {
        return await client.GetTopUsersByParamNameAsync(GetUID, paramName);
    }


    public async void UpdateTaskCompletion(int taskIndex, int rewardAmount)
    {
        await client.UpdateUserTaskAsync(GetUID, taskIndex, true);

        await client.UpdateUserDataByParamAsync(GetUID, "coins",
            await client.GetUserParamByNameAsync(GetUID, "coins") + rewardAmount);

        topLayout.UpdateMoneyCount(rewardAmount);
    }

    public async void UpdateTaskCompletion(int rewardAmount)
    {
        Debug.Log("Just get coins");

        await client.UpdateUserDataByParamAsync(GetUID, "coins",
            await client.GetUserParamByNameAsync(GetUID, "coins") + rewardAmount);

        topLayout.UpdateMoneyCount(rewardAmount);
    }

    //todo remake all requests to creating one json and sending them (now is sending 3 requests with different json's)
    public async UniTask IncreaseMultiplier()
    {
        await client.UpdateUserDataByParamAsync(GetUID, "lastSessionCoins", 0);
        await client.UpdateUserDataByParamAsync(GetUID, "lastSessionScore", 0);

        await client.UpdateAllUserTasksAsync(GetUID, new CurrentTasks());

        await client.UpdateUserDataByParamAsync(GetUID, "multiplier", currentUser.multiplier + 1);

        topLayout.UpdateMulti(currentUser.multiplier + 1);
    }

    public async UniTask<List<User>> GetFriendsAsync()
    {
        List<User> list = new List<User>();

        var me = await client.GetUserAsync(GetUID);

        foreach (var friend in me.ReferralFriends)
        {
            var user = await client.GetUserAsync(friend.UID);
            if (user != null)
                list.Add(user);
        }

        return list;
    }

    private async void Play()
    {
        if (await DecreaseEnergy())
        {
            playButton.gameObject.SetActive(false);

            bottomLayout.HideLayout();
            topLayout.HideLayout();
            inviteFriendsScreen.CloseScreen();

            levelHandler.Play();

            await UniTask.Delay(1000);

            GameHud.ShowHud(this);
        }
        else
        {
            errorText.rectTransform.DOKill();
            errorText.DOKill();
            errorText.color = Color.white;

            errorText.rectTransform.anchoredPosition = Vector2.zero;
            errorText.enabled = true;

            errorText.DOFade(1, 0.5f).OnComplete(() =>
            {
                errorText.DOFade(0, 3f).OnComplete(() => errorText.enabled = false);
            });

            errorText.rectTransform.DOAnchorPosY(errorText.rectTransform.anchoredPosition.y + 200, 3f);
            errorText.text = "Not enough energy...";
        }
    }

    private async UniTask<bool> DecreaseEnergy()
    {
        int currentEnergy = await client.GetUserParamByNameAsync(GetUID, "currentEnergy");

        if (currentEnergy - 75 < 0)
        {
            return false;
        }

        int updatedEnergy = currentEnergy - 75;

        await client.UpdateUserDataByParamAsync(GetUID, "currentEnergy", updatedEnergy);
        return true;
    }

    public async void RestoreEnergy()
    {
        await client.UpdateUserDataByParamAsync(GetUID, "currentEnergy", 1000);
    }

    public async void DecreaseCoins(int amount)
    {
        topLayout.UpdateMoneyCount(-amount);
        await client.UpdateUserDataByParamAsync(GetUID, "coins", await GetCurrentCoins() - amount);
    }
}