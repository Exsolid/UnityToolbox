using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ZoomMenuItemTriggerButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool inMenu;
    [SerializeField] private ZoomMenuItem _zoom;
    private bool _isEnabled;

    public void Awake()
    {
        GetComponentInParent<MenuEnable>().activeChanged += (isActive) => { _isEnabled = isActive; };
    }

    public void OnPointerClick(PointerEventData data)
    {
        if (!(inMenu ^ _isEnabled))
        {
            if(inMenu) _zoom.ZoomOut();
            else _zoom.ZoomIn();
            GetComponentInParent<MenuEnable>().IsActive = !_isEnabled;
        }
    }
}
