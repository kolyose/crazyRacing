using UnityEngine;
using SocketIO;
using System;

public class MockDataService : MonoBehaviour, IDataService
{
    [HideInInspector]
    public IServerDataParser dataParser;

    public SocketIOComponent socket;

    private uint _currentRoundNumber;

    void Awake()
    {
        if (dataParser == null) dataParser = GetComponent<IServerDataParser>();
    }

    public void Login(string name, string password)
    {     
        //TODO: change to real data from server
        Messenger<bool>.Broadcast(ServerCommand.LOGIN, true);
    }    

    public void JoinRoom(string roomID, PlayerVO playerVO)
    {
        //mock data
        /*string playersRawData = Resources.Load<TextAsset>("players").text;
        Messenger<PlayerVO[]>.Broadcast(ServerCommand.ADD_PLAYERS, dataParser.GetPlayersData(playersRawData));

        string initialGameData = getRoundDataFromJson(_currentRoundNumber);
        Messenger<RoundResultVO[]>.Broadcast(ServerCommand.START_GAME, dataParser.GetRoundResultsData(initialGameData));
        _currentRoundNumber++;
        */

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
            roomID = name,
            playerVO = playerVO
        };

        socket.Emit(ServerCommands.JOIN_ROOM, new JSONObject(JsonUtility.ToJson(commandVO)));
    }
    
    public void SendUserActions(UserActionsVO actions)
    {
        string roundResultsData = getRoundDataFromJson(_currentRoundNumber);
        _currentRoundNumber++;

        Messenger<RoundResultVO[]>.Broadcast(ServerCommand.ROUND_RESULTS, dataParser.GetRoundResultsData(roundResultsData));
    }

    private string getRoundDataFromJson(uint roundNumber)
    {
       return Resources.Load<TextAsset>("Round_" + roundNumber).text;
    }

    public void Login(SessionDataVO loginData)
    {
        throw new NotImplementedException();
    }
}
