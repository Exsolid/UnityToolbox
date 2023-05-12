using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// This script is placed on a <see cref="Text"/> component and swaps out the localisation in editor and runtime.
/// </summary>
public class LocalizedText : MonoBehaviour
{
    [SerializeField] private LocalisationID _localisationID;
    [SerializeField] [ReadOnly] private string _displayedString;
    private List<LocalisationLanguage> _allLanguages;
    private List<string> _allLanguagesString;
    [SerializeField] [DropDown(nameof(_allLanguagesString))] private int _selectedLanuage;

    private Text _textToDisplay;

    private string _languagePref;

    private void Start()
    {
        if (!Localizer.Instance.IsInitialized)
        {
            Localizer.Instance.Initialize();
        }

        _allLanguages = Localizer.Instance.LocalisationLanguages.ToList();
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
        KeyValuePair<LocalisationID, Dictionary<LocalisationLanguage, string>> temp = Localizer.Instance.LocalisationData.Where(e => e.Key.Equals(_localisationID)).FirstOrDefault();
        _displayedString = temp.Value == null ? "LocalisationID not valid!" : temp.Value[_allLanguages.ElementAt(atIndex)];

        _textToDisplay = GetComponent<Text>();
        _textToDisplay.text = _displayedString;
    }

    private void UpdateText(LocalisationLanguage language)
    {
        KeyValuePair<LocalisationID, Dictionary<LocalisationLanguage, string>> temp = Localizer.Instance.LocalisationData.Where(e => e.Key.Equals(_localisationID)).FirstOrDefault();
        _displayedString = temp.Value == null ? "LocalisationID not valid!" : temp.Value[language];

        _textToDisplay = GetComponent<Text>();
        _textToDisplay.text = _displayedString;
    }

    public void OnValidate()
    {
        if (!Localizer.Instance.IsInitialized)
        {
            Localizer.Instance.Initialize();
        }
        if (_allLanguages == null || Localizer.Instance.LocalisationLanguages.Count != _allLanguages.Count)
        {
            _allLanguages = Localizer.Instance.LocalisationLanguages.ToList();
            _allLanguagesString = Localizer.Instance.LocalisationLanguages.Select(x => x.Name).ToList();
        }

        if(_selectedLanuage >= _allLanguages.Count)
        {
            _selectedLanuage = 0;
        }

        UpdateText(_selectedLanuage);
    }
}
