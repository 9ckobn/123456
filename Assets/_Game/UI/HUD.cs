using System;
using System.Web;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [Header("InGame")]
    [SerializeField] private LoseScreen loseScreen;
    [SerializeField] private TryAgainScreen tryAgainScreen;

    [SerializeField] private WorldGenerator worldGenerator;
    [SerializeField] private Player player;

    [Header("Menu")] 
    [SerializeField] private MenuHandler menu;
    
    public async void ShowTryAgainScreen(int score, int coins)
    {
        var client = new API();
        long uid = menu.GetUID;

        int pastCoins = await client.GetUserParamByNameAsync(uid, "coins");

        await client.UpdateUserDataByParamAsync(uid, "lastSessionCoins", coins);
        await client.UpdateUserDataByParamAsync(uid, "lastSessionScore", score);
        
        await client.UpdateUserDataByParamAsync(uid, "coins", coins + pastCoins);

        if (await client.GetUserParamByNameAsync(uid, "maxScore") < score)
        {
            await client.UpdateUserDataByParamAsync(uid, "maxScore", score);
        }

        loseScreen.SetupScreen(coins, score, ()=>
        {
            player.ClearPlayer();
            worldGenerator.ClearLevel();
            loseScreen.CloseScreen();
            menu.StartMenu();
        });

        tryAgainScreen.onTimeEnded =()=>
        {
            menu.GameHud.HideHud();
            ShowLoseScreen();
        };
        tryAgainScreen.OpenScreen();
    }

    public void ShowLoseScreen()
    {
        loseScreen.OpenScreen();
    }


//     private long GetUserId()
//     {
// #if PLATFORM_WEBGL && !UNITY_EDITOR
//         Uri uri = new Uri(Application.absoluteURL);
// #elif UNITY_EDITOR
//         Uri uri = new Uri(
//             "https://www.catflip.run/index.html?userid=6641226142&usernickname=LegendarioAdmin");
// #endif
//         string query = uri.Query;
//         var queryParameters = HttpUtility.ParseQueryString(query);
//
//         return long.Parse(queryParameters["userid"]);
//     }
}