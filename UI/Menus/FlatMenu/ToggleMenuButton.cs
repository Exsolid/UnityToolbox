using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ToggleMenuButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private MenuType menuType;

    public void OnPointerClick(PointerEventData data)
    {
        if (!parentCanvas.enabled) return;
        ModuleManager.GetModule<UIEventManager>().ToggleMenu(menuType);
    }
}
