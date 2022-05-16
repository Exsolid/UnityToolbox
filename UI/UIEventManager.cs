using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIEventManager : Module
{
    public Action menuWheelPrevious;
    public Action menuWheelNext;

    public Action<bool> togglePauseMenu;
    private bool isPaused;

    public void MenuWheelPrevious()
    {
        if(menuWheelPrevious != null)
        {
            menuWheelPrevious();
        }
    }
    public void MenuWheelNext()
    {
        if (menuWheelNext != null)
        {
            menuWheelNext();
        }
    }
    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        if (togglePauseMenu != null)
        {
            togglePauseMenu(isPaused);
        }
    }
}