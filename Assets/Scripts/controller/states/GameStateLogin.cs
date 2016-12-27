public class GameStateLoginInput : BaseGameState
{
    public GameStateLoginInput(GameManager manager, IGameStatesFactory factory):base(manager, factory){}

    public override void Entry()
    {
        base.Entry();
        _manager.ShowLoginScreen();
    }
}
