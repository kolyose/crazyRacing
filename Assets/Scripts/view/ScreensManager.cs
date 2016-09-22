using UnityEngine;

public enum ScreenID
{
    NONE,
    LOGIN,
    WAITING_FOR_PLAYERS,
    SELECT_CHARACTERS,  
    SELECT_ACTIONS,
    SIMULATION,
    GAME_RESULTS
}

public class ScreensManager : MonoBehaviour, IScreensManager {
    
    public void InitScreens()
    {
    }

    public void ShowScreen(ScreenID screenId)
    {
        Messenger<ScreenID>.Broadcast(ViewEvent.SHOW_SCREEN, screenId);
    }

    public void HideScreen(ScreenID screenId)
    {
        Messenger<ScreenID>.Broadcast(ViewEvent.HIDE_SCREEN, screenId);
    }

    public void ResetScreen(ScreenID screenId)
    {
        Messenger<ScreenID>.Broadcast(ViewEvent.RESET_SCREEN, screenId);
    }
}
