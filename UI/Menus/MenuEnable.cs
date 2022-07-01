using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuEnable : MonoBehaviour
{
    public bool IsActive { get { return _isActive; } set { _isActive = value; if (activeChanged != null) activeChanged(value); } }
    private bool _isActive;

    public Action<bool> activeChanged;
}
