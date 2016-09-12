using UnityEngine.UI;

public class SelectActionsScreenMediator : BaseScreenMediator
{
    private Button _btnBoost;
    private Button _btnSelect;
    private ToggleGroup _direction;

    private UserActionsVO _userActionsVO;

    protected override void Awake()
    {
        base.Awake();

        _btnBoost = transform.Find("Boost").GetComponent<Button>();
        _btnSelect = transform.Find("Select").GetComponent<Button>();
        _direction = transform.Find("Direction").GetComponent<ToggleGroup>();

        _userActionsVO = new UserActionsVO();
    }

    public void OnBoostClick()
    {
        _userActionsVO.boost = !_userActionsVO.boost;
    }

    public void OnUpClick()
    {
        _userActionsVO.direction = (_userActionsVO.direction == 1) ? 0 : 1;
    }

    public void OnDownClick()
    {
        _userActionsVO.direction = (_userActionsVO.direction == -1) ? 0 : -1;
    }

    public void OnSelectClick()
    {
        Messenger<UserActionsVO>.Broadcast(GameEvent.USER_ACTIONS_SELECTED, _userActionsVO);
    }

    public override uint GetScreenID()
    {
        return ScreensManager.SELECT_ACTIONS;
    }
}
