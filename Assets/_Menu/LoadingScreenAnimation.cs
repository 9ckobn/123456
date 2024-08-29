using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LoadingScreenAnimation : MonoBehaviour
{
    private Image rootImage;

    [SerializeField] private Sprite[] openSprites;
    [SerializeField] private Sprite[] middleSprites;

    public int fps = 30;
    private int milisecondsDelay;

    private bool condition = false;
    private Sequence closeSequence;
    public Action onScreenReady;
    [SerializeField] private bool startFromMiddleOnEnable;

    private void Awake()
    {
        rootImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        if (startFromMiddleOnEnable)
        {
            OpenScreenFromMiddle();
        }
    }

    public void OpenScreen()
    {
        milisecondsDelay = (int)1000f / fps;

        rootImage.color = new Color(1, 1, 1, 0);
        rootImage.DOFade(1, 0.3f).OnComplete(() => { OpenAnimation(); });
    }

    public void OpenScreenInstant()
    {
        milisecondsDelay = (int)1000f / fps;

        rootImage.color = new Color(1, 1, 1, 1);
        OpenAnimation();
    }

    public void OpenScreenFromMiddle()
    {
        milisecondsDelay = (int)1000f / fps;

        rootImage.color = new Color(1, 1, 1, 1);
        Loading();
    }

    async void OpenAnimation()
    {
        for (int i = 0; i < openSprites.Length; i++)
        {
            rootImage.sprite = openSprites[i];
            await UniTask.Delay(milisecondsDelay);
        }

        onScreenReady?.Invoke();

        Loading();
    }

    async void Loading()
    {
        while (!condition)
        {
            for (int i = 0; i < middleSprites.Length; i++)
            {
                rootImage.sprite = middleSprites[i];
                await UniTask.Delay(milisecondsDelay);

                if (condition || !gameObject.activeInHierarchy) break;
            }
        }

        condition = false;

        for (int i = openSprites.Length - 1; i >= 0; i--)
        {
            rootImage.sprite = openSprites[i];
            await UniTask.Delay(milisecondsDelay);
        }

        closeSequence.Play();
        // rootImage.DOFade(0, 0.3f);
    }

    [ContextMenu("Close")]
    public void CloseScreen()
    {
        closeSequence = DOTween.Sequence();
        closeSequence.Append(rootImage.DOFade(0, 0.3f));
        condition = true;
    }

    public void CloseScreenScale()
    {
        closeSequence = DOTween.Sequence();
        closeSequence
            .Append(rootImage.DOFade(0, 0.3f))
            .OnComplete(() => { gameObject.SetActive(false); });
        condition = true;
    }

    private void OnDestroy()
    {
        condition = true;
        transform.DOKill();
        rootImage.DOKill();
    }
}