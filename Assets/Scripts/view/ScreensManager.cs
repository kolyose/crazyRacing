using UnityEngine;

public class ScreensManager : MonoBehaviour, IScreensManager {

    //  public Canvas selectActionsScreen;
    //  public Canvas loginScreen;

    public const uint NONE = 0;
    public const uint LOGIN = 1;
    public const uint SELECT_ACTIONS = 2;

    public void InitScreens()
    {
        HideSelectActionsScreen();
    }

    public void ShowLoginScreen()
    {
        //    loginScreen.enabled = true;
        Messenger<uint>.Broadcast(ViewEvent.SHOW_SCREEN, ScreensManager.LOGIN);
    }

    public void HideLoginScreen()
    {
        //    loginScreen.enabled = false;
        Messenger<uint>.Broadcast(ViewEvent.HIDE_SCREEN, ScreensManager.LOGIN);
    }

    public void ShowSelectActionsScreen()
    {
        //  selectActionsScreen.enabled = true;
        Messenger<uint>.Broadcast(ViewEvent.SHOW_SCREEN, ScreensManager.SELECT_ACTIONS);
    }

    public void HideSelectActionsScreen()
    {
        //    selectActionsScreen.enabled = false;
        Messenger<uint>.Broadcast(ViewEvent.HIDE_SCREEN, ScreensManager.SELECT_ACTIONS);
    }

    public void ShowLoaderScreen()
    {

    }

    public void HideLoaderScreen()
    {

    }
}
