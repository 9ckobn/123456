using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TaskElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI taskText;
    [SerializeField] private TextMeshProUGUI rewardText;

    public void Setup(string text, int rewardAmount)
    {
        taskText.text = text;
        rewardText.text = $"+{rewardAmount} <sprite=0>";
    }

    public async UniTask HideTask()
    {
        await transform.DOMoveX(-500, 0.75f).SetDelay(0.3f).SetEase(Ease.InOutElastic).OnComplete(() =>
        {
            gameObject.SetActive(false);
        }).AsyncWaitForCompletion().AsUniTask();
    }
}