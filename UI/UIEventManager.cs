using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class UIEventManager : Module
{
    public event Action OnMenuWheelPrevious;
    public event Action OnMenuWheelNext;

    public event Action<bool, MenuType> OnTogglePaused;

    public event Action<DialogNode> OnDialogNodeChanged;

    public event Action<bool> OnBindingKey;

    public void MenuWheelPrevious()
    {
        OnMenuWheelPrevious?.Invoke();
    }

    public void MenuWheelNext()
    {
        OnMenuWheelNext?.Invoke();
    }

    public void TogglePaused(bool isPaused, MenuType type)
    {
        OnTogglePaused?.Invoke(isPaused, type);
    }

    public void DialogNodeChanged(DialogNode currentNode)
    {
        OnDialogNodeChanged?.Invoke(currentNode);
    }

    public void BindingKey(bool isBindingKey)
    {
        OnBindingKey?.Invoke(isBindingKey);
    }
}