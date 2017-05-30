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

    public IGameState GetStateLoginVerification(GameManager manager)
    {
        return new GameStateLoginVerification(manager, this);
    }

    public IGameState GetStateMatchmaking(GameManager manager)
    {
        return new GameStateMatchmaking(manager, this);
    }

    public IGameState GetStateMatchStart(GameManager manager)
    {
        return new GameStateMatchStart(manager, this);
    }

    public IGameState GetStateSelectActions(GameManager manager)
    {
        return new GameStateSelectActions(manager, this);
    }

    public IGameState GetStateWaitingRoundResults(GameManager manager)
    {
        return new GameStateWaitingRoundResults(manager, this);
    }

    public IGameState GetStateProcessingRoundResults(GameManager manager)
    {
        return new GameStateProcessingRoundResults(manager, this);
    }

    public IGameState GetStateDisplayRoundResults(GameManager manager)
    {
        return new GameStateDisplayRoundResults(manager, this);
    }

    public IGameState GetStateMatchEnd(GameManager manager)
    {
        return new GameStateMatchEnd(manager, this);
    }

    public IGameState GetStateCharacterSelection(GameManager manager)
    {
        return new GameStateCharacterSelection(manager, this);
    }
}
