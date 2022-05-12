using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuWheelNavigation : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] MenuWheelNavigationTypes direction;
    private void OnMouseUp()
    {
        switch (direction)
        {
            case MenuWheelNavigationTypes.Next:
                ModulManager.get<UIEventManager>().MenuWheelNext();
                break;
            case MenuWheelNavigationTypes.Previous:
                ModulManager.get<UIEventManager>().MenuWheelPrevious();
                break;
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        switch (direction)
        {
            case MenuWheelNavigationTypes.Next:
                ModulManager.get<UIEventManager>().MenuWheelNext();
                break;
            case MenuWheelNavigationTypes.Previous:
                ModulManager.get<UIEventManager>().MenuWheelPrevious();
                break;
        }
    }
}

enum MenuWheelNavigationTypes
{
    Next,
    Previous
}