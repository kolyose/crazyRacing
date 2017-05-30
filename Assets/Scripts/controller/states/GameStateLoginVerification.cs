using System;

public class GameStateLoginVerification : BaseGameState
{
    public GameStateLoginVerification(GameManager manager, IGameStatesFactory factory):base(manager, factory){ }

    public override void Entry()
    {
        base.Entry();
        Messenger<PlayerVO>.AddListener(ServerCommand.LOGIN, OnLoginCommandResponse);

        _manager.SendLoginDataToServer(_manager.GetLoginData());
    }

    public override void Exit()
    {
        base.Exit();
        Messenger<PlayerVO>.RemoveListener(ServerCommand.LOGIN, OnLoginCommandResponse);
        _manager.HideScreen(ScreenID.LOGIN);
    }

    private void OnLoginCommandResponse(PlayerVO playerVO)
    {
        _manager.SaveUserData(playerVO);
        _manager.ApplyState(_factory.GetStateCharacterSelection(_manager));
    }
}
