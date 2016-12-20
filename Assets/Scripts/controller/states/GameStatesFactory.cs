public class GameStatesFactory : IGameStatesFactory
{
    public IGameState GetStateInitial(GameManager manager)
    {
        return new GameStateInitial(manager, this);
    }
}
