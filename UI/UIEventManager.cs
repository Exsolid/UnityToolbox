using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIEventManager : Modul
{
    public Action menuWheelPrevious;
    public Action menuWheelNext;

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
}