public interface IGameStatesFactory
{
    IGameState GetStateInitial(GameManager manager);
    IGameState GetStateLoginInput(GameManager manager);
}
