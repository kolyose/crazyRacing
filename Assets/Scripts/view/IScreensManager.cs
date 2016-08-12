using UnityEngine;
using System.Collections.Generic;

public interface IScreensManager
{
    void InitScreens();
    void ShowLoginScreen();
    void HideLoginScreen();
    void ShowSelectActionsScreen();
    void HideSelectActionsScreen();
    void ShowLoaderScreen();
    void HideLoaderScreen();   
}
