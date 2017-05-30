public class GameStateLoginInput : BaseGameState
{
    public GameStateLoginInput(GameManager manager, IGameStatesFactory factory):base(manager, factory){}

    public override void Entry()
    {
        base.Entry();

        Messenger<string, string>.AddListener(ModelEvent.LOGIN_INPUT, OnLoginDataReady);
        _manager.ShowScreen(ScreenID.LOGIN);
    }

    public override void Exit()
    {
        base.Exit();
        Messenger<string, string>.RemoveListener(ModelEvent.LOGIN_INPUT, OnLoginDataReady);        
    }

    private void OnLoginDataReady(string login, string password)
    {
        _manager.SetLoginData(login, password);
        _manager.ApplyState(_factory.GetStateLoginVerification(_manager));

    }
}
