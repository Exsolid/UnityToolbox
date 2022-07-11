using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QuitButton : MonoBehaviour, IPointerDownHandler
{
    private bool _isEnabled;

    public void Awake()
    {
        GetComponentInParent<Menu>().OnActiveChanged += (isActive) => 
        {
            _isEnabled = isActive;
        };
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (_isEnabled)
        {
            Application.Quit();
        }
    }
}
