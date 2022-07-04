using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ToggleMenuButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private MenuType _menuType;
    private bool _isEnabled;

    public void Awake()
    {
        GetComponentInParent<Menu>().activeChanged += (isActive) => { _isEnabled = isActive; };
    }

    public void OnPointerClick(PointerEventData data)
    {
        if (!_isEnabled) return;
        ModuleManager.GetModule<MenuManager>().ToggleMenu(_menuType, true);
    }
}
