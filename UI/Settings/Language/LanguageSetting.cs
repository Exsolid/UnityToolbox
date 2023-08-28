using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityToolbox.UI.Localisation;

namespace UnityToolbox.UI.Settings
{
    /// <summary>
    /// A script which displays the language settings.
    /// Requires the <see cref="UIEventManager"/> and the <see cref="SettingsManager"/>.
    /// <see cref="LanguageSettingButton"/> is required to change its value.
    /// </summary>
    public class LanguageSetting : MonoBehaviour
    {
        private HashSet<LocalisationLanguage> _localisationLanguages;
        private int _currentLanguage;
        private Text _displayText;

        private string _languagePref;

        private void Start()
        {
            _languagePref = ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.LANGUAGE);
            _displayText = GetComponent<Text>();
            _currentLanguage = PlayerPrefs.GetInt(_languagePref);

            if (!Localizer.Instance.IsInitialized)
            {
                Localizer.Instance.Initialize();
            }
            _localisationLanguages = Localizer.Instance.LocalisationLanguages;

            if (_currentLanguage >= _localisationLanguages.Count)
            {
                _currentLanguage = 0;
            }

            _displayText.text = _localisationLanguages.ElementAt(_currentLanguage).Name;
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
            if (_currentLanguage == _localisationLanguages.Count - 1)
            {
                _currentLanguage = 0;
            }
            else
            {
                _currentLanguage++;
            }

            _displayText.text = _localisationLanguages.ElementAt(_currentLanguage).Name;
            ModuleManager.GetModule<UIEventManager>().LanguageUpdated(_localisationLanguages.ElementAt(_currentLanguage));
            PlayerPrefs.SetInt(_languagePref, _currentLanguage);
        }

        private void PreviousLanguage()
        {
            if (_currentLanguage == 0)
            {
                _currentLanguage = _localisationLanguages.Count - 1;
            }
            else
            {
                _currentLanguage--;
            }

            _displayText.text = _localisationLanguages.ElementAt(_currentLanguage).Name;
            ModuleManager.GetModule<UIEventManager>().LanguageUpdated(_localisationLanguages.ElementAt(_currentLanguage));
            PlayerPrefs.SetInt(_languagePref, _currentLanguage);
        }
    } 
}
