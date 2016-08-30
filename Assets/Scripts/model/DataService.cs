using UnityEngine;
using SocketIO;
using System.Collections.Generic;

public class DataService : MonoBehaviour, IDataService
{
    [HideInInspector]
    public IServerDataParser dataParser;
    public SocketIOComponent socket;

    void Awake()
    {
        if (dataParser == null) dataParser = GetComponent<IServerDataParser>();
    }

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
        socket.On(ServerCommand.ROUND_RESULTS, OnRoundResults);
        socket.Emit(ServerCommands.JOIN_ROOM, new JSONObject(JsonUtility.ToJson(commandVO)));
    }

    private void OnAddPlayers(SocketIOEvent evt)
    {
        string playersRawData = evt.data["data"].ToString();
        Debug.Log("ADD_PLAYERS: " + playersRawData);
        PlayerVO[] players = dataParser.GetPlayersData(playersRawData);
        Messenger<PlayerVO[]>.Broadcast(ServerCommand.ADD_PLAYERS, players);
    }
    
    private void OnStartGame(SocketIOEvent evt)
    {
        string resultsRawData = evt.data["data"].ToString();
        Debug.Log("START_GAME: " + resultsRawData);
        RoundResultVO[] results = dataParser.GetRoundResultsData(resultsRawData);
        Messenger<RoundResultVO[]>.Broadcast(ServerCommand.START_GAME, results);
    }

    private void OnRoundResults(SocketIOEvent evt)
    {
        string resultsRawData = evt.data["data"].ToString();
        Debug.Log("ROUND_RESULTS: " + resultsRawData);
        RoundResultVO[] results = dataParser.GetRoundResultsData(resultsRawData);
        Messenger<RoundResultVO[]>.Broadcast(ServerCommand.ROUND_RESULTS, results);
    }

    public void SendUserActions(UserActionsVO actions)
    {
        socket.Emit(ServerCommands.USER_ACTIONS, new JSONObject(JsonUtility.ToJson(actions)));
    }
}
