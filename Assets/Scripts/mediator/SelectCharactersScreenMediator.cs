using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SelectCharactersScreenMediator:BaseScreenMediator
{
    private ToggleGroup _tgCharacters;
    private Button _btnSelect;
    private uint _characterId;

    protected override void Awake()
    {
        base.Awake();

        _tgCharacters = transform.Find("CharactersToggleGroup").GetComponent<ToggleGroup>();
        _btnSelect = transform.Find("Select").GetComponent<Button>();
    }

    public void OnSelect()
    {
        _characterId = uint.Parse(_tgCharacters.ActiveToggles().FirstOrDefault().name.Split('_')[1]);
        Messenger<uint>.Broadcast(GameEvent.CHARACTER_SELECTED, _characterId);
    }

    public override ScreenID GetScreenID()
    {
        return ScreenID.SELECT_CHARACTERS;
    }
}
