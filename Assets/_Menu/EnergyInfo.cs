using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyInfo : MonoBehaviour
{
    private int currentEnergy = 1000;

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image slider;

    private void Awake()
    {
        text.text = "Loading...";
        slider.fillAmount = 0;
    }

    public void UpdateEnergy(int amount)
    {
        currentEnergy = amount;
        text.text = $"{currentEnergy} / 1000";
        slider.fillAmount = currentEnergy / 1000f;
    }
}