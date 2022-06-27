using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ZoomMenuItemTrigger : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool inMenu;
    [SerializeField] private ZoomMenuItem _zoom;
    private bool isMenuActive;

    private void Start()
    {
        isMenuActive = ModuleManager.get<MenuManager>().InMenu;
        ModuleManager.get<MenuManager>().inMenuChanged += (active) => { isMenuActive = active; };
    }

    public void OnPointerClick(PointerEventData data)
    {
        if (!(inMenu ^ isMenuActive))
        {
            if(inMenu) _zoom.ZoomOut();
            else _zoom.ZoomIn();
            ModuleManager.get<MenuManager>().InMenu = !isMenuActive;
        }
    }
}
