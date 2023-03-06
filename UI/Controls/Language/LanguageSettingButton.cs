using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LanguageSettingButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ButtonDirection _direction;

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (_direction)
        {
            case ButtonDirection.Previous:
                ModuleManager.GetModule<UIEventManager>().LanguagePrevious();
                break;
            case ButtonDirection.Next:
                ModuleManager.GetModule<UIEventManager>().LanguageNext();
                break;
        }
    }

    private enum ButtonDirection
    {
        Previous,
        Next
    }
}
