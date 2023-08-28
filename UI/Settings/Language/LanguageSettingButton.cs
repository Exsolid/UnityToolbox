using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityToolbox.UI.Settings
{
    /// <summary>
    /// A script which handels the language settings. On click, the next language is set.
    /// Requires the <see cref="UIEventManager"/>, the <see cref="SettingsManager"/> and the a <see cref="LanguageSetting"/> to display.
    /// </summary>
    public class LanguageSettingButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private LanguageButtonDirection _direction;

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (_direction)
            {
                case LanguageButtonDirection.Previous:
                    ModuleManager.GetModule<UIEventManager>().LanguagePrevious();
                    break;
                case LanguageButtonDirection.Next:
                    ModuleManager.GetModule<UIEventManager>().LanguageNext();
                    break;
            }
        }

        private enum LanguageButtonDirection
        {
            Previous,
            Next
        }
    } 
}
