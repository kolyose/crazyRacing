public class GameStateInitial : BaseGameState
{
    public GameStateInitial(GameManager manager, IGameStatesFactory factory):base(manager, factory)
    {

    }

    public override void Entry()
    {
        base.Entry();
        _manager.InitializeUI();
        _manager.ApplyState(_factory.GetStateLoginInput(_manager));
    }
}
