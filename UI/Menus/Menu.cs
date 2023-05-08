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
    [SerializeField] private bool _mayToggleWithControl;

    /// <summary>
    /// Defines whether the player is able to activate the menu by themselves. (e.g. Interaction)
    /// </summary>
    public bool MayUserToogle 
    { 
        get { return _mayToggleWithControl; } 
        set { _mayToggleWithControl = value;}
    }

    public event Action<bool> OnActiveChanged;

    public void Start()
    {
        if (!ModuleManager.ModuleRegistered<MenuManager>())
        {
            IsActive = true;
        }
    }
}
