using TMPro;
using UnityEngine;

public class SwitchButton : Button
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TMP_FontAsset activeFont, inactiveFont;
    [SerializeField] private Color activeTextColor, inActiveTextColor;

    [SerializeField] private Sprite activeSprite, inActiveSprite;

    public override bool interactable
    {
        get => _interactable;
        set
        {
            _interactable = value;

            if (value)
            {
                text.font = activeFont;
                text.color = activeTextColor;
                targetGraphic.sprite = activeSprite;
            }
            else
            {
                text.font = inactiveFont;
                text.color = inActiveTextColor;
                targetGraphic.sprite = inActiveSprite;
            }
        }
    }
}