using UnityEngine;
using System.Collections.Generic;

public interface IScreensManager
{
    void InitScreens();
    void ShowLoginScreen();
    void HideLoginScreen();
    void ShowSelectActionsScreen(uint distance);
    void HideSelectActionsScreen();
    void ShowLoaderScreen();
    void HideLoaderScreen();   
}
