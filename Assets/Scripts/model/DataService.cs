using UnityEngine;
using SocketIO;

public class DataService : MonoBehaviour, IDataService
{
    public SocketIOComponent socket;

    public void Login(string name, string password)
    {
        //socket.Connect();

        //TODO: change to real data from server
        //if login successful - we get player's raw data
        //otherwise we get null data (error object)
        PlayerVO playerVO = new PlayerVO();
        playerVO.id = name;
        playerVO.name = name;
            
        Messenger<PlayerVO>.Broadcast(ServerCommand.LOGIN, playerVO);
    }

    public void JoinRoom(string roomID, PlayerVO playerVO)
    {
        //TODO: implement functionality:
        //send user's data to server
        //wait for commands from server: 
        //ServerCommand.ADD_PLAYERS
        //ServerCommand.REMOVE_PLAYERS
        //ServerCommand.START_GAME

        //Dictionary<string, string> data = new Dictionary<string, string>();
        //data["message"] = "hello server!";

        //socket.Connect();

        JoinRoomCommandVO commandVO = new JoinRoomCommandVO
        {
            roomID = roomID,           
            playerVO = playerVO
        };

        socket.On(ServerCommand.ADD_PLAYERS, OnAddPlayers);
        socket.On(ServerCommand.START_GAME, OnStartGame);
        socket.Emit(ServerCommands.JOIN_ROOM, new JSONObject(JsonUtility.ToJson(commandVO)));
    }

    private void OnAddPlayers(SocketIOEvent evt)
    {
        Debug.Log(string.Format("[name: {0}, data: {1}]", evt.name, evt.data));
    }

    private void OnStartGame(SocketIOEvent evt)
    {
        Debug.Log(string.Format("[name: {0}, data: {1}]", evt.name, evt.data));
    }

    public void SendUserActions(UserActionsVO actions)
    {
       // Messenger<RoundResultVO[]>.Broadcast(ServerCommand.ROUND_RESULTS, dataParser.GetRoundResultsData(roundResultsData));
    }
}
