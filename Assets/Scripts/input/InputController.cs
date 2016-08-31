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

    public void OnDoneClick()
    {
        Messenger<UserActionsVO>.Broadcast(GameEvent.USER_ACTIONS_SELECTED, _userActionsVO);
    }

}
