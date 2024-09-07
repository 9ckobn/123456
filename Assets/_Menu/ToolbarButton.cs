using TMPro;
using UnityEngine;

public class ToolbarButton : Button
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Color activeTextColor, inActiveTextColor;

    [SerializeField] private Sprite activeSprite, inActiveSprite;

    public override bool interactable
    {
        get => _interactable;
        set
        {
            _interactable = value;

            if (!value)
            {
                text.color = activeTextColor;
                targetGraphic.sprite = activeSprite;
            }
            else
            {
                text.color = inActiveTextColor;
                targetGraphic.sprite = inActiveSprite;
            }
        }
    }
}