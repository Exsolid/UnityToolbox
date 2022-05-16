using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GotoMenu : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Canvas menu;

    public void OnPointerClick(PointerEventData data)
    {
        ModuleManager.get<MenuManager>().setActiveMenu(menu);
    }
}
