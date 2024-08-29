using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button : MonoBehaviour, IPointerDownHandler
{
    public Action onClick;
    public Image targetGraphic;

    public virtual bool interactable
    {
        set
        {
            _interactable = value;

            if (value)
            {
                targetGraphic.color = Color.white;
            }
            else
            {
                targetGraphic.color = Color.grey;
            }
        }

        get => _interactable;
    }

    protected bool _interactable = true;

    private void OnValidate()
    {
        if (targetGraphic == null)
        {
            targetGraphic = gameObject.AddComponent<Image>();
            targetGraphic = GetComponent<Image>();
        }
    }

    void Awake()
    {
        if (targetGraphic == null)
        {
            targetGraphic = gameObject.AddComponent<Image>();
            targetGraphic = GetComponent<Image>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_interactable)
            return;

        onClick?.Invoke();
    }
}