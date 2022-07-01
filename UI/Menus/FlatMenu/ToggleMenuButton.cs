using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ToggleMenuButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private MenuType menuType;
    private bool _isEnabled;

    public void Awake()
    {
        GetComponentInParent<MenuEnable>().activeChanged += (isActive) => { _isEnabled = isActive; };
    }

    public void OnPointerClick(PointerEventData data)
    {
        if (!_isEnabled) return;
        ModuleManager.GetModule<UIEventManager>().ToggleMenu(menuType);
    }
}
