using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class UIEventManager : Module
{
    public Action menuWheelPrevious;
    public Action menuWheelNext;

    public Action<bool, MenuType> togglePaused;

    public Action<DialogNode> dialogNodeChanged;

    public Action<bool> bindingKey;

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

    public void TogglePaused(bool isPaused, MenuType type)
    {
        if (togglePaused != null)
        {
            togglePaused(isPaused, type);
        }
    }

    public void DialogNodeChanged(DialogNode currentNode)
    {
        if (dialogNodeChanged != null)
        {
            dialogNodeChanged(currentNode);
        }
    }

    public void BindingKey(bool isBindingKey)
    {
        if (bindingKey != null)
        {
            bindingKey(isBindingKey);
        }
    }
}