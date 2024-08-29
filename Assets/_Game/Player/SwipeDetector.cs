using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeDetector : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private Vector2 endTouchPosition;
    private bool stopTouch = false;

    public float swipeRange = 40f;
    public float tapRange = 10f;
    public static Action<ControlType> OnSwipe;

    public void OnBeginDrag(PointerEventData eventData)
    {
        stopTouch = false;
        startTouchPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("drag");
        currentTouchPosition = eventData.position;
        Vector2 distance = currentTouchPosition - startTouchPosition;

        if (!stopTouch)
        {
            if (distance.x < -swipeRange)
            {
                stopTouch = true;
                OnSwipeLeft();
            }
            else if (distance.x > swipeRange)
            {
                stopTouch = true;
                OnSwipeRight();
            }
            else if (distance.y > swipeRange)
            {
                stopTouch = true;
                OnSwipeUp();
            }
            else if (distance.y < -swipeRange)
            {
                stopTouch = true;
                OnSwipeDown();
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        stopTouch = true;
        endTouchPosition = eventData.position;

        Vector2 distance = endTouchPosition - startTouchPosition;

        if (Mathf.Abs(distance.x) < tapRange && Mathf.Abs(distance.y) < tapRange)
        {
            OnTap();
        }
    }

    private void OnSwipeLeft()
    {
        OnSwipe?.Invoke(ControlType.SwipeLeft);
    }

    private void OnSwipeRight()
    {
        OnSwipe?.Invoke(ControlType.SwipeRight);
    }

    private void OnSwipeUp()
    {
        OnSwipe?.Invoke(ControlType.SwipeUp);
    }

    private void OnSwipeDown()
    {
        OnSwipe?.Invoke(ControlType.SwipeDown);
    }

    private void OnTap()
    {
        OnSwipe?.Invoke(ControlType.Touch);
    }
}

public enum ControlType
{
    SwipeLeft,
    SwipeRight,
    SwipeUp,
    SwipeDown,
    Touch
}