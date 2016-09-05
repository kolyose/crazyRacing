using UnityEngine;
using System;
using System.Collections;

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
        gameBoard.InitializeBackground();
        screensManager.InitScreens();
    }

    public void InitCharacters()
	{
        gameBoard.InitCharacters(mainModel.RoundPlayers, mainModel.User);
	}
    
    public void GetLoginData()
    {
        screensManager.ShowLoginScreen();
    }

    public void OnLoginDataReady()
    {
        screensManager.HideLoginScreen();
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

    public void ShowLoader()
    {
        screensManager.ShowLoaderScreen();
    }

    public void HideLoader()
    {
        screensManager.HideLoaderScreen();
    }

    public void JoinRoom(string roomID)
    {
        dataService.JoinRoom(roomID, mainModel.User);
    }

    public void InitRoundData(RoundResultVO[] results)
    {
        mainModel.InitRoundData(results);
    }

    public void UpdateCharactersPositions()
    {
        gameBoard.UpdateCharactersPositions();
    }

    public void SelectActions()
    {
        screensManager.ShowSelectActionsScreen();
    }

    public void OnActionsSelected()
    {
        screensManager.HideSelectActionsScreen();
    }   

    public void SendSelectedActions(UserActionsVO actions)
    {       
        dataService.SendUserActions(actions);
    }

    public void ShowRoundPreResults()
    {
        gameBoard.ShowRoundPreResults(mainModel.GetRoundPreResults());
    }

    public void ShowGameResults()
    {
        gameBoard.ShowGameResults();
    }

    public void SaveRoundResults(RoundResultVO[] results)
    {
        mainModel.SaveRoundResults(results);
    }
}
