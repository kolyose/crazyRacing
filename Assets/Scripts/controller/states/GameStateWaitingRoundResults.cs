public class GameStateWaitingRoundResults : BaseGameState
{
    public GameStateWaitingRoundResults(GameManager manager, IGameStatesFactory factory) : base(manager, factory)
    {

    }

    public override void Entry()
    {
        base.Entry();
        Messenger<RoundResultVO[]>.AddListener(ServerCommand.ROUND_RESULTS, OnRoundResults);
        _manager.ShowScreen(ScreenID.WAITING_FOR_PLAYERS);
    }

    public override void Exit()
    {
        base.Exit();
        Messenger<RoundResultVO[]>.RemoveListener(ServerCommand.ROUND_RESULTS, OnRoundResults);
        _manager.HideScreen(ScreenID.WAITING_FOR_PLAYERS);
    }

    private void OnRoundResults(RoundResultVO[] results)
    {
        _manager.SaveRoundResults(results);
        _manager.ApplyState(_factory.GetStateDisplayRoundResults(_manager));
    }
}