public interface IDataService
{
    void Login(string login, string password);
    void InitGame(CharacterVO characterVO);
    void SendUserActions(UserActionsVO actions);
}
