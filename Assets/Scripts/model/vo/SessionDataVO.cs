public class SessionDataVO
{
    public SessionDataVO(string login, string password)
    {
        Login = login;
        Password = password;
    }

    public string Login { get; private set; }
    public string Password { get; private set; }
}