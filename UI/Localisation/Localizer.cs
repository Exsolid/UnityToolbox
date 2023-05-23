using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;

/// <summary>
/// The heart of the localisation system. Every localisation, its serialisation and editing is managed here.
/// </summary>
public class Localizer
{
    public event Action<LocalisationScope> ScopeEdited;
    public event Action<LocalisationLanguage> LanguageEdited;
    public event Action<LocalisationID> LocalisationIDEdited;

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


    private readonly string _dataPath = "LocalisationData.txt";
    private readonly string _languagesDataPath = "LocalisationLanguages.txt";
    private readonly string _scopesDataPath = "LocalisationScopes.txt";

    private bool _isInitialized;
    public bool IsInitialized { get { return _isInitialized; } }
    private JsonSerializerSettings _settings;

    /// <summary>
    /// Adds a new localisation to the system.
    /// </summary>
    /// <param name="localisationID"></param>
    /// <param name="localisations"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Adds a new <see cref="LocalisationScope"/> to the system.
    /// </summary>
    /// <param name="scopeName"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Adds a new <see cref="LocalisationLanguage"/> to the system.
    /// </summary>
    /// <param name="languageName"></param>
    /// <param name="languageShortName"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Removes a <see cref="LocalisationLanguage"/> from the system.
    /// </summary>
    /// <param name="language"></param>
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

    /// <summary>
    /// Removes a <see cref="LocalisationScope"/> from the system.
    /// </summary>
    /// <param name="scope"></param>
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

    /// <summary>
    /// Removes a localisation, that is a <see cref="LocalisationID"/> with all its content, from the system.
    /// </summary>
    /// <param name="localisationID"></param>
    public void RemoveLocalisation(LocalisationID localisationID)
    {
        if (!_isInitialized)
        {
            Initialize();
        }

        _localisationData.Remove(localisationID);
        OnLocalisationEdited(localisationID);
    }

    /// <summary>
    /// Edits an existing <see cref="LocalisationID"/> by replacing the IDs, but keeping its content.
    /// </summary>
    /// <param name="oldID"></param>
    /// <param name="newID"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Replaces a <see cref="LocalisationScope"/> with a new one for all available data.
    /// </summary>
    /// <param name="oldScope"></param>
    /// <param name="newScopeName"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Replaces a <see cref="LocalisationLanguage"/> with a new one for all available data.
    /// </summary>
    /// <param name="oldLanguage"></param>
    /// <param name="newLanguageName"></param>
    /// <param name="newLanguageShortName"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Edits a given localisation found by the <see cref="LocalisationID"/> by replacing the current content with <paramref name="localisations"/>.
    /// </summary>
    /// <param name="localisationID"></param>
    /// <param name="localisations"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Writes the data to disk. For this to work, a valid path within the project must be setup.
    /// </summary>
    public void WriteData()
    {
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
            ResourcesUtil.WriteFile(ProjectPrefKeys.LOCALISATIONSAVEPATH, _dataPath, serializableDictionary);
            ResourcesUtil.WriteFile(ProjectPrefKeys.LOCALISATIONSAVEPATH, _languagesDataPath, _localisationLanguages);
            ResourcesUtil.WriteFile(ProjectPrefKeys.LOCALISATIONSAVEPATH, _scopesDataPath, _localisationScopes);
        }
    }

    /// <summary>
    /// Initializes the <see cref="Localizer"/>. This is required due to its dependency on a valid path.
    /// </summary>
    public void Initialize()
    {
        _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        _localisationData = new Dictionary<LocalisationID, Dictionary<LocalisationLanguage, string>>();
        _localisationScopes = new HashSet<LocalisationScope>();

        _defaultScope = new LocalisationScope();
        _defaultScope.Name = "DefaultScope";
        _localisationScopes.Add(_defaultScope);

        if (ProjectPrefs.GetString(ProjectPrefKeys.LOCALISATIONSAVEPATH) == null || ProjectPrefs.GetString(ProjectPrefKeys.LOCALISATIONSAVEPATH).Equals(""))
        {
            _isInitialized = false;
            return;
        }

        _isInitialized = true;

        _localisationLanguages = ResourcesUtil.GetFileData<HashSet<LocalisationLanguage>>(ProjectPrefKeys.LOCALISATIONSAVEPATH, _languagesDataPath);

        if (_localisationLanguages == null)
        {
            _localisationLanguages = new HashSet<LocalisationLanguage>();
            return;
        }

        List<KeyValuePair<LocalisationID, List<KeyValuePair<LocalisationLanguage, string>>>> serializableDictionary 
            = ResourcesUtil.GetFileData<List<KeyValuePair<LocalisationID, List<KeyValuePair<LocalisationLanguage, string>>>>>(ProjectPrefKeys.LOCALISATIONSAVEPATH, _dataPath);
        _localisationScopes = ResourcesUtil.GetFileData<HashSet<LocalisationScope>>(ProjectPrefKeys.LOCALISATIONSAVEPATH, _scopesDataPath);

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

    /// <summary>
    /// Callback for when a <see cref="LocalisationScope"/> is edited or removed.
    /// </summary>
    /// <param name="oldScope"></param>
    private void OnScopeEdited(LocalisationScope oldScope)
    {
        ScopeEdited?.Invoke(oldScope);
    }

    /// <summary>
    /// Callback for when a <see cref="LocalisationLanguage"/> is edited or removed.
    /// </summary>
    /// <param name="oldLanguage"></param>
    private void OnLanguageEdited(LocalisationLanguage oldLanguage)
    {
        LanguageEdited?.Invoke(oldLanguage);
    }

    /// <summary>
    /// Callback for when a <see cref="LocalisationID"/> is edited or removed.
    /// </summary>
    /// <param name="oldLocalisationID"></param>
    private void OnLocalisationEdited(LocalisationID oldLocalisationID)
    {
        LocalisationIDEdited?.Invoke(oldLocalisationID);
    }
}
