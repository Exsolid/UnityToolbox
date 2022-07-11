using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuWheelNavigation : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] MenuWheelNavigationTypes _direction;
    private bool _isEnabled;

    public void Awake()
    {
        GetComponentInParent<Menu>().OnActiveChanged += (isActive) => { _isEnabled = isActive; };
    }

    public void OnPointerClick(PointerEventData data)
    {
        if (!_isEnabled) return;
        switch (_direction)
        {
            case MenuWheelNavigationTypes.Next:
                ModuleManager.GetModule<UIEventManager>().MenuWheelNext();
                break;
            case MenuWheelNavigationTypes.Previous:
                ModuleManager.GetModule<UIEventManager>().MenuWheelPrevious();
                break;
        }
    }
}

enum MenuWheelNavigationTypes
{
    Next,
    Previous
}