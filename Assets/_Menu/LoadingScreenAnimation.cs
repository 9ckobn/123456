using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class LoadingScreenAnimation : MonoBehaviour
{
    private VideoPlayer rootVideo;
    [SerializeField] private CanvasGroup rootCanvasGroup;
    public Action onScreenReady;

    private void Awake()
    {
        rootVideo = GetComponent<VideoPlayer>();
        rootVideo.url = System.IO.Path.Combine(Application.streamingAssetsPath, "1.mp4");
        rootVideo.Prepare();
    }

    public void OpenScreen()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        rootCanvasGroup.alpha = 0;
        float elapsedTime = 0f;

        rootVideo.Play();

        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;
            rootCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / 1);
            yield return null;
        }

        rootCanvasGroup.alpha = 1f;

        onScreenReady?.Invoke();
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(1.5f);

        float elapsedTime = 0f;

        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;
            rootCanvasGroup.alpha = Mathf.Clamp01(1f - (elapsedTime / 1));
            yield return null;
        }

        rootCanvasGroup.alpha = 0f;

        rootVideo.Stop();
    }

    public void CloseScreen()
    {
        StartCoroutine(FadeOut());
    }
}