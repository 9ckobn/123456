using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LeaderScreen : MonoBehaviour, ICategoryScreen
{
    private List<LeaderFrame> createdFrames = new List<LeaderFrame>();
    [SerializeField] private MenuHandler menu;
    [SerializeField] private LeaderFrame framePrefab;
    [SerializeField] private RectTransform root;

    [SerializeField] private Image loadingCircle;

    public async void EnableScreen(string leaderParamName = "")
    {
        gameObject.SetActive(true);

        if (loadingCircle == null)
            return;

        loadingCircle.rectTransform.DOKill();
        loadingCircle.enabled = true;
        loadingCircle.rectTransform.DORotate(new Vector3(90, 0, 0), 1)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);

        if (!string.IsNullOrEmpty(leaderParamName))
        {
            var leaders = await menu.GetTopUsersByParamName(leaderParamName);

            foreach (var (user, rank) in leaders)
            {
                var go = Instantiate(framePrefab, root);
                go.SetupFrame(rank, user.Nickname,
                    leaderParamName.Equals("coins") ? $"{user.coins} <sprite=0>" : user.maxScore.ToString(),
                    user.UID == menu.GetUID);

                createdFrames.Add(go);
            }
        }


        loadingCircle.enabled = false;
        loadingCircle.DOKill();
    }

    public void CloseScreen()
    {
        if (createdFrames != null)
        {
            foreach (var item in createdFrames)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
        }

        gameObject.SetActive(false);
    }

    public void EnableScreen()
    {
        throw new NotImplementedException();
    }
}