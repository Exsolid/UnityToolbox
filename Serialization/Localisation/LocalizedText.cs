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

    private void Start()
    {
        //Todo selected language
    
    }

    public void OnValidate()
    {
        if (!Localizer.Instance.IsInitialized)
        {
            string assetPathInProject = ProjectPrefs.GetString("LocalisationPath");
            Localizer.Instance.AssetPathInProject = assetPathInProject;
            Localizer.Instance.Initialize();
        }
        if (_allLanguages == null)
        {
            _allLanguages = Localizer.Instance.LocalisationLanguages.ToList();
            _allLanguagesString = Localizer.Instance.LocalisationLanguages.Select(x => x.Name).ToList();
        }

        KeyValuePair<LocalisationID, Dictionary<LocalisationLanguage, string>> temp = Localizer.Instance.LocalisationData.Where(e => e.Key.Equals(_localisationID)).FirstOrDefault();
        _displayedString = temp.Value == null ? "LocalisationID not valid!" : temp.Value[_allLanguages.ElementAt(_selectedLanuage)];

        _textToDisplay = GetComponent<Text>();
        _textToDisplay.text = _displayedString;
    }
}
