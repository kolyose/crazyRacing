public class GameStateCharacterSelection:BaseGameState
{
    public GameStateCharacterSelection(GameManager manager, IGameStatesFactory factory):base(manager, factory){}

    public override void Entry()
    {
        base.Entry();
        Messenger<uint>.AddListener(GameEvent.CHARACTER_SELECTED, OnCharacterSelected);
        _manager.ShowScreen(ScreenID.SELECT_CHARACTERS);
    }

    public override void Exit()
    {
        base.Exit();
        Messenger<uint>.RemoveListener(GameEvent.CHARACTER_SELECTED, OnCharacterSelected);
        _manager.HideScreen(ScreenID.SELECT_CHARACTERS);
    }

    private void OnCharacterSelected(uint characterId)
    {
        _manager.SaveCharacterId(characterId);
        _manager.ApplyState(_factory.GetStateMatchmaking(_manager));
    }
}