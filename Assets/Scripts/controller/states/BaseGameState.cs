using System;
using UnityEngine;

public abstract class BaseGameState : IGameState
{
    protected GameManager _manager;
    protected IGameStatesFactory _factory;

    public BaseGameState(GameManager manager, IGameStatesFactory factory)
    {
        _manager = manager;
        _factory = factory;
    }

    public virtual void Entry()
    {

    }

    public virtual void Exit()
    {

    }    
}
