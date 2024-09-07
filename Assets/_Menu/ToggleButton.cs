using UnityEngine;

public class ToggleButton : Button
{
    [SerializeField] private Sprite activeSprite, inactiveSprite;
    private bool enabled = true;

    public void SwitchButton()
    {
        enabled = !enabled;

        targetGraphic.sprite = enabled ? activeSprite : inactiveSprite;
    }
}