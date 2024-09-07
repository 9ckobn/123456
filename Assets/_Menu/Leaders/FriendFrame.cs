using TMPro;
using UnityEngine;

public class FriendFrame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nickname;
    [SerializeField] private TextMeshProUGUI score, coins;

    public void SetupFrame(User friend)
    {
        nickname.text = friend.Nickname;
        score.text = $"{friend.maxScore}";
        coins.text = $"{friend.coins} <sprite=0>";
    }
}