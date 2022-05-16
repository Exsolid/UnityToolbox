using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class ToggleMenuButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData data)
    {
        ModuleManager.get<UIEventManager>().TogglePauseMenu();
    }
}
