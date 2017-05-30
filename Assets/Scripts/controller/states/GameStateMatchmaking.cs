public class GameStateMatchmaking:BaseGameState
{
    public GameStateMatchmaking(GameManager manager, IGameStatesFactory factory) : base(manager, factory) { }

    public override void Entry()
    {
        base.Entry();      
        
        _manager.ShowScreen(ScreenID.WAITING_FOR_PLAYERS);

        Messenger<PlayerVO[]>.AddListener(ServerCommand.REMOVE_PLAYERS, OnRemovePlayers);
        Messenger<PlayerVO[]>.AddListener(ServerCommand.ADD_PLAYERS, OnAddPlayers);
        Messenger<SettingsVO>.AddListener(ServerCommand.START_MATCH, OnStartMatch);

        _manager.JoinRoom(null);
    }

    private void OnAddPlayers(PlayerVO[] players)
    {
        _manager.AddPlayersToModel(players);
    }

    private void OnRemovePlayers(PlayerVO[] players)
    {
        _manager.RemovePlayersFromModel(players);
    }

    private void OnStartMatch(SettingsVO settings)
    {
        _manager.SetGameSettings(settings);
        _manager.ApplyState(_factory.GetStateMatchStart(_manager));
    }
    
    public override void Exit()
    {
        base.Exit();

        Messenger<PlayerVO[]>.RemoveListener(ServerCommand.REMOVE_PLAYERS, OnRemovePlayers);
        Messenger<PlayerVO[]>.RemoveListener(ServerCommand.ADD_PLAYERS, OnAddPlayers);
        Messenger<SettingsVO>.RemoveListener(ServerCommand.START_MATCH, OnStartMatch);
    }
}