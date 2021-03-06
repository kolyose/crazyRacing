﻿using UnityEngine;

public class GameStateMatchStart : BaseGameState
{
    public GameStateMatchStart(GameManager manager, IGameStatesFactory factory) : base(manager, factory) { }

    public override void Entry()
    {
        base.Entry();
        _manager.HideScreen(ScreenID.WAITING_FOR_PLAYERS);
        Messenger<RoundResultVO[]>.AddListener(ServerCommand.ROUND_RESULTS, OnRoundResults);

        _manager.PrepareModel();
        _manager.PrepareCamera();
        _manager.PrepareGameboard();
    } 
    
    private void OnRoundResults(RoundResultVO[] results)
    {
        Messenger<RoundResultVO[]>.RemoveListener(ServerCommand.ROUND_RESULTS, OnRoundResults); 
        Messenger.AddListener(ViewEvent.COMPLETE, OnCharatersPositionsUpdateComplete);

        _manager.SaveRoundResults(results);
        _manager.ProcessMilestones();
        
        Util.Instance.SetTimeout(OnTimeoutFinished, 0.3f);
    }

    private void OnTimeoutFinished()
    {        
        _manager.ZoomToFieldWidth();       
    }

    private void OnCharatersPositionsUpdateComplete()
    {
        Messenger.RemoveListener(ViewEvent.COMPLETE, OnCharatersPositionsUpdateComplete);
        _manager.ApplyState(_factory.GetStateSelectActions(_manager));
    }
}