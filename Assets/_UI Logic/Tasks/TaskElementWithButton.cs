using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskElementWithButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI taskText;
    [SerializeField] private TextMeshProUGUI rewardText;

    [SerializeField] private Button button;

    [SerializeField] private Image icon;

    [SerializeField] private Sprite telegramIco, xIco, ytIco;

    public void Setup(string text, int rewardAmount, string onClickUrl, Action callBack)
    {
        var type = ParseText(text);
        Debug.Log(text + " " + type);

        switch (type)
        {
            case IconType.TelegramLink:
                icon.sprite = telegramIco;
                taskText.text = "Subscribe to telegram channel";
                break;
            case IconType.TwitterLink:
                icon.sprite = xIco;
                taskText.text = "Subscribe to X channel";
                break;
            case IconType.YouTubeChannelLink:
                icon.sprite = ytIco;
                taskText.text = "Subscribe to youtube channel";
                break;
            case IconType.YouTubeVideoLink:
                icon.sprite = ytIco;
                taskText.text = "Watch the video";
                break;
        }
        
        rewardText.text = $"+{rewardAmount} <sprite=0>";

        button.onClick = null;

        button.onClick += callBack;

#if UNITY_WEBGL && !UNITY_EDITOR
       button.onClick += () => OpenUrl(onClickUrl);
#else
        button.onClick += () => Application.OpenURL(onClickUrl);
#endif
    }

    [DllImport("__Internal")]
    private static extern void OpenUrl(string url);

    public async UniTask HideTask()
    {
        await transform.DOMoveX(-500, 0.75f).SetDelay(0.3f).SetEase(Ease.InOutElastic).OnComplete(() =>
        {
            gameObject.SetActive(false);
        }).AsyncWaitForCompletion().AsUniTask();
    }
    
    public enum IconType
    {
        None,
        TelegramLink,
        TwitterLink,
        YouTubeVideoLink,
        YouTubeChannelLink
        // Добавьте другие типы по мере необходимости
    }
    
    public IconType ParseText(string input)
    {
        // Проверка на наличие "t.me"
        if (input.Contains("t.me"))
        {
            return IconType.TelegramLink;
        }

        // Проверка на наличие "twitter.com"
        if (input.Contains("x.com"))
        {
            return IconType.TwitterLink;
        }

        // Проверка на наличие "facebook.com"
        if (input.Contains("youtube.com"))
        {
            return IconType.YouTubeChannelLink;
        }
        
        if (input.Contains("youtu.be"))
        {
            return IconType.YouTubeVideoLink;
        }

        // Можно добавить больше проверок для других типов

        // Если ничего не найдено, возвращаем None
        return IconType.None;
    }
}