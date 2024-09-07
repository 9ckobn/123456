using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private LoadingScreenAnimation _loadingScreenAnimation;

    public bool instantOpen = true;

    public enum WaitType
    {
        timer,
        condition
    }

    public void OpenScreen(Action onScreenReady = null)
    {
        StopAllCoroutines();

        gameObject.SetActive(true);
        _loadingScreenAnimation.gameObject.SetActive(true);

        _loadingScreenAnimation.onScreenReady = onScreenReady;

        _loadingScreenAnimation.OpenScreen();


        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1.5f);
        _loadingScreenAnimation.CloseScreen();
        yield break;
    }
}