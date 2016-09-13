public interface IScreenMediator
{
    void ShowScreenHandler(ScreenID screenID);
    void HideScreenHandler(ScreenID screenID);
    ScreenID GetScreenID();
}