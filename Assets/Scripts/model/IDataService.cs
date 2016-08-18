public interface IDataService
{
    void Login(string login, string password);
    void JoinRoom(string roomID, PlayerVO playerVO);
    void SendUserActions(UserActionsVO actions);
}
