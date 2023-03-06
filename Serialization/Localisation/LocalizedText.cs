using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LocalizedText : MonoBehaviour
{
    [SerializeField] private LocalisationID _localisationID;
    [SerializeField] [ReadOnly] private string _displayedString;
    private List<LocalisationLanguage> _allLanguages;
    private List<string> _allLanguagesString;
    [SerializeField] [DropDown(nameof(_allLanguagesString))] private int _selectedLanuage;

    private Text _textToDisplay;

    private string _languagePref;
    private int _currentIngameLanguage;

    private void Start()
    {
        if (!Localizer.Instance.IsInitialized)
        {
            string assetPathInProject = ProjectPrefs.GetString("LocalisationPath");
            Localizer.Instance.AssetPathInProject = assetPathInProject;
            Localizer.Instance.Initialize();
        }

        _languagePref = ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.LANGUAGE);
        _currentIngameLanguage = PlayerPrefs.GetInt(_languagePref);

        if (_currentIngameLanguage >= _allLanguages.Count)
        {
            _currentIngameLanguage = 0;
        }

        ModuleManager.GetModule<UIEventManager>().OnLanguageNext += NextLanguage;
        ModuleManager.GetModule<UIEventManager>().OnLanguagePrevious += PreviousLanguage;
        UpdateText(_currentIngameLanguage);
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
        if (_currentIngameLanguage == _allLanguages.Count - 1)
        {
            _currentIngameLanguage = 0;
        }
        else
        {
            _currentIngameLanguage++;
        }

        UpdateText(_currentIngameLanguage);
    }

    private void PreviousLanguage()
    {
        if (_currentIngameLanguage == 0)
        {
            _currentIngameLanguage = _allLanguages.Count - 1;
        }
        else
        {
            _currentIngameLanguage--;
        }

        UpdateText(_currentIngameLanguage);
    }

    private void UpdateText(int atIndex)
    {
        KeyValuePair<LocalisationID, Dictionary<LocalisationLanguage, string>> temp = Localizer.Instance.LocalisationData.Where(e => e.Key.Equals(_localisationID)).FirstOrDefault();
        _displayedString = temp.Value == null ? "LocalisationID not valid!" : temp.Value[_allLanguages.ElementAt(atIndex)];

        _textToDisplay = GetComponent<Text>();
        _textToDisplay.text = _displayedString;
    }

    public void OnValidate()
    {
        if (!Localizer.Instance.IsInitialized)
        {
            string assetPathInProject = ProjectPrefs.GetString("LocalisationPath");
            Localizer.Instance.AssetPathInProject = assetPathInProject;
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
