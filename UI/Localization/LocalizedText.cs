using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityToolbox.General.Attributes;
using UnityToolbox.General.Management;
using UnityToolbox.General.Preferences;

namespace UnityToolbox.UI.Localization
{
    /// <summary>
    /// This script is placed on a <see cref="Text"/> component and swaps out the Localization in editor and runtime.
    /// </summary>
    public class LocalizzedText : MonoBehaviour
    {
        [SerializeField] private LocalizationID _LocalizationID;
        [SerializeField][ReadOnly] private string _displayedString;
        private List<LocalizationLanguage> _allLanguages;
        private List<string> _allLanguagesString;
        [SerializeField][DropDown(nameof(_allLanguagesString))] private int _selectedLanguage;

        private Text _textToDisplay;

        private string _languagePref;

        private void Start()
        {
            if (!Localizzer.Instance.IsInitialized)
            {
                Localizzer.Instance.Initialize();
            }

            _allLanguages = Localizzer.Instance.LocalizationLanguages.ToList();
            _languagePref = ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.LANGUAGE);
            int currentIngameLanguage = PlayerPrefs.GetInt(_languagePref);

            if (currentIngameLanguage >= _allLanguages.Count)
            {
                currentIngameLanguage = 0;
            }

            ModuleManager.GetModule<UIEventManager>().OnLanguageUpdated += UpdateText;
            UpdateText(currentIngameLanguage);
        }

        private void OnDestroy()
        {
            if (ModuleManager.ModuleRegistered<UIEventManager>())
            {
                ModuleManager.GetModule<UIEventManager>().OnLanguageUpdated -= UpdateText;
            }
        }

        private void UpdateText(int atIndex)
        {
            KeyValuePair<LocalizationID, Dictionary<LocalizationLanguage, string>> temp = Localizzer.Instance.LocalizationData.Where(e => e.Key.Equals(_LocalizationID)).FirstOrDefault();
            _displayedString = temp.Value == null ? "LocalizationID not valid!" : temp.Value[_allLanguages.ElementAt(atIndex)];

            _textToDisplay = GetComponent<Text>();
            _textToDisplay.text = _displayedString;
        }

        private void UpdateText(LocalizationLanguage language)
        {
            KeyValuePair<LocalizationID, Dictionary<LocalizationLanguage, string>> temp = Localizzer.Instance.LocalizationData.Where(e => e.Key.Equals(_LocalizationID)).FirstOrDefault();
            _displayedString = temp.Value == null ? "LocalizationID not valid!" : temp.Value[language];

            _textToDisplay = GetComponent<Text>();
            _textToDisplay.text = _displayedString;
        }

        public void OnValidate()
        {
            if (!Localizzer.Instance.IsInitialized)
            {
                Localizzer.Instance.Initialize();
            }
            if (_allLanguages == null || Localizzer.Instance.LocalizationLanguages.Count != _allLanguages.Count)
            {
                _allLanguages = Localizzer.Instance.LocalizationLanguages.ToList();
                _allLanguagesString = Localizzer.Instance.LocalizationLanguages.Select(x => x.Name).ToList();
            }

            if (_selectedLanguage >= _allLanguages.Count)
            {
                _selectedLanguage = 0;
            }

            UpdateText(_selectedLanguage);
        }
    } 
}
