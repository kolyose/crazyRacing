using UnityEngine;
using System.Collections;
using System;

public class GameplayStrategy : MonoBehaviour, IGameplayStrategy {

    private GameManager _gameManager;

    public void SetGameManager(GameManager gm)
    {
        _gameManager = gm;
    }

    public void Play()
    {
        Messenger<string, string>.AddListener(ModelEvent.DATA_READY, OnLoginDataReady);
        _gameManager.GetLoginData();
    }

    public void OnLoginDataReady(string name, string password)
    {
         Messenger<string, string>.RemoveListener(ModelEvent.DATA_READY, OnLoginDataReady);
         Messenger<PlayerVO>.AddListener(ServerCommand.LOGIN, OnLoginCommand);
        //TODO: add error handling from login errors

        _gameManager.OnLoginDataReady();
        _gameManager.Login(name, password);
    }

    public void OnLoginCommand(PlayerVO playerVO)
    {
        Messenger<PlayerVO>.RemoveListener(ServerCommand.LOGIN, OnLoginCommand);
        Messenger<uint>.AddListener(GameEvent.CHARACTER_SELECTED, OnCharacterSelected);

        _gameManager.SaveUserData(playerVO);
        _gameManager.ShowSelectCharactersScreen();
        _gameManager.PlayBackgroundSound();

        //TODO: add functionality for characters selection
        //TODO: add functionality for private rooms creation
        //TODO: add functionality for joining to specific private rooms
        //JoinRoom();

        
    }

    private void OnCharacterSelected(uint characterId)
    {
        Messenger<uint>.RemoveListener(GameEvent.CHARACTER_SELECTED, OnCharacterSelected);
        _gameManager.HideSelectCharactersScreen();
        _gameManager.SaveCharacterId(characterId);
        JoinRoom();
    }

    public void JoinRoom(string roomID=null)
    {
        Messenger<PlayerVO[]>.AddListener(ServerCommand.REMOVE_PLAYERS, OnRemovePlayersCommand);
        Messenger<PlayerVO[]>.AddListener(ServerCommand.ADD_PLAYERS, OnAddPlayersCommand);
        Messenger<SettingsVO>.AddListener(ServerCommand.START_GAME, OnStartGameCommand);
        Messenger<RoundResultVO[]>.AddListener(ServerCommand.ROUND_RESULTS, OnRoundResultsCommand);

        _gameManager.ShowWaitingScreen();
        _gameManager.JoinRoom(roomID);
    }

    private void OnAddPlayersCommand(PlayerVO[] players)
    {
        _gameManager.OnAddPlayersCommand(players);
    }

    private void OnRemovePlayersCommand(PlayerVO[] players)
    {
        _gameManager.OnRemovePlayersCommand(players);
    }

    private void OnStartGameCommand(SettingsVO gameSettings)
    {
        Messenger<PlayerVO[]>.RemoveListener(ServerCommand.REMOVE_PLAYERS, OnRemovePlayersCommand);
        Messenger<PlayerVO[]>.RemoveListener(ServerCommand.ADD_PLAYERS, OnAddPlayersCommand);

        Messenger<Vector3>.AddListener(ViewEvent.POSITION_UPDATED, OnUserPositionUpdated);

        _gameManager.StartGame(gameSettings);
        _gameManager.StopBackgroundSound();
    }
    
    /*
     * After receiving round results displaying pre-results animation (rolling dice)
     */
    private void OnRoundResultsCommand(RoundResultVO[] results)
    {
        _gameManager.HideWaitingScreen();
        _gameManager.ProcessRoundResults(results);

        Messenger.AddListener(ViewEvent.COMPLETE, OnCharatersPositionsUpdateComplete);
        SoundManager.instance.PlayRunning();
        _gameManager.StartMoving();
    }

    private void OnUserPositionUpdated(Vector3 userPosition)
    {
        _gameManager.UpdateCameraPosition(userPosition);
    }

    /*
     * Selecting user actions such as boost, direction, etc.
     */
    private void SelectActions()
    {
        Messenger<UserActionsVO>.AddListener(GameEvent.USER_ACTIONS_SELECTED, OnUserActionsSelected);
        _gameManager.SelectActions();
    }

    /*
     * After the actions selected sending them to server and waiting for round result
     */
    private void OnUserActionsSelected(UserActionsVO actions)
    {
        Messenger<UserActionsVO>.RemoveListener(GameEvent.USER_ACTIONS_SELECTED, OnUserActionsSelected);

        _gameManager.OnActionsSelected();
        _gameManager.ShowWaitingScreen();
        _gameManager.SendSelectedActions(actions);
    }
    

    private void OnCharatersPositionsUpdateComplete()
    {
        Messenger.RemoveListener(ViewEvent.COMPLETE, OnCharatersPositionsUpdateComplete);
        SoundManager.instance.StopSound();

        if (_gameManager.IsGameEnd())
        {
            _gameManager.ShowGameResults();
            Messenger<Vector3>.RemoveListener(ViewEvent.POSITION_UPDATED, OnUserPositionUpdated);
            return;
        }

        SelectActions();
    }    
}
