using UnityEngine;
using System.Linq;
using System.Collections.Generic;

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
    public IGameplayStrategy    gameplay;
    public IInput               inputController;
    public IScreensManager      screensManager;

 	void Start () 
    {
        InitializeComponents();
        InitializeUI();

        gameplay.Play();
    }

    private void InitializeComponents()
    {
        if (mainModel == null)          mainModel =         GetComponent<MainModel>();
        if (camera == null)             camera =            GetComponent<PixelPerfectCamera>();
        if (gameBoard == null)          gameBoard =         GetComponent<IGameBoard>();
        if (dataService == null)        dataService =       GetComponent<IDataService>();
        if (gameplay == null)           gameplay =          GetComponent<IGameplayStrategy>();
        if (inputController == null)    inputController =   GetComponent<IInput>();
        if (screensManager == null)     screensManager =    GetComponent<IScreensManager>();

        gameplay.SetGameManager(this);
    }

    public void InitializeUI()
    {       
        screensManager.InitScreens();
    }

    public void GetLoginData()
    {
        screensManager.ShowScreen(ScreenID.LOGIN);
    }

    public void OnLoginDataReady()
    {
        screensManager.HideScreen(ScreenID.LOGIN);
    }    

    public void Login(string login, string password)
    {
        dataService.Login(login, password);
    }

    public void SaveUserData(PlayerVO playerVO)
    {
        mainModel.User = playerVO;
    }

    public void OnAddPlayersCommand(PlayerVO[] players)
    {
        mainModel.AddPlayers(players);
    }

    public void OnRemovePlayersCommand(PlayerVO[] players)
    {
        mainModel.RemovePlayers(players);
    }

    public void OnStartGameCommand(RoundResultVO[] results)
    {

    }

    public void ShowSelectCharactersScreen()
    {
        screensManager.ShowScreen(ScreenID.SELECT_CHARACTERS);
    }

    public void HideSelectCharactersScreen()
    {
        screensManager.HideScreen(ScreenID.SELECT_CHARACTERS);
    }

    public void ShowWaitingScreen()
    {
        screensManager.ShowScreen(ScreenID.WAITING_FOR_PLAYERS);
    }

    public void HideWaitingScreen()
    {
        screensManager.HideScreen(ScreenID.WAITING_FOR_PLAYERS);
    }

    public void SaveCharacterId(uint characterId)
    {
        mainModel.User.characterData = new CharacterVO(characterId);
    }

    public void JoinRoom(string roomID)
    {
        dataService.JoinRoom(roomID, mainModel.User);
    }

    public void StartGame(SettingsVO gameSettings)
    {
        mainModel.ResetGameData();
        mainModel.GameSettings = gameSettings;

        camera.UpdateSettings();
        camera.ZoomToFieldLength();

        gameBoard.InitBackground();
        gameBoard.InitTiles();
        gameBoard.InitCharacters();
    }

    public void StartMoving()
    {
        //!!! mainModel.IsNewGame getter acts like trigger, so we must to call it once. In other case the second call will return a result opposite to the first call's one
        if (mainModel.IsNewGame)
        {
            gameBoard.UpdateCharactersPositions(true);
        }
        else
        {
            gameBoard.UpdateCharactersPositions(false);
            gameBoard.DisplayCharactersAnimation(AnimationState.Running, true);
        }      
    }  

    public void StopMoving()
    {
        gameBoard.DisplayCharactersAnimation(AnimationState.Running, false);
    }

    public void UpdateCameraPosition(Vector3 position)
    {
        camera.FollowUserCharacter(position, mainModel.IsNewGame);
    }   

    public void SelectActions()
    {
        screensManager.ResetScreen(ScreenID.SELECT_ACTIONS);
        uint userDistance = mainModel.RoundResultsByPlayerId[mainModel.User.id].distance;
        Messenger<uint>.Broadcast(ViewEvent.SET_DISTANCE, userDistance);
        screensManager.ShowScreen(ScreenID.SELECT_ACTIONS);
    }

    public void OnActionsSelected()
    {
        screensManager.HideScreen(ScreenID.SELECT_ACTIONS);
    }   

    public void SendSelectedActions(UserActionsVO actions)
    {       
        dataService.SendUserActions(actions);
    }

    public void ShowGameResults()
    {
        screensManager.ResetScreen(ScreenID.GAME_RESULTS);
        SortedDictionary<uint, string> playersByPlace = mainModel.GetPlayersByPlace();
        Messenger<SortedDictionary<uint, string>>.Broadcast(ViewEvent.SET_GAME_RESULTS, playersByPlace);
        screensManager.ShowScreen(ScreenID.GAME_RESULTS);
    }

    public void ProcessRoundResults(RoundResultVO[] results)
    {
        mainModel.SaveRoundResults(results);
        Messenger<bool>.Broadcast(ViewEvent.UPDATE_BOOST_AVAILABILITY, mainModel.IsBoostAllowed());
    }

    public bool IsGameEnd()
    {
        return mainModel.IsGameEnd();
    }
}
