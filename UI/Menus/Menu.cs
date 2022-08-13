using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    [SerializeField] private bool _mayUserToogle;
    public bool MayUserToogle 
    { 
        get { return _mayUserToogle; } 
        set { _mayUserToogle = value;}
    }

    public event Action<bool> OnActiveChanged;
}
