using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class TaskScreen : DefaultMenuScreen
{
    [Range(1, 30)] private int currentLevel = 1;
    [SerializeField] private List<Task> currentTasks = new List<Task>();
    [SerializeField] private RectTransform taskRoot;
    [SerializeField] private TaskElement defaultPrefab;
    [SerializeField] private TaskElement prefabWithButton;

    private TaskElement prefab;
    [SerializeField] private Image loadingImage;
    [SerializeField] private GameObject levelUpPopUp;
    [SerializeField] private TextMeshProUGUI levelsText;

    private const string TasksKey = "SavedTasks";
    private int multiplier;
    private List<Task> currentActiveTasks = new List<Task>();
    private bool levelTask = true;

    private User currentUser;
    private API client;

    [ContextMenu("SwitchTasks")]
    public void SwitchRoot()
    {
        SwitchRootTasks(!levelTask);
    }

    public async void SwitchRootTasks(bool isLevelTasks)
    {
        multiplier = await menu.GetCurrentMultiplier();
        levelTask = isLevelTasks;

        ClearExistingTaskPrefabs();

        if (levelTask)
        {
            await SetupAndInitializeGameTasks();
        }
        else
        {
            await LoadExternalTasksAndInitialize();
        }
    }

    public async UniTask SetupAndInitializeGameTasks()
    {
        multiplier = await menu.GetCurrentMultiplier();
        Debug.Log(multiplier);
        
        currentTasks = LoadTasks();
        if (!currentTasks.Any()) GenerateNewTasks();
        
        var activeTaskStatus = await menu.GetActiveTasks();
        UpdateTaskStatus(activeTaskStatus);

        InitializeActiveTasks();
        
        await UpdateTasksProgress();
    }

    private async UniTask LoadExternalTasksAndInitialize()
    {
        currentUser = await client.GetUserAsync(menu.GetUID);

        var watchUrls = await FetchFilteredUrlsAsync("watchurl", currentUser.AlreadyDoneUrls);
        var subscribeUrls = await FetchFilteredUrlsAsync("subscribeurl", currentUser.AlreadyDoneUrls);

        currentTasks = watchUrls.Select(url => GenerateExternalTask(TaskType.WatchUrl, url))
            .Concat(subscribeUrls.Select(url => GenerateExternalTask(TaskType.SubscribeUrl, url)))
            .ToList();

        InitializeActiveTasks();
    }

    public List<Task> LoadTasks()
    {
        string json = PlayerPrefs.GetString(TasksKey, "[]");
        return JsonConvert.DeserializeObject<List<Task>>(json);
    }

    private async UniTask<string[]> FetchFilteredUrlsAsync(string endpoint, List<string> alreadyDoneUrls)
    {
        var allUrls = await client.GetUrlsAsync(endpoint);
        return allUrls.Except(alreadyDoneUrls).ToArray();
    }

    public override async void OpenScreenLazy()
    {
        client = new API();

        base.OpenScreen();
        ShowLoadingAnimation();

        ClearExistingTaskPrefabs();

        if (levelTask)
        {
            await SetupAndInitializeGameTasks();
        }
        else
        {
            await LoadExternalTasksAndInitialize();
        }

        HideLoadingAnimation();

        if (currentActiveTasks.All(x => x.IsCompleted))
        {
            await HandleLevelUp();
            await SetupAndInitializeGameTasks();
        }

        taskRoot.ForceUpdateRectTransforms();
        
    }
    
    private async UniTask UpdateTasksProgress()
    {
        Debug.Log("start checking");
        
        foreach (var task in currentActiveTasks)
        {
            Debug.Log(task.Type);
            
            if (task.IsCompleted) continue;

            int value = task.Type switch
            {
                TaskType.CollectXCoins => await menu.GetLastSessionCoins(),
                TaskType.RunXMeters => await menu.GetLastSessionScore(),
                _ => 0
            };

            Debug.Log(value);
            Debug.Log(task.TaskText);
            
            if (task.action.CheckForCompletion(value))
            {
                CompleteTask(task);
            }
        }
    }
    
    private void CompleteTask(Task task)
    {
        task.IsCompleted = true;
        task.OnComplete?.Invoke();

        int taskIndex = currentActiveTasks.IndexOf(task);
        menu.UpdateTaskCompletion(taskIndex, task.RewardAmount);
    }

    private void ClearExistingTaskPrefabs()
    {
        foreach (Transform child in taskRoot)
        {
            Destroy(child.gameObject);
        }
        currentActiveTasks.Clear();
    }

    private async UniTask HandleLevelUp()
    {
        PlayerPrefs.SetString(TasksKey, "[]");
        
        await menu.IncreaseMultiplier();
        multiplier++;
        UpdateLevelText();
        // multiplier = await menu.GetCurrentMultiplier();
        currentActiveTasks.Clear();
        levelUpPopUp.transform.localScale = Vector3.zero;
        levelUpPopUp.SetActive(true);

        var seq = DOTween.Sequence();
        seq.Append(levelUpPopUp.transform.DOScale(Vector3.one, 1))
            .Append(levelUpPopUp.transform.DOScale(Vector3.zero, 0.3f));

        await seq.Play().AsyncWaitForCompletion().AsUniTask();
    }

    private void GenerateNewTasks()
    {
        currentTasks = Enumerable.Range(0, 3)
            .Select(_ => GenerateTask(multiplier))
            .ToList();

        SaveTasks(currentTasks);
    }
    
    public void SaveTasks(List<Task> tasks)
    {
        string json = JsonConvert.SerializeObject(tasks);
        PlayerPrefs.SetString(TasksKey, json);
        PlayerPrefs.Save();
    }

    private void ShowLoadingAnimation()
    {
        loadingImage.rectTransform.DOKill();
        loadingImage.enabled = true;
        loadingImage.rectTransform.DORotate(new Vector3(90, 0, 0), 1)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
    }

    private void HideLoadingAnimation()
    {
        loadingImage.rectTransform.DOKill();
        loadingImage.enabled = false;
    }

    private void UpdateTaskStatus(CurrentTasks dict)
    {
        // Удаляем выполненные задачи
        if (dict.task1) currentTasks[0] = null;
        if (dict.task2) currentTasks[1] = null;
        if (dict.task3) currentTasks[2] = null;
    }

    private void InitializeActiveTasks()
    {
        foreach (var task in currentTasks)
        {
            if (task == null) continue;

            currentActiveTasks.Add(task);

            prefab = task.Type == TaskType.SubscribeUrl || task.Type == TaskType.WatchUrl
                ? prefabWithButton
                : defaultPrefab;

            var go = Instantiate(prefab, taskRoot);
            go.transform.localScale = Vector3.zero;
            go.Setup(ReplaceX(task.TaskText, task.action.totalValue), task.RewardAmount);
            go.transform.DOScale(Vector3.one, 0.5f);

            task.OnComplete = async () =>
            {
                await go.HideTask();
                menu.GetReward(task.RewardAmount);
                RemoveTask(task);
                if (!levelTask) await client.AddUrlToUserAsync(menu.GetUID, task.TaskUrl);
            };
        }
    }

    private void RemoveTask(Task task)
    {
        currentActiveTasks.Remove(task);
        currentTasks.Remove(task);
        task.OnComplete = null;
    }

    private Task GenerateTask(int multi)
    {
        Random random = new Random();
        TaskType randomType = (TaskType)random.Next(Enum.GetValues(typeof(TaskType)).Length - 2);

        return new Task
        {
            Type = randomType,
            action = randomType switch
            {
                TaskType.CollectXCoins => new CollectXCoins(multi),
                TaskType.RunXMeters => new RunXMeters(multi),
                _ => throw new ArgumentOutOfRangeException()
            },
            TaskText = $"{randomType}",
            IsCompleted = false,
            RewardAmount = random.Next(10, 100)
        };
    }

    private Task GenerateExternalTask(TaskType type, string url)
    {
        return new Task
        {
            Type = type,
            action = type switch
            {
                TaskType.WatchUrl => new WatchUrl(url),
                TaskType.SubscribeUrl => new SubscribeUrl(url),
                _ => throw new ArgumentOutOfRangeException()
            },
            TaskText = $"{type}: {url}",
            IsCompleted = false,
            RewardAmount = 50
        };
    }

    private void UpdateLevelText()
    {
        levelsText.text = $"{multiplier}->{multiplier + 1} level Tasks";
    }

    private string ReplaceX(string input, object value)
    {
        return input.Replace("X", $" <color=#AF67C8>{value}</color> ");
    }
}

[Serializable]
public class Task
{
    public string TaskText;
    public string TaskUrl;
    public TaskAction action;
    public TaskType Type;
    public Action OnComplete;
    public int RewardAmount;
    public bool IsCompleted;
}

public enum TaskType
{
    CollectXCoins,
    RunXMeters,
    WatchUrl,
    SubscribeUrl,
}
