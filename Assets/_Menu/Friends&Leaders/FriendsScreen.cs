using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FriendsScreen : MonoBehaviour, ICategoryScreen
{
    [SerializeField] private RectTransform noFriends, friendsRoot;

    [SerializeField] private FriendFrame framePrefab;
    [SerializeField] private MenuHandler menu;
    [SerializeField] private Image loadingCircle;

    private List<FriendFrame> createdFriendsFrames = new();

    public async void EnableScreen()
    {
        gameObject.SetActive(true);

        if (loadingCircle == null)
            return;

        loadingCircle.rectTransform.DOKill();
        loadingCircle.enabled = true;
        loadingCircle.rectTransform.DORotate(new Vector3(0, 0, 90), 0.1f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);

        var friendsList = await menu.GetFriendsAsync();

        if (friendsList.Count == 0)
        {
            noFriends.gameObject.SetActive(true);
            friendsRoot.gameObject.SetActive(false);
        }
        else
        {
            friendsRoot.gameObject.SetActive(true);
            noFriends.gameObject.SetActive(false);

            foreach (var friend in friendsList)
            {
                var go = (Instantiate(framePrefab, friendsRoot));
                go.SetupFrame(friend);
                createdFriendsFrames.Add(go);
            }
        }

        loadingCircle.enabled = false;
        loadingCircle.DOKill();
    }

    public void EnableScreen(string leaderParamName = "")
    {
        throw new System.NotImplementedException();
    }

    public void CloseScreen()
    {
        if (createdFriendsFrames != null)
        {
            foreach (var item in createdFriendsFrames)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
        }

        gameObject.SetActive(false);
    }
}