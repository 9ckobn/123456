using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInput : MonoBehaviour
{
    public event System.Action OnGoRightEvent;
    public event System.Action OnGoLeftEvent;
    public event System.Action OnRollEvent;
    public event System.Action OnJumpEvent;

    void OnEnable()
    {
        SwipeDetector.OnSwipe += HandleSwipes;
    }

    private void OnDisable()
    {
        SwipeDetector.OnSwipe -= HandleSwipes;
    }

    private void HandleSwipes(ControlType data)
    {
        switch (data)
        {
            case ControlType.SwipeUp:
                OnJump();
                break;
            case ControlType.SwipeRight:
                OnTurnTo(false);
                break;
            case ControlType.SwipeLeft:
                OnTurnTo(true);
                break;
            case ControlType.SwipeDown:
                OnRoll();
                break;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnTurnTo(true);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnTurnTo(false);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnJump();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnRoll();
        }
    }

    public void OnTurnTo(bool isLeft)
    {
        if (isLeft)
            OnGoLeftEvent?.Invoke();
        else
        {
            OnGoRightEvent?.Invoke();
        }
    }

    public void OnRoll()
    {
        OnRollEvent?.Invoke();
    }

    public void OnJump()
    {
        // if (!GameManager.Instance.IsGameStarted)
        // {
        //     GameManager.Instance.StartIntro();
        //     return;
        // }

        OnJumpEvent?.Invoke();
    }
}