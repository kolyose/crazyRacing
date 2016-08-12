using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour, IInput {

    private UserActionsVO _userActionsVO;

    void Awake()
    {
        _userActionsVO = new UserActionsVO();
    }

    public void OnLoginEntered(string data)
    {
    }

	public void OnBoostClick()
    {
        _userActionsVO.Boost = !_userActionsVO.Boost;
    }

    public void OnUpClick()
    {
        _userActionsVO.Direction.y = (_userActionsVO.Direction.y == 1) ? 0 : 1;
    }

    public void OnDownClick()
    {
        _userActionsVO.Direction.y = (_userActionsVO.Direction.y == -1) ? 0 : -1;
    }

    public void OnDoneClick()
    {
        Messenger<UserActionsVO>.Broadcast(GameEvent.USER_ACTIONS_SELECTED, _userActionsVO);
    }

}
