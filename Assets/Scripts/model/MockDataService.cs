using UnityEngine;
using SocketIO;
using System.Collections.Generic;

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
        //Dictionary<string, string> data = new Dictionary<string, string>();
        //data["message"] = "hello server!";

        //socket.Connect();

        PlayerVO player = new PlayerVO {
            id = 1,
            name = "hello there"
        };

        socket.Emit("join", new JSONObject(JsonUtility.ToJson(player)));

        //TODO: change to real data from server
        Messenger<bool>.Broadcast(ServerCommand.LOGIN, true);
    }    

    public void InitGame(CharacterVO characterVO)
    {
        //TODO: implement functionality:
            //send user's data to server
            //wait for commands from server: 
                //ServerCommand.ADD_PLAYERS
                //ServerCommand.REMOVE_PLAYERS
                //ServerCommand.START_GAME
         
        string playersRawData = Resources.Load<TextAsset>("players").text;
        Messenger<PlayerVO[]>.Broadcast(ServerCommand.ADD_PLAYERS, dataParser.GetPlayersData(playersRawData));

        string initialGameData = getRoundDataFromJson(_currentRoundNumber);
        Messenger<RoundResultVO[]>.Broadcast(ServerCommand.START_GAME, dataParser.GetRoundResultsData(initialGameData));
        _currentRoundNumber++;
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
}
