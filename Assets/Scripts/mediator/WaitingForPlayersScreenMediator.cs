using UnityEngine;
using System.Collections;

public class WaitingForPlayersScreenMediator : BaseScreenMediator
{
    public override ScreenID GetScreenID()
    {
        return ScreenID.WAITING_FOR_PLAYERS;
    }
}
