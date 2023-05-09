using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// A script which is placed on a canvas to be recognized by the <see cref="MenuManager"/>.
/// </summary>
public class Menu : MonoBehaviour
{
    public bool IsActive
    { 
        get { return _isActive; } 
        set { 
            _isActive = value; 
            OnActiveChanged?.Invoke(value); 
        } 
    }

    private bool _isActive;

    public event Action<bool> OnActiveChanged;

    public void Start()
    {
        if (!ModuleManager.ModuleRegistered<MenuManager>())
        {
            IsActive = true;
        }
    }
}
