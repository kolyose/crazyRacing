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
        Messenger<string, string>.Broadcast(ModelEvent.DATA_READY, tfLogin.text, tfPassword.text);
    }

    public override uint GetScreenID()
    {
        return ScreensManager.LOGIN;
    }
}
