using DG.Tweening;
using UnityEngine;

public abstract class Screen : MonoBehaviour
{
    
    protected const float animationDuration = 0.25f;
    protected const float yPosToShow = 850;
    protected const float yPosToHide = -1500;
    protected const Ease animationEase = Ease.InOutCubic;
    
    public abstract Screen OpenScreen();
    
    public abstract void OpenScreenLazy();
    
    public abstract void CloseScreen();
}