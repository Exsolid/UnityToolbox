using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;

public class Localizer
{
    public Action<LocalisationScope> ScopeEdited;
    public Action<LocalisationLanguage> LanguageEdited;
    public Action<LocalisationID> LocalisationIDEdited;

    private string _assetPathInProject;
    public string AssetPathInProject
    {
        set
        {
            if (value.Contains(Application.dataPath))
            {
                _assetPathInProject = value;
            }
            else if(!value.Contains(":"))
            {
                _assetPathInProject = Application.dataPath + value;
            }
        }
    }

    private static Localizer _instance;
    public static Localizer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Localizer();
            }
            return _instance;
        }
    }

    private Dictionary<LocalisationID, Dictionary<LocalisationLanguage, string>> _localisationData;
    public Dictionary<LocalisationID, Dictionary<LocalisationLanguage, string>> LocalisationData
    {
        get
        {
            return _localisationData.ToDictionary(entry => entry.Key, entry => entry.Value);
        }
    }

    private HashSet<LocalisationLanguage> _localisationLanguages;
    public HashSet<LocalisationLanguage> LocalisationLanguages
    {
        get
        {
            return _localisationLanguages.ToHashSet();
        }
    }

    private HashSet<LocalisationScope> _localisationScopes;
    public HashSet<LocalisationScope> LocalisationScopes
    {
        get
        {
            return _localisationScopes.ToHashSet();
        }
    }

    private LocalisationScope _defaultScope;
    public LocalisationScope DefaultScope
    {
        get { return _defaultScope; }
    }


    private readonly string _dataPath = "/LocalisationData.dat";
    private readonly string _languagesDataPath = "/LocalisationLanguages.dat";
    private readonly string _scopesDataPath = "/LocalisationScopes.dat";

    private bool _isInitialized;
    public bool IsInitialized { get { return _isInitialized; } }
    private JsonSerializerSettings _settings;

    public bool AddLocalisation(LocalisationID localisationID, Dictionary<LocalisationLanguage, string> localisations)
    {
        if (!_isInitialized)
        {
            Initialize();
        }

        if (localisations == null)
        {
            Debug.LogError("The " + nameof(Localizer) + " cannot add null as " + nameof(localisations));
            return false;
        }

        if(_localisationLanguages.Count != localisations.Count)
        {
            Debug.LogError("The " + nameof(Localizer) + " knows " + _localisationLanguages.Count + " languages, but " + localisations.Count + " were given.");
            return false;
        }

        if (_localisationData.ContainsKey(localisationID))
        {
            Debug.LogError("The " + nameof(Localizer) + " already contains localisations for the ID " + localisationID +".");
            return false;
        }

        foreach(LocalisationLanguage langInData in _localisationLanguages)
        {
            if (!localisations.ContainsKey(langInData))
            {
                Debug.LogError("The " + nameof(Localizer) + " did not find the localisation for the language '" + langInData.Name);
                return false;
            }
        }
        _localisationData.Add(localisationID, localisations);
        return true;
    }

    public bool AddScope(string scopeName)
    {
        if (!_isInitialized)
        {
            Initialize();
        }

        if(scopeName == null || scopeName.Trim() == "")
        {
            return false;
        }

        LocalisationScope newScope = new LocalisationScope();
        newScope.Name = scopeName.Trim();

        if (newScope.Equals(_defaultScope) || _localisationScopes.Contains(newScope))
        {
            return false;
        }
        else
        {
            _localisationScopes.Add(newScope);
        }
        return true;
    }

    public bool AddLanguage(string languageName, string languageShortName)
    {
        if (!_isInitialized)
        {
            Initialize();
        }

        if (languageName == null || languageName.Trim() == "" || languageShortName == null || languageShortName.Trim() == "")
        {
            return false;
        }

        LocalisationLanguage newLang = new LocalisationLanguage();
        newLang.Name = languageName.Trim();
        newLang.ShortName = languageShortName.Trim();

        if (_localisationLanguages.Contains(newLang))
        {
            return false;
        }
        else
        {
            _localisationLanguages.Add(newLang);
        }
        return true;
    }

    public void RemoveLanguage(LocalisationLanguage language)
    {
        if (!_isInitialized)
        {
            Initialize();
        }

        foreach (KeyValuePair<LocalisationID, Dictionary<LocalisationLanguage, string>> pair
            in _localisationData)
        {
            _localisationData[pair.Key].Remove(language);
        }
        OnLanguageEdited(language);
        _localisationLanguages.Remove(language); 
    }

    public void RemoveScope(LocalisationScope scope)
    {
        if (!_isInitialized)
        {
            Initialize();
        }

        if (scope.Equals(_defaultScope))
        {
            return;
        }

        foreach (KeyValuePair<LocalisationID, Dictionary<LocalisationLanguage, string>> pair
            in _localisationData.Where(locaSet => locaSet.Key.Scope.Equals(scope)).ToList())
        {
            LocalisationID newID = new LocalisationID();
            newID.Scope = _defaultScope;
            newID.Name = pair.Key.Name;

            _localisationData.Remove(pair.Key);
            _localisationData.Add(newID, pair.Value);
        }

        OnScopeEdited(scope);
        _localisationScopes.Remove(scope);
    }

    public void RemoveLocalisation(LocalisationID localisationID)
    {
        if (!_isInitialized)
        {
            Initialize();
        }

        _localisationData.Remove(localisationID);
        OnLocalisationEdited(localisationID);
    }

    public bool EditLocalisationID(LocalisationID oldID, LocalisationID newID)
    {
        if (!_isInitialized)
        {
            Initialize();
        }

        if (!_localisationData.ContainsKey(newID))
        {
            _localisationData.Add(newID, _localisationData[oldID]);
            _localisationData.Remove(oldID);
        }
        else
        {
            return false;
        }

        OnLocalisationEdited(oldID);
        return true;
    }

    public bool EditScope(LocalisationScope oldScope, string newScopeName)
    {
        if (!_isInitialized)
        {
            Initialize();
        }

        if(newScopeName == null || newScopeName.Trim() == "")
        {
            return false;
        }

        LocalisationScope newScope = new LocalisationScope();
        newScope.Name = newScopeName.Trim();

        if (newScope.Equals(_defaultScope) || _localisationScopes.Contains(newScope) || !_localisationScopes.Contains(oldScope))
        {
            return false;
        }
        else
        {
            _localisationScopes.Add(newScope);
            OnScopeEdited(oldScope);
            _localisationScopes.Remove(oldScope);
            foreach (KeyValuePair<LocalisationID, Dictionary<LocalisationLanguage, string>> pair
                in _localisationData.Where(locaSet => locaSet.Key.Scope.Equals(oldScope)).ToList())
            {
                LocalisationID newID = new LocalisationID();
                newID.Scope = newScope;
                newID.Name = pair.Key.Name;

                _localisationData.Remove(pair.Key);
                _localisationData.Add(newID, pair.Value);
            }
        }
        return true;
    }

    public bool EditLanguage(LocalisationLanguage oldLanguage, string newLanguageName, string newLanguageShortName)
    {
        if (!_isInitialized)
        {
            Initialize();
        }

        if (newLanguageName == null || newLanguageName.Trim() == "" || newLanguageShortName == null || newLanguageShortName.Trim() == "" || !_localisationLanguages.Contains(oldLanguage))
        {
            return false;
        }

        LocalisationLanguage newLang = new LocalisationLanguage();
        newLang.Name = newLanguageName.Trim();
        newLang.ShortName = newLanguageShortName.Trim();

        if (_localisationLanguages.Contains(newLang))
        {
            return false;
        }
        else
        {

            _localisationLanguages.Remove(oldLanguage);
            _localisationLanguages.Add(newLang);
            foreach (KeyValuePair<LocalisationID, Dictionary<LocalisationLanguage, string>> pair
                in _localisationData)
            {
                _localisationData[pair.Key].Add(newLang, _localisationData[pair.Key][oldLanguage]);
                _localisationData[pair.Key].Remove(oldLanguage);
            }
            OnLanguageEdited(oldLanguage);
        }

        return true;
    } 

    public bool EditLocalisation(LocalisationID localisationID, Dictionary<LocalisationLanguage, string> localisations)
    {
        if (!_isInitialized)
        {
            Initialize();
        }

        if (localisations == null)
        {
            Debug.LogError("The " + nameof(Localizer) + " cannot edit null as " + nameof(localisations));
            return false;
        }

        if (_localisationLanguages.Count != localisations.Count)
        {
            Debug.LogError("The " + nameof(Localizer) + " knows " + _localisationLanguages.Count + " languages, but " + localisations.Count + " were given.");
            return false;
        }

        if (!_localisationData.ContainsKey(localisationID))
        {
            Debug.LogError("The " + nameof(Localizer) + " does not contain localisations for the ID " + localisationID + ".");
            return false;
        }

        foreach (LocalisationLanguage langInData in _localisationLanguages)
        {
            if (!localisations.ContainsKey(langInData))
            {
                Debug.LogError("The " + nameof(Localizer) + " did not find the localisation for the language '" + langInData.Name);
                return false;
            }
        }

        _localisationData[localisationID] = localisations;
        OnLocalisationEdited(localisationID);
        return true;
    }

    public void WriteData()
    {
        if (!Directory.Exists(_assetPathInProject))
        {
            Debug.LogError("The path '" + _assetPathInProject + "could not be found! The " + nameof(Localizer) + " cannot work without it.");
            return;
        }

        if (!_isInitialized)
        {
            Initialize();
        }

        if(_localisationData != null)
        {
            List<KeyValuePair<LocalisationID, List<KeyValuePair<LocalisationLanguage, string>>>> serializableDictionary = new List<KeyValuePair<LocalisationID, List<KeyValuePair<LocalisationLanguage, string>>>>();

            foreach (KeyValuePair<LocalisationID, Dictionary<LocalisationLanguage, string>> pair in _localisationData)
            {
                serializableDictionary.Add(new KeyValuePair<LocalisationID, List<KeyValuePair<LocalisationLanguage, string>>>(pair.Key, new List<KeyValuePair<LocalisationLanguage, string>>()));
                foreach (KeyValuePair<LocalisationLanguage, string> locaPair in pair.Value)
                {
                    serializableDictionary[serializableDictionary.Count-1].Value.Add(locaPair);
                }
            }

            string localisationData = JsonConvert.SerializeObject(serializableDictionary, _settings);
            string localisationLanguages = JsonConvert.SerializeObject(_localisationLanguages, _settings);
            string localisationScopes = JsonConvert.SerializeObject(_localisationScopes, _settings);
            File.WriteAllText(_assetPathInProject + _dataPath, localisationData);
            File.WriteAllText(_assetPathInProject + _languagesDataPath, localisationLanguages);
            File.WriteAllText(_assetPathInProject + _scopesDataPath, localisationScopes);
        }
    }

    public void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        _localisationData = new Dictionary<LocalisationID, Dictionary<LocalisationLanguage, string>>();

        _defaultScope = new LocalisationScope();
        _defaultScope.Name = "DefaultScope";
        _localisationScopes = new HashSet<LocalisationScope>();
        _localisationScopes.Add(_defaultScope);

        _localisationLanguages = new HashSet<LocalisationLanguage>();

        if (!Directory.Exists(_assetPathInProject))
        {
            Debug.LogError("The path '" + _assetPathInProject + "' could not be found! The " + nameof(Localizer) + " cannot work without it.");
            return;
        }

        _isInitialized = true;

        if (!File.Exists(_assetPathInProject + _dataPath))
        {
            return;
        }

        string localisationData = File.ReadAllText(_assetPathInProject + _dataPath);
        string localisationLanguages = File.ReadAllText(_assetPathInProject + _languagesDataPath);
        string localisationScopes = File.ReadAllText(_assetPathInProject + _scopesDataPath);
        List<KeyValuePair<LocalisationID, List<KeyValuePair<LocalisationLanguage, string>>>> serializableDictionary = JsonConvert.DeserializeObject<List<KeyValuePair<LocalisationID, List<KeyValuePair<LocalisationLanguage, string>>>>>(localisationData, _settings);
        _localisationLanguages = JsonConvert.DeserializeObject<HashSet<LocalisationLanguage>>(localisationLanguages, _settings);
        _localisationScopes = JsonConvert.DeserializeObject<HashSet<LocalisationScope>>(localisationScopes, _settings);

        foreach (KeyValuePair<LocalisationID, List<KeyValuePair<LocalisationLanguage, string>>> pair in serializableDictionary)
        {
            _localisationData.Add(pair.Key, new Dictionary<LocalisationLanguage, string>());
            foreach (KeyValuePair<LocalisationLanguage, string> locaPair in pair.Value)
            {
                _localisationData[pair.Key].Add(locaPair.Key, locaPair.Value);
            }
        }
    }

    //Events

    private void OnScopeEdited(LocalisationScope oldScope)
    {
        ScopeEdited?.Invoke(oldScope);
    }

    private void OnLanguageEdited(LocalisationLanguage oldLanguage)
    {
        LanguageEdited?.Invoke(oldLanguage);
    }

    private void OnLocalisationEdited(LocalisationID oldLocalisationID)
    {
        LocalisationIDEdited?.Invoke(oldLocalisationID);
    }
}
