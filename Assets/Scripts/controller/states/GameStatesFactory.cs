using System;

public class GameStatesFactory : IGameStatesFactory
{
    public IGameState GetStateInitial(GameManager manager)
    {
        return new GameStateInitial(manager, this);
    }

    public IGameState GetStateLoginInput(GameManager manager)
    {
        return new GameStateLoginInput(manager, this);
    }
}
