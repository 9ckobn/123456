using System;
using DG.Tweening;
using UnityEngine;

public class ScaleAnimation : MonoBehaviour
{
    void Start()
    {
        transform.DOScale(0.75f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}