using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuWheelNavigation : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] MenuWheelNavigationTypes direction;
    public void OnMouseUp()
    {
        switch (direction)
        {
            case MenuWheelNavigationTypes.Next:
                ModuleManager.get<UIEventManager>().MenuWheelNext();
                break;
            case MenuWheelNavigationTypes.Previous:
                ModuleManager.get<UIEventManager>().MenuWheelPrevious();
                break;
        }
    }

    public void OnPointerClick(PointerEventData data)
    {
        switch (direction)
        {
            case MenuWheelNavigationTypes.Next:
                ModuleManager.get<UIEventManager>().MenuWheelNext();
                break;
            case MenuWheelNavigationTypes.Previous:
                ModuleManager.get<UIEventManager>().MenuWheelPrevious();
                break;
        }
    }
}

enum MenuWheelNavigationTypes
{
    Next,
    Previous
}