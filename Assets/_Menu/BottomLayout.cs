using DG.Tweening;
using UnityEngine;

public class BottomLayout : MonoBehaviour
{
    [SerializeField] private DefaultMenuScreen profileScreen;
    [SerializeField] private ToolbarButton profileButton;

    [SerializeField] private TaskScreen taskScreen;
    [SerializeField] private ToolbarButton tasksButton;

    [SerializeField] private DefaultMenuScreen shopScreen;
    [SerializeField] private ToolbarButton shopButton;

    [SerializeField] private DefaultMenuScreen friendsScreen;
    [SerializeField] private ToolbarButton friendsButton;

    private MenuHandler menu;

    public void Setup(MenuHandler menu)
    {
        this.menu = menu;

        InitializeButtons();
    }

    private void InitializeButtons()
    {
        SetButtonActions(profileButton, profileScreen);
        SetButtonActions(tasksButton, taskScreen);
        SetButtonActions(shopButton, shopScreen);
        SetButtonActions(friendsButton, friendsScreen);
    }

    private void SetButtonActions(Button button, DefaultMenuScreen screen)
    {
        if (screen == null)
            return;

        screen.SetupMenu(menu);

        button.onClick = () =>
        {
            ClearInteractables();
            menu.OpenScreen(screen);
            button.interactable = false;
        };

        screen.SetupCloseScreen(() => { button.interactable = true; });
    }

    public void ShowLayout()
    {
        ClearAllScreens();
        GetComponent<RectTransform>().DOAnchorPosY(0, 0.5f);
    }

    public void HideLayout()
    {
        ClearAllScreens();
        GetComponent<RectTransform>().DOAnchorPosY(-3000, 0.5f);
    }

    private void ClearAllScreens()
    {
        profileScreen.CloseScreen();
        taskScreen.CloseScreen();
        shopScreen.CloseScreen();
        friendsScreen.CloseScreen();

        ClearInteractables();
    }

    private void ClearInteractables()
    {
        Debug.Log("clear");

        profileButton.interactable = true;
        tasksButton.interactable = true;
        shopButton.interactable = true;
        friendsButton.interactable = true;
    }
}