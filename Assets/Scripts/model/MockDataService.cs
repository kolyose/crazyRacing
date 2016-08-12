using UnityEngine;
using System;
using SocketIO;
using SocketIOClient;
using SocketIOClient.Messages;

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
        /* Client client = new Client("ws://127.0.0.1:3000");
         client.Opened += SocketOpened;
         client.SocketConnectionClosed += SocketConnectionClosed;

         client.Connect();*/

        socket.Emit("Hello there!");

        //TODO: change to real data from server
        Messenger<bool>.Broadcast(ServerCommand.LOGIN, true);
    }

    private void SocketOpened(object sender, EventArgs e)
    {
        Debug.Log("SOCKET OPENED");
    }

    private void SocketConnectionClosed(object sender, EventArgs e)
    {
        Debug.Log("SOCKET CLOSED");
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
