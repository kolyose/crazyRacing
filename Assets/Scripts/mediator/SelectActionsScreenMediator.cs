using UnityEngine.UI;

public class SelectActionsScreenMediator : BaseScreenMediator
{
    private Toggle _toggleBoost;
    private Button _btnSelect;
    private ToggleGroup _tgDirection;
    private Text _tfDistance;

    private UserActionsVO _userActionsVO;

    protected override void Awake()
    {
        base.Awake();

        _toggleBoost = transform.Find("Boost").GetComponent<Toggle>();
        _btnSelect = transform.Find("Select").GetComponent<Button>();
        _tgDirection = transform.Find("Direction").GetComponent<ToggleGroup>();
        _tfDistance = transform.Find("Distance").GetComponent<Text>();

        _userActionsVO = new UserActionsVO();

        Messenger<uint>.AddListener(ViewEvent.SET_DISTANCE, OnSetDistance);
        Messenger<bool>.AddListener(ViewEvent.UPDATE_BOOST_AVAILABILITY, OnUpdateBoostAvailability);
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

    public override ScreenID GetScreenID()
    {
        return ScreenID.SELECT_ACTIONS;
    }

    protected void OnSetDistance(uint data)
    {
        _tfDistance.text = "Your next distance is " + data.ToString();
    }

    protected override void OnReset(ScreenID screenId)
    {
        base.OnReset(screenId);
        _userActionsVO.boost = false;
        _toggleBoost.isOn = false;
        _userActionsVO.direction = 0;
        _tgDirection.SetAllTogglesOff();
    }

    protected void OnUpdateBoostAvailability(bool value)
    {
        _toggleBoost.interactable = value;
    }

}
