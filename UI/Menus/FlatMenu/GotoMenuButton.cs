using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GotoMenuButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Canvas menu;
    [SerializeField] private Canvas parentCanvas;

    public void OnPointerClick(PointerEventData data)
    {
        if(parentCanvas.enabled) ModuleManager.get<MenuManager>().setActiveMenu(menu);
    }
}
