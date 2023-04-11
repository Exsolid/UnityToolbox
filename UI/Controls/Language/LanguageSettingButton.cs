using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A script which handels the language settings. On click, the next language is set.
/// Requires the <see cref="UIEventManager"/>, the <see cref="SettingsManager"/> and the a <see cref="LanguageSetting"/> to display.
/// </summary>
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
