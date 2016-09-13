using System;
using UnityEngine;

public abstract class BaseScreenMediator : MonoBehaviour, IScreenMediator
{
    protected Canvas view;

    protected virtual void Awake()
    {
        view = GetComponent<Canvas>();

        Messenger<ScreenID>.AddListener(ViewEvent.SHOW_SCREEN, ShowScreenHandler);
        Messenger<ScreenID>.AddListener(ViewEvent.HIDE_SCREEN, HideScreenHandler);
        Messenger<ScreenID>.AddListener(ViewEvent.RESET_SCREEN, OnReset);
    }

    public abstract ScreenID GetScreenID();

    public void HideScreenHandler(ScreenID screenId)
    {
        if (GetScreenID() != screenId)
            return;

        view.enabled = false;
    }

    public void ShowScreenHandler(ScreenID screenId)
    {
        if (GetScreenID() != screenId)
            return;

        view.enabled = true;
    }

    protected virtual void OnReset(ScreenID screenId)
    {
        if (GetScreenID() != screenId)
            return;

    }
}