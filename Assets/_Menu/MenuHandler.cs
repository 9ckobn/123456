using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TopLayout topLayout;
    [SerializeField] private BottomLayout bottomLayout;

    [SerializeField] private DefaultMenuScreen inviteFriendsScreen;

    [SerializeField] private EnergyInfo energyInfo;

    [SerializeField] private LevelHandler levelHandler;

    private Screen currentScreen, pastScreen;

    private User currentUser;
    private API client;

    public Action<Screen> onScreenChanged;

    [SerializeField] private Image clickablePanel;
    public InGameHud GameHud;
    public bool canPlay;

    public long GetUID
    {
        get => currentUser.UID;
    }

    public void GetReward(int amount)
    {
        Debug.Log($"Yeeapy, your reward is {amount}");
    }

    public async void StartMenu(User user = null)
    {
        GameHud.HideHud();

        client = new();

        if (user != null)
        {
            currentUser = user;
        }

        bottomLayout.Setup(this);
        topLayout.Setup(this);

        bottomLayout.ShowLayout();
        topLayout.ShowLayout();

        energyInfo.UpdateEnergy(await client.GetUserParamByNameAsync(GetUID, "currentEnergy"));
        clickablePanel.enabled = true;
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

        onScreenChanged?.Invoke(currentScreen);
    }

    public void OpenInviteFriends()
    {
        inviteFriendsScreen.SetupMenu(this);
        inviteFriendsScreen.SetupCloseScreen(() => canPlay = true);
        inviteFriendsScreen.OpenScreen();
    }

    private void OnDestroy()
    {
        onScreenChanged = null;
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
        Debug.Log("getting last score");
        return await client.GetUserParamByNameAsync(GetUID, "lastSessionScore");
    }

    public async UniTask<int> GetLastSessionCoins()
    {
        Debug.Log("getting last coins");
        return await client.GetUserParamByNameAsync(GetUID, "lastSessionCoins");
    }

    // public async UniTask<List<(User user, int rank)>> GetTopUsersByMaxScore()
    // {
    //     return await client.GetTopUsersByMaxScoreAsync();
    // }

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

    //todo remake all requests to creating one json and sending them (now is sending 3 requests with different json's)
    public async UniTask IncreaseMultiplier()
    {
        await client.UpdateUserDataByParamAsync(GetUID, "lastSessionCoins", 0);
        await client.UpdateUserDataByParamAsync(GetUID, "lastSessionScore", 0);

        await client.UpdateAllUserTasksAsync(GetUID, new CurrentTasks());

        await client.UpdateUserDataByParamAsync(GetUID, "multiplier", currentUser.multiplier + 1);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("point");
        
        if (canPlay)
            Play();
    }

    private async void Play()
    {
        if (await DecreaseEnergy())
        {
            bottomLayout.HideLayout();
            topLayout.HideLayout();
            inviteFriendsScreen.CloseScreen();
            clickablePanel.enabled = false;
            Debug.Log("start running");

            levelHandler.Play();

            await UniTask.Delay(1000);

            GameHud.ShowHud(this);
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

        energyInfo.UpdateEnergy(updatedEnergy);

        return true;
    }

    public async void RestoreEnergy()
    {
        await client.UpdateUserDataByParamAsync(GetUID, "currentEnergy", 1000);
        energyInfo.UpdateEnergy(1000);
    }

    public async void DecreaseCoins(int amount)
    {
        topLayout.UpdateMoneyCount(-amount);
        await client.UpdateUserDataByParamAsync(GetUID, "coins", await GetCurrentCoins() - amount);
    }
}