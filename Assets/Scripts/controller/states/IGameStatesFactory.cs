public interface IGameStatesFactory
{
    IGameState GetStateInitial(GameManager manager);
    IGameState GetStateLoginInput(GameManager manager);
    IGameState GetStateLoginVerification(GameManager manager);
    IGameState GetStateCharacterSelection(GameManager manager);
    IGameState GetStateMatchmaking(GameManager manager);
    IGameState GetStateMatchStart(GameManager manager);
    IGameState GetStateSelectActions(GameManager manager);
    IGameState GetStateWaitingRoundResults(GameManager manager);
    IGameState GetStateProcessingRoundResults(GameManager manager);
    IGameState GetStateDisplayRoundResults(GameManager manager);
    IGameState GetStateMatchEnd(GameManager manager);
}
