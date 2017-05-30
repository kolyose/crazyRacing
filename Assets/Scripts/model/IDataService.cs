public interface IDataService
{
    void Login(SessionDataVO loginData);
    void JoinRoom(string roomID, PlayerVO playerVO);
    void SendUserActions(UserActionsVO actions);
}
