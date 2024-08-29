using UnityEngine;

public class StartScreen : Screen
{
    public override void OpenScreenLazy()
    {
        OpenScreen();
    }

    public override void CloseScreen()
    {
        gameObject.SetActive(false);
        // throw new System.NotImplementedException();
    }

    public override Screen OpenScreen()
    {
        gameObject.SetActive(true);
        return this;
    }
}