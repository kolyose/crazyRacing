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

    public void Login(SessionDataVO sessionData)
    {
        //socket.Connect();

        //TODO: change to real data from server
        //if login successful - we get player's raw data
        //otherwise we get null data (error object)
        PlayerVO playerVO = new PlayerVO();
        playerVO.id = sessionData.Login;
        playerVO.name = sessionData.Login;
            
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
        socket.On(ServerCommand.START_MATCH, OnStartGame);
        socket.On(ServerCommand.ROUND_RESULTS, OnRoundResults);
        string json = JsonUtility.ToJson(commandVO);
        socket.Emit(ServerCommands.JOIN_ROOM, new JSONObject(json));
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
        string rawData = evt.data["data"].ToString();
        Debug.Log("START_GAME: " + rawData);
        SettingsVO settings = dataParser.GetSettingsData(rawData);
        settings.fieldLength += 1; //enlarging field for finish line
        Messenger<SettingsVO>.Broadcast(ServerCommand.START_MATCH, settings);
    }

    private void OnRoundResults(SocketIOEvent evt)
    {
        string rawData = evt.data["data"].ToString();
        Debug.Log("ROUND_RESULTS: " + rawData);
        RoundResultVO[] results = dataParser.GetRoundResultsData(rawData);
        Messenger<RoundResultVO[]>.Broadcast(ServerCommand.ROUND_RESULTS, results);
    }

    public void SendUserActions(UserActionsVO actions)
    {
        socket.Emit(ServerCommands.USER_ACTIONS, new JSONObject(JsonUtility.ToJson(actions)));
    }
}
