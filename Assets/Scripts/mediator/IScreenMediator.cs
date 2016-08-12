public interface IScreenMediator
{
    void ShowScreenHandler(uint screenID);
    void HideScreenHandler(uint screenID);
    uint GetScreenID();
}