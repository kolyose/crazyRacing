using UnityEngine;
using System.Collections;

public class UserActionsVO
{
    public Vector2 Direction;
    public bool Boost;

    public UserActionsVO()
    {
        Direction = new Vector2(0, 0);
        Boost = false;
    }
}
