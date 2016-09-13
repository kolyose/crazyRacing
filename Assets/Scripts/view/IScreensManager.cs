public interface IScreensManager
{
    void InitScreens();
    void ShowScreen(ScreenID screenId);
    void HideScreen(ScreenID screenId);
    void ResetScreen(ScreenID screenId);
}
