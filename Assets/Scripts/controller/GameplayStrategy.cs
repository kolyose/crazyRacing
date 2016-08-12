using UnityEngine;
using System.Collections;

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
         Messenger<bool>.AddListener(ServerCommand.LOGIN, OnLoginCommand);

        _gameManager.OnLoginDataReady();
        _gameManager.Login(name, password);
    }

    public void OnLoginCommand(bool loginSuccessful)
    {
        Messenger<bool>.RemoveListener(ServerCommand.LOGIN, OnLoginCommand);

        if (loginSuccessful)
        {
            InitGame();
        }
    }

    public void InitGame()
    {
        Messenger<PlayerVO[]>.AddListener(ServerCommand.REMOVE_PLAYERS, OnRemovePlayersCommand);
        Messenger<PlayerVO[]>.AddListener(ServerCommand.ADD_PLAYERS, OnAddPlayersCommand);
        Messenger<RoundResultVO[]>.AddListener(ServerCommand.START_GAME, OnStartGameCommand);
        Messenger<RoundResultVO[]>.AddListener(ServerCommand.ROUND_RESULTS, OnRoundResultsCommand);

        _gameManager.ShowLoader();
        _gameManager.InitGame();
    }

    private void OnAddPlayersCommand(PlayerVO[] players)
    {
        _gameManager.OnAddPlayersCommand(players);
    }

    private void OnRemovePlayersCommand(PlayerVO[] players)
    {
        _gameManager.OnRemovePlayersCommand(players);
    }

    private void OnStartGameCommand(RoundResultVO[] results)
    {
        Messenger.AddListener(ViewEvent.COMPLETE, OnCharatersPositionsUpdateComplete);

        _gameManager.HideLoader();
        _gameManager.InitRoundData(results);
        _gameManager.InitCharacters();
        _gameManager.UpdateCharactersPositions();
    }
    
    /*
     * After receiving round results displaying pre-results animation (rolling dice)
     */
    private void OnRoundResultsCommand(RoundResultVO[] results)
    {
        Messenger.AddListener(ViewEvent.COMPLETE, OnRoundPreResultsShowComplete);

        _gameManager.HideLoader();
        _gameManager.SaveRoundResults(results);
        _gameManager.ShowRoundPreResults();
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
        _gameManager.ShowLoader();
        _gameManager.SendSelectedActions(actions);
    }
    
    /*
     * After pre-result animation move characters
     */
    private void OnRoundPreResultsShowComplete()
    {
        Messenger.RemoveListener(ViewEvent.COMPLETE, OnRoundPreResultsShowComplete);
        Messenger.AddListener(ViewEvent.COMPLETE, OnCharatersPositionsUpdateComplete);

        _gameManager.UpdateCharactersPositions();      
    }

    private void OnCharatersPositionsUpdateComplete()
    {
        Messenger.RemoveListener(ViewEvent.COMPLETE, OnCharatersPositionsUpdateComplete);
        CheckIfRoundComplete();
    }

    /*
     *
     */
    private void CheckIfRoundComplete()
    {
        //TODO: remove GAP and implement real checking
        bool roundComplete = false;

        if (roundComplete)
        {
            _gameManager.ShowGameResults();
        } 
        else
        {
            SelectActions();
        }
    }    
}
