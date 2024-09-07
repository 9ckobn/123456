using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LeaderFrame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rank;
    [SerializeField] private TextMeshProUGUI nickname;
    [SerializeField] private TextMeshProUGUI score;

    [SerializeField] private Image mainFrame;

    [SerializeField] private Sprite activeFrame, inActiveFrame;

    public void SetupFrame(int rank, string nick, string score, bool myCard)
    {
        this.rank.text = rank.ToString();
        nickname.text = nick;
        this.score.text = score;

        if (myCard)
        {
            mainFrame.color = Color.magenta;
            nickname.color = Color.white;
            this.score.color = Color.white;
        }
    }
}