using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIEventManager : Module
{
    public Action menuWheelPrevious;
    public Action menuWheelNext;

    public Action<bool, MenuType> toggleMenu;
    private bool _isPaused;
    private MenuType _currentPausedType;

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

    public void ToggleMenu(MenuType toggledType)
    {
        if (toggleMenu != null && ((toggledType.Equals(_currentPausedType) && _isPaused) || !_isPaused))
        {
            _isPaused = !_isPaused;
            _currentPausedType = toggledType;
            toggleMenu(_isPaused, toggledType);
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

public enum MenuType
{
    Pause,
    Inventory
}