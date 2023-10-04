using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityToolbox.General.Management;
using UnityToolbox.General.Preferences;
using UnityToolbox.UI.Localization;

namespace UnityToolbox.UI.Settings.Language
{
    /// <summary>
    /// A script which displays the language settings.
    /// Requires the <see cref="UIEventManager"/> and the <see cref="SettingsManager"/>.
    /// <see cref="LanguageSettingButton"/> is required to change its value.
    /// </summary>
    public class LanguageSetting : MonoBehaviour
    {
        private HashSet<LocalizationLanguage> _LocalizationLanguages;
        private int _currentLanguage;
        private Text _displayText;

        private string _languagePref;

        private void Start()
        {
            _languagePref = ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.LANGUAGE);
            _displayText = GetComponent<Text>();
            _currentLanguage = PlayerPrefs.GetInt(_languagePref);

            if (!Localizzer.Instance.IsInitialized)
            {
                Localizzer.Instance.Initialize();
            }
            _LocalizationLanguages = Localizzer.Instance.LocalizationLanguages;

            if (_currentLanguage >= _LocalizationLanguages.Count)
            {
                _currentLanguage = 0;
            }

            _displayText.text = _LocalizationLanguages.ElementAt(_currentLanguage).Name;
            ModuleManager.GetModule<UIEventManager>().OnLanguageNext += NextLanguage;
            ModuleManager.GetModule<UIEventManager>().OnLanguagePrevious += PreviousLanguage;
        }

        private void OnDestroy()
        {
            if (ModuleManager.ModuleRegistered<UIEventManager>())
            {
                ModuleManager.GetModule<UIEventManager>().OnLanguageNext -= NextLanguage;
                ModuleManager.GetModule<UIEventManager>().OnLanguagePrevious -= PreviousLanguage;
            }
        }

        private void NextLanguage()
        {
            if (_currentLanguage == _LocalizationLanguages.Count - 1)
            {
                _currentLanguage = 0;
            }
            else
            {
                _currentLanguage++;
            }

            _displayText.text = _LocalizationLanguages.ElementAt(_currentLanguage).Name;
            ModuleManager.GetModule<UIEventManager>().LanguageUpdated(_LocalizationLanguages.ElementAt(_currentLanguage));
            PlayerPrefs.SetInt(_languagePref, _currentLanguage);
        }

        private void PreviousLanguage()
        {
            if (_currentLanguage == 0)
            {
                _currentLanguage = _LocalizationLanguages.Count - 1;
            }
            else
            {
                _currentLanguage--;
            }

            _displayText.text = _LocalizationLanguages.ElementAt(_currentLanguage).Name;
            ModuleManager.GetModule<UIEventManager>().LanguageUpdated(_LocalizationLanguages.ElementAt(_currentLanguage));
            PlayerPrefs.SetInt(_languagePref, _currentLanguage);
        }
    } 
}
