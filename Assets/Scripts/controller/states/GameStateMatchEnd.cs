public class GameStateMatchEnd : BaseGameState
{
    public GameStateMatchEnd(GameManager manager, IGameStatesFactory factory) : base(manager, factory)
    {

    }

    public override void Entry()
    {
        base.Entry();

        _manager.ResetScreen(ScreenID.GAME_RESULTS);
        _manager.UpdateMatchResults();
        _manager.ShowScreen(ScreenID.GAME_RESULTS);
    }

    public override void Exit()
    {
        base.Exit();
    }
}