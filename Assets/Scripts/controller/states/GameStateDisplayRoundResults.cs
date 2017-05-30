using UnityEngine;

public class GameStateDisplayRoundResults:BaseGameState
{
    public GameStateDisplayRoundResults(GameManager manager, IGameStatesFactory factory) : base(manager, factory)
    {

    }

    public override void Entry()
    {
        base.Entry();
        Messenger<Vector3>.AddListener(ViewEvent.POSITION_UPDATED, OnUserPositionUpdated);
        Messenger.AddListener(ViewEvent.COMPLETE, OnCharatersPositionsUpdateComplete);
        
        _manager.ProcessMilestones();
        _manager.StartMoving();
    }

    public override void Exit()
    {
        base.Exit();
        Messenger<Vector3>.RemoveListener(ViewEvent.POSITION_UPDATED, OnUserPositionUpdated);
    }

    private void OnUserPositionUpdated(Vector3 userPosition)
    {
        _manager.UpdateCameraPosition(userPosition);
    }

    private void OnCharatersPositionsUpdateComplete()
    {
        Messenger.RemoveListener(ViewEvent.COMPLETE, OnCharatersPositionsUpdateComplete);
        _manager.StopMoving();

        if (_manager.IsMatchEnd())
        {
            _manager.ApplyState(_factory.GetStateMatchEnd(_manager));
            return;
        }

        _manager.ApplyState(_factory.GetStateSelectActions(_manager));
    }
}