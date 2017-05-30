using UnityEngine.UI;

public class LoginScreenMediator : BaseScreenMediator {

    InputField tfLogin;
    InputField tfPassword;

    protected override void Awake () {

        base.Awake();

        tfLogin = transform.Find("Login").GetComponent<InputField>();
        tfPassword = transform.Find("Password").GetComponent<InputField>();
    }	

    public void OnLoginEntered()
    {
        
    }

    public void OnPasswordEntered()
    {
        //TODO: dispatch the event after corresponding button clicked
        Messenger<string, string>.Broadcast(ModelEvent.LOGIN_INPUT, tfLogin.text, tfPassword.text);
    }

    public override ScreenID GetScreenID()
    {
        return ScreenID.LOGIN;
    }
}
