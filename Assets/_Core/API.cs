using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class API
{
    private readonly string databaseURL = "https://catflip-run-default-rtdb.europe-west1.firebasedatabase.app";

    private async UniTask<string> SendWebRequestAsync(string url, string method = "GET", string jsonData = null)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, method))
        {
            if (jsonData != null)
            {
                request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData))
                {
                    contentType = "application/json"
                };
            }

            request.downloadHandler = new DownloadHandlerBuffer();

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var operation = request.SendWebRequest();
            await operation.ToUniTask();

            stopwatch.Stop();

            Debug.Log($"WebRequest to {url} took {stopwatch.ElapsedMilliseconds} ms.");

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Request error: {request.error}\nURL: {url}");
                throw new Exception($"Request failed with error: {request.error}");
            }

            return request.downloadHandler.text;
        }
    }

    public async UniTask<User> CreateUserAsync(long userId, string nickname, bool isPremium, bool canBeReferral)
    {
        string url = $"{databaseURL}/users/{userId}.json";
        User user = new User(userId, nickname, isPremium, canBeReferral);
        string jsonData = JsonConvert.SerializeObject(user);

        await SendWebRequestAsync(url, "PUT", jsonData);
        Debug.Log($"User with id {userId} and nickname {nickname} was created successfully.");
        return user;
    }

    public async UniTask<User> GetUserAsync(long userId)
    {
        string url = $"{databaseURL}/users/{userId}.json";
        string responseData = await SendWebRequestAsync(url);

        if (!string.IsNullOrEmpty(responseData))
        {
            return JsonConvert.DeserializeObject<User>(responseData);
        }
        else
        {
            Debug.LogError($"Cannot find user with uid {userId}, returning null.");
            return null;
        }
    }

    public async UniTask<int> GetUserInventoryParamByNameAsync(long userId, string paramName)
    {
        string url = $"{databaseURL}/users/{userId}/Inventory/{paramName}.json";
        string responseData = await SendWebRequestAsync(url);

        if (int.TryParse(responseData, out int data))
        {
            return data;
        }
        else
        {
            Debug.LogError($"Cannot parse user data with uid {userId} by param {paramName}, returning 0.");
            return 0;
        }
    }

    public async UniTask<int> GetUserParamByNameAsync(long userId, string paramName)
    {
        string url = $"{databaseURL}/users/{userId}/{paramName}.json";
        string responseData = await SendWebRequestAsync(url);

        if (int.TryParse(responseData, out int data))
        {
            return data;
        }
        else
        {
            // Debug.LogError($"Cannot parse user data with uid {userId} by param {paramName}, returning 0.");
            return 0;
        }
    }

    public async UniTask UpdateUserDataByParamAsync(long userId, string paramName, int updateData)
    {
        string url = $"{databaseURL}/users/{userId}/{paramName}.json";
        string jsonData = JsonConvert.SerializeObject(updateData);

        await SendWebRequestAsync(url, "PUT", jsonData);
        Debug.Log("Data was updated successfully.");
    }

    public async UniTask UpdateUserDataByParamAsync(long userId, string paramName, bool updateData)
    {
        string url = $"{databaseURL}/users/{userId}/{paramName}.json";
        string jsonData = JsonConvert.SerializeObject(updateData);

        await SendWebRequestAsync(url, "PUT", jsonData);
        Debug.Log("Data was updated successfully.");
    }

    public async UniTask UpdateUserAsync(long userId, UserInventory updateData)
    {
        string url = $"{databaseURL}/users/{userId}/Inventory.json";
        string jsonData = JsonConvert.SerializeObject(updateData);

        await SendWebRequestAsync(url, "PATCH", jsonData);
        Debug.Log("Data was updated successfully.");
    }

    public async UniTask<List<ReferralFriend>> GetReferralFriendsAsync(long userId)
    {
        string url = $"{databaseURL}/users/{userId}/ReferralFriends.json";
        string responseData = await SendWebRequestAsync(url);

        if (!string.IsNullOrEmpty(responseData))
        {
            try
            {
                List<ReferralFriend> referralFriends =
                    JsonConvert.DeserializeObject<List<ReferralFriend>>(responseData);
                return referralFriends ?? new List<ReferralFriend>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error deserializing ReferralFriends for user {userId}: {ex.Message}");
                return new List<ReferralFriend>();
            }
        }
        else
        {
            Debug.LogError($"No data returned for user {userId} ReferralFriends.");
            return new List<ReferralFriend>();
        }
    }

    public async UniTask AddReferralFriendAsync(long userId, ReferralFriend newFriend)
    {
        var referralFriends = await GetReferralFriendsAsync(userId) ?? new List<ReferralFriend>();

        if (referralFriends.All(friend => friend.UID != newFriend.UID))
        {
            referralFriends.Add(newFriend);
            string jsonData = JsonConvert.SerializeObject(referralFriends);
            string url = $"{databaseURL}/users/{userId}/ReferralFriends.json";
            await SendWebRequestAsync(url, "PUT", jsonData);
        }
        else
        {
            Debug.Log($"Referral user with UID={newFriend.UID} already exists, skipping add operation.");
        }
    }

    public async UniTask AddReferralFriendsListAsync(long userId, List<ReferralFriend> newFriends)
    {
        // Сериализуем новый список в JSON
        string jsonData = JsonConvert.SerializeObject(newFriends);

        // Формируем URL для отправки данных
        string url = $"{databaseURL}/users/{userId}/ReferralFriends.json";

        // Отправляем новый список на сервер с помощью PUT-запроса
        await SendWebRequestAsync(url, "PUT", jsonData);
    }

    public async UniTask<List<(User user, int rank)>> GetTopUsersByParamNameAsync(long myUid, string param,
        int topN = 7)
    {
        // Шаг 1: Получаем всех пользователей
        string url = $"{databaseURL}/users.json?orderBy=\"{param}\"";
        string responseData = await SendWebRequestAsync(url);

        var usersDict = JsonConvert.DeserializeObject<Dictionary<string, User>>(responseData);

        // Если данные пустые, возвращаем пустой список
        if (usersDict == null || usersDict.Count == 0)
        {
            return new List<(User, int)>();
        }

        // Шаг 2: Сортируем пользователей по указанному параметру и определяем ранг
        var sortedUsers = usersDict.Values
            .OrderByDescending(u => GetUserParamValue(u, param))
            .Select((user, index) => (user, rank: index + 1))
            .ToList();

        // Шаг 3: Определяем ранг myUser
        var myUserEntry = sortedUsers.FirstOrDefault(u => u.user.UID == myUid);

        if (myUserEntry.user == null)
        {
            throw new Exception($"User with UID {myUid} not found.");
        }

        int myUserRank = myUserEntry.rank;

        // Шаг 4: Формируем итоговый список
        var topUsers = sortedUsers.Take(topN).ToList();

        // Если myUser не входит в топN, добавляем его позицию отдельно
        if (myUserRank > topN)
        {
            topUsers.Add(myUserEntry);
        }

        return topUsers;
    }

    private object GetUserParamValue(User user, string param)
    {
        // Используем рефлексию для получения значения поля по имени
        var propertyInfo = typeof(User).GetProperty(param);
        if (propertyInfo == null)
        {
            throw new ArgumentException($"Parameter {param} is not a valid property of User.");
        }

        return propertyInfo.GetValue(user);
    }

    public async UniTask<CurrentTasks> GetUserCurrentTasksAsync(long userId)
    {
        string url = $"{databaseURL}/users/{userId}/CurrentTasks.json";
        string responseData = await SendWebRequestAsync(url);

        try
        {
            return JsonConvert.DeserializeObject<CurrentTasks>(responseData);
        }
        catch (Exception ex)
        {
            Debug.LogError(
                $"Cannot parse user data with uid {userId} current tasks, exception: {ex.Message}, returning empty dictionary.");
            return new CurrentTasks();
        }
    }

    public async UniTask UpdateUserTaskAsync(long userId, int taskIndex, bool isCompleted)
    {
        // Определяем, какое поле обновить в зависимости от индекса задачи
        string fieldToUpdate = taskIndex switch
        {
            0 => "task1",
            1 => "task2",
            2 => "task3",
            _ => throw new ArgumentOutOfRangeException(nameof(taskIndex), "Invalid task index")
        };

        string url = $"{databaseURL}/users/{userId}/CurrentTasks.json";

        var updateData = new Dictionary<string, bool> { { fieldToUpdate, isCompleted } };
        string requestBody = JsonConvert.SerializeObject(updateData);

        await SendWebRequestAsync(url, "PATCH", requestBody);
    }

    public async UniTask UpdateAllUserTasksAsync(long userId, CurrentTasks updatedTasks)
    {
        string url = $"{databaseURL}/users/{userId}/CurrentTasks.json";
        string requestBody = JsonConvert.SerializeObject(updatedTasks);

        await SendWebRequestAsync(url, "PUT", requestBody);
    }

    public async UniTask<List<string>> GetUrlsAsync(string urlType)
    {
        string url = $"{databaseURL}/tasks/{urlType}.json";

        string jsonResponse = await SendWebRequestAsync(url, "GET");

        List<string> urls = JsonConvert.DeserializeObject<List<string>>(jsonResponse) ?? new List<string>();

        return urls;
    }

    public async UniTask AddUrlToUserAsync(long userId, string urlToAdd)
    {
        // Получаем текущий список URL-ов, которые пользователь уже выполнил
        string userUrlsPath = $"{databaseURL}/users/{userId}/alreadyDoneUrls/.json";
        string jsonResponse = await SendWebRequestAsync(userUrlsPath, "GET");

        List<string> userUrls = JsonConvert.DeserializeObject<List<string>>(jsonResponse) ?? new List<string>();

        // Добавляем новый URL, если его еще нет в списке
        if (!userUrls.Contains(urlToAdd))
        {
            userUrls.Add(urlToAdd);

            // Сохраняем обновленный список обратно в базу данных
            string jsonData = JsonConvert.SerializeObject(userUrls);
            await SendWebRequestAsync(userUrlsPath, "PUT", jsonData);

            Debug.Log("URL was added to user data successfully.");
        }
        else
        {
            Debug.Log("URL is already in the user's completed list.");
        }
    }
}