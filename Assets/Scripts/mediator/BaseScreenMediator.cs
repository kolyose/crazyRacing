using System;
using UnityEngine;

public abstract class BaseScreenMediator : MonoBehaviour, IScreenMediator
{
    protected Canvas view;

    protected virtual void Awake()
    {
        view = GetComponent<Canvas>();

        Messenger<uint>.AddListener(ViewEvent.SHOW_SCREEN, ShowScreenHandler);
        Messenger<uint>.AddListener(ViewEvent.HIDE_SCREEN, HideScreenHandler);
    }

    public abstract uint GetScreenID();

    public void HideScreenHandler(uint screenID)
    {
        if (GetScreenID() != screenID)
            return;

        view.enabled = false;
    }

    public void ShowScreenHandler(uint screenID)
    {
        if (GetScreenID() != screenID)
            return;

        view.enabled = true;
    }
}