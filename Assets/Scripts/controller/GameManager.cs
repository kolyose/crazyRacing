using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {
    
    //Currently we're using properties injection through common GameObject ('Main') our scripts are attached too.
    //But the following properties should still remain public for injection purposes - if we want to inject them some other way in future.
    //But let's hide them from the Inspector in order to avoid mess with empty properties' fields.
    [HideInInspector]
    public MainModel mainModel;
    [HideInInspector]
    public PixelPerfectCamera camera;

    public IGameBoard           gameBoard;
    public IDataService         dataService;
    public IInput               inputController;
    public IScreensManager      screensManager;
    public IGameStatesFactory   gameStatesFactory;

    private IGameState _state;

 	void Start () 
    {
        InitializeComponents();
        ApplyState(gameStatesFactory.GetStateInitial(this));
    }

    public void ApplyState(IGameState newState)
    {
        Debug.Log("STATE: " + newState);

        if (_state != null)
        {
            _state.Exit();
        }

        _state = newState;
        _state.Entry();
    }

    private void InitializeComponents()
    {
        if (mainModel == null)          mainModel =         GetComponent<MainModel>();
        if (camera == null)             camera =            GetComponent<PixelPerfectCamera>();
        if (gameBoard == null)          gameBoard =         GetComponent<IGameBoard>();
        if (dataService == null)        dataService =       GetComponent<IDataService>();
        if (inputController == null)    inputController =   GetComponent<IInput>();
        if (screensManager == null)     screensManager =    GetComponent<IScreensManager>();
        if (gameStatesFactory == null) gameStatesFactory =  GetComponent<IGameStatesFactory>();        
    }

    public void InitializeUI()
    {       
        screensManager.InitScreens();
    }

    public void ShowScreen(ScreenID screenID)
    {
        screensManager.ShowScreen(screenID);
    }

    public void HideScreen(ScreenID screenID)
    {
        screensManager.HideScreen(screenID);
    }

    public void ResetScreen(ScreenID screenID)
    {
        screensManager.ResetScreen(screenID);
    }

    public void SendLoginDataToServer(SessionDataVO loginData)
    {
        dataService.Login(loginData);
    }

    public void SetLoginData(string login, string password)
    {
        SessionDataVO newSession = new SessionDataVO(login, password);
        mainModel.SessionData = newSession;
    }

    public SessionDataVO GetLoginData()
    {
        return mainModel.SessionData;
    }

    public void SaveUserData(PlayerVO playerVO)
    {
        mainModel.User = playerVO;
    }

    public void AddPlayersToModel(PlayerVO[] players)
    {
        mainModel.AddPlayers(players);
    }

    public void RemovePlayersFromModel(PlayerVO[] players)
    {
        mainModel.RemovePlayers(players);
    }

    public void SaveCharacterId(uint characterId)
    {
        mainModel.User.characterData = new CharacterVO(characterId);
    }

    public void JoinRoom(string roomID)
    {
        dataService.JoinRoom(roomID, mainModel.User);
    }

    public void SetGameSettings(SettingsVO settings)
    {
        mainModel.GameSettings = settings;
    }

    public void SaveRoundResults(RoundResultVO[] results)
    {
        mainModel.SaveRoundResults(results);
    }

    public void PrepareModel()
    {
        mainModel.ResetMatchData();
    }

    public void PrepareCamera()
    {
        camera.UpdateSettings();
        ZoomToFieldLength();
    }

    public void PrepareGameboard()
    {
        gameBoard.InitBackground();
        gameBoard.InitTiles();
        gameBoard.InitCharacters();
    }

    public void ProcessMilestones()
    {
        gameBoard.ProcessMilestones();
    }

    public void ZoomToFieldWidth()
    {
        camera.ZoomToFieldWidth();
    }

    public void ZoomToFieldLength()
    {
        camera.ZoomToFieldLength();
    }

    public void StartMoving()
    {
        gameBoard.DisplayCharactersAnimation(AnimationState.Running, true);
    }  

    public void StopMoving()
    {
        gameBoard.DisplayCharactersAnimation(AnimationState.Running, false);
    }

    public void UpdateCameraPosition(Vector3 position)
    {
        camera.FollowUserCharacter(position, mainModel.IsNewMatch);
    }   
    
    public void UpdateUserDistance()
    {
        uint userDistance = mainModel.RoundResultsByPlayerId[mainModel.User.id].distance;
        Messenger<uint>.Broadcast(ViewEvent.SET_DISTANCE, userDistance);
    }
    
    public void UpdateBoostAvailability()
    {
        Messenger<bool>.Broadcast(ViewEvent.UPDATE_BOOST_AVAILABILITY, mainModel.IsBoostAllowed());
    }

    public void UpdateMatchResults()
    {
        SortedDictionary<uint, string> playersByPlace = mainModel.GetPlayersByPlace();
        Messenger<SortedDictionary<uint, string>>.Broadcast(ViewEvent.SET_GAME_RESULTS, playersByPlace);
    }

    public void SendSelectedActions(UserActionsVO actions)
    {       
        dataService.SendUserActions(actions);
    }
        
    public bool IsMatchEnd()
    {
        return mainModel.IsMatchEnd();
    }
}
