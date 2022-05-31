using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ToggleMenuButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Canvas parentCanvas;

    public void OnPointerClick(PointerEventData data)
    {
        if (!parentCanvas.enabled) return;
        ModuleManager.get<UIEventManager>().TogglePauseMenu();
    }
}
