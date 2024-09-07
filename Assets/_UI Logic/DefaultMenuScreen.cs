using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class DefaultMenuScreen : Screen, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    protected MenuHandler menu;
    private Action onClose;
    private RectTransform rectTransform;
    private Vector2 startDragPosition;
    private Vector2 originalPosition = new Vector2(0, yPosToShow);
    private float dragStartTime;
    private const float SpeedThreshold = 1000f;

    // private void OnValidate()
    // {
    //     rectTransform = GetComponent<RectTransform>();
    // }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public override Screen OpenScreen()
    {
        rectTransform = GetComponent<RectTransform>();

        rectTransform.DOKill();
        SetInitialPosition();
        gameObject.SetActive(true);
        MoveToShowPosition();
        return this;
    }

    public override void OpenScreenLazy()
    {
        rectTransform = GetComponent<RectTransform>();

        rectTransform.DOKill();
        SetInitialPosition();
        gameObject.SetActive(true);
        MoveToShowPosition();
    }

    public void SetupMenu(MenuHandler menu)
    {
        rectTransform = GetComponent<RectTransform>();

        this.menu = menu;
    }

    public void SetupCloseScreen(Action onCloseClick)
    {
        rectTransform = GetComponent<RectTransform>();

        onClose += onCloseClick;
    }

    public override void CloseScreen()
    {
        rectTransform.DOAnchorPosY(yPosToHide, animationDuration)
            .SetEase(animationEase)
            .OnComplete(() =>
            {
                onClose?.Invoke();
                gameObject.SetActive(false);
                // menu.canPlay = true;
            });
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startDragPosition = eventData.position;
        dragStartTime = Time.time;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float deltaY = eventData.position.y - startDragPosition.y;

        if (deltaY < 0)
        {
            rectTransform.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y + deltaY);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float dragEndTime = Time.time;
        float dragDuration = dragEndTime - dragStartTime;
        float dragDistance = startDragPosition.y - eventData.position.y;

        float dragSpeed = dragDistance / dragDuration;

        Debug.Log(dragSpeed);

        if (rectTransform.anchoredPosition.y < originalPosition.y - rectTransform.rect.height / 2 ||
            dragSpeed > SpeedThreshold)
        {
            CloseScreen();
        }
        else
        {
            MoveToShowPosition();
        }
    }

    private void SetInitialPosition()
    {
        transform.position = new Vector3(transform.position.x, yPosToHide, transform.position.z);
    }

    private void MoveToShowPosition()
    {
        rectTransform.DOAnchorPosY(yPosToShow, animationDuration).SetEase(animationEase);
    }
}