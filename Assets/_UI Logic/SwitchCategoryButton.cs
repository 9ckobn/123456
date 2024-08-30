using TMPro;
using UnityEngine;

public class SwitchCategoryButton : Button
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Color activeColor, inActiveColor;

    public override bool interactable
    {
        get => _interactable;
        set
        {
            _interactable = value;

            if (!value)
            {
                text.color = activeColor;
                targetGraphic.color = activeColor;
            }
            else
            {
                text.color = inActiveColor;
                targetGraphic.color = inActiveColor;
            }
        }
    }

    public void SetInteractableWithoutChangeGraphic(bool interactable)
    {
        _interactable = interactable;
    }
}