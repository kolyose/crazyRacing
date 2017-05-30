public class GameStateSelectActions : BaseGameState
{
    public GameStateSelectActions(GameManager manager, IGameStatesFactory factory) : base(manager, factory)
    {

    }

    public override void Entry()
    {
        base.Entry();

        Messenger<UserActionsVO>.AddListener(GameEvent.USER_ACTIONS_SELECTED, OnUserActionsSelected);    

        _manager.ResetScreen(ScreenID.SELECT_ACTIONS);
        _manager.UpdateBoostAvailability();
        _manager.UpdateUserDistance();
        _manager.ShowScreen(ScreenID.SELECT_ACTIONS);
    }

    private void OnUserActionsSelected(UserActionsVO actions)
    {
        Messenger<UserActionsVO>.RemoveListener(GameEvent.USER_ACTIONS_SELECTED, OnUserActionsSelected);

        _manager.HideScreen(ScreenID.SELECT_ACTIONS);        
        _manager.SendSelectedActions(actions);

        _manager.ApplyState(_factory.GetStateWaitingRoundResults(_manager));
    }
}