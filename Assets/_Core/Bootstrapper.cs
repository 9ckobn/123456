using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private MenuHandler menu;

    private User currentUser;
    private API client;

#if UNITY_EDITOR
    [SerializeField] private MockDataExample mockUserDataExample;

    public enum MockDataExample
    {
        bergmanDev,
        Yanaooo0,
        theDevil
    }

    private UserDataRaw mockUserData
    {
        get
        {
            switch (mockUserDataExample)
            {
                case MockDataExample.bergmanDev:
                    return new UserDataRaw()
                    {
                        user = new UserRaw()
                        {
                            first_name = "Anton",
                            id = 1362109502,
                            is_premium = true,
                            username = "bergmanDev"
                        },
                        referral = ""
                    };
                case MockDataExample.theDevil:
                    return new UserDataRaw()
                    {
                        user = new UserRaw()
                        {
                            id = 666,
                            is_premium = true,
                            username = "theDevil"
                        },
                        referral = "referral_1362109502"
                    };
                case MockDataExample.Yanaooo0:
                    return new UserDataRaw()
                    {
                        user = new UserRaw()
                        {
                            id = 897956266,
                            is_premium = false,
                            username = "Yanaooo0"
                        },
                        referral = "referral_666"
                    };
                default:
                    return new UserDataRaw()
                    {
                        user = new UserRaw()
                        {
                            first_name = "Anton",
                            id = 1362109502,
                            is_premium = true,
                            username = "bergmanDev"
                        },
                        referral = ""
                    };
            }
        }
    }
#endif

    private void Awake()
    {
        client = new API();
#if UNITY_EDITOR
        GetOrCreateUser(mockUserData).Forget();
#endif
    }

    public void SetUserRaw(string userDataJson)
    {
        var rawUserData = JsonConvert.DeserializeObject<UserDataRaw>(userDataJson);
        GetOrCreateUser(rawUserData).Forget();
    }

    private async UniTask GetOrCreateUser(UserDataRaw userDataRaw)
    {
        Debug.Log("Starting user retrieval process...");

        var user = userDataRaw.user;
        currentUser = await client.GetUserAsync(user.id);

        if (currentUser == null)
        {
            try
            {
                Debug.Log("User not found, creating a new one...");
                currentUser = await client.CreateUserAsync(user.id, user.username, user.is_premium,
                    !string.IsNullOrEmpty(userDataRaw.referral));
                
                PlayerPrefs.DeleteAll();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during user creation: {e.Message}");
                return;
            }
        }

        if (!string.IsNullOrEmpty(userDataRaw.referral) && currentUser.CanBeReferral)
        {
            await UpdateReferralData(userDataRaw.referral);
        }

        Debug.Log("User retrieval/creation complete, starting menu...");
        menu.StartMenu(currentUser);
    }

    private async UniTask UpdateReferralData(string referral)
    {
        string[] parts = referral.Split('_');
        if (parts.Length == 2 && long.TryParse(parts[1], out long referralId))
        {
            currentUser.CanBeReferral = false;
            await client.UpdateUserDataByParamAsync(currentUser.UID, "CanBeReferral", false);
            await client.AddReferralFriendAsync(referralId, new ReferralFriend(currentUser.UID, currentUser.IsPremium));

            if (currentUser.IsPremium)
            {
                await client.UpdateUserDataByParamAsync(referralId, "coins",
                    menu.GetUserCoins + 15500);
                await client.UpdateUserDataByParamAsync(currentUser.UID, "coins",
                    menu.GetUserCoins + 15500);
            }
            else
            {
                await client.UpdateUserDataByParamAsync(referralId, "coins",
                    menu.GetUserCoins + 5500);
                await client.UpdateUserDataByParamAsync(currentUser.UID, "coins",
                    menu.GetUserCoins + 5500);
            }

            Debug.Log("Referral data updated successfully.");
        }
        else
        {
            Debug.LogError("Invalid referral format.");
        }
    }

    [Serializable]
    private struct UserDataRaw
    {
        public UserRaw user;
        public string referral;
    }

    [Serializable]
    private struct UserRaw
    {
        public long id;
        public bool is_bot;
        public string first_name;
        public string last_name;
        public string username;
        public string language_code;
        public bool is_premium;
    }
}