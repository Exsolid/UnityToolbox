using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityToolbox.General.Management;
using UnityToolbox.General.Preferences;

namespace UnityToolbox.UI.Localization
{
    /// <summary>
    /// The heart of the Localization system. Every Localization, its serialisation and editing is managed here.
    /// </summary>
    public class Localizzer
    {
        public event Action<LocalizationScope> ScopeEdited;
        public event Action<LocalizationLanguage> LanguageEdited;
        public event Action<LocalizationID> LocalizationIDEdited;

        private static Localizzer _instance;
        public static Localizzer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Localizzer();
                }
                return _instance;
            }
        }

        private Dictionary<LocalizationID, Dictionary<LocalizationLanguage, string>> _LocalizationData;
        public Dictionary<LocalizationID, Dictionary<LocalizationLanguage, string>> LocalizationData
        {
            get
            {
                return _LocalizationData.ToDictionary(entry => entry.Key, entry => entry.Value);
            }
        }

        private HashSet<LocalizationLanguage> _LocalizationLanguages;
        public HashSet<LocalizationLanguage> LocalizationLanguages
        {
            get
            {
                return _LocalizationLanguages.ToHashSet();
            }
        }

        private HashSet<LocalizationScope> _LocalizationScopes;
        public HashSet<LocalizationScope> LocalizationScopes
        {
            get
            {
                return _LocalizationScopes.ToHashSet();
            }
        }

        private LocalizationScope _defaultScope;
        public LocalizationScope DefaultScope
        {
            get { return _defaultScope; }
        }


        private readonly string _dataPath = "LocalizationData.txt";
        private readonly string _languagesDataPath = "LocalizationLanguages.txt";
        private readonly string _scopesDataPath = "LocalizationScopes.txt";

        private bool _isInitialized;
        public bool IsInitialized { get { return _isInitialized; } }
        private JsonSerializerSettings _settings;

        /// <summary>
        /// Adds a new Localization to the system.
        /// </summary>
        /// <param name="LocalizationID"></param>
        /// <param name="Localizations"></param>
        /// <exception cref="LocalizationException"></exception>
        public void AddLocalization(LocalizationID LocalizationID, Dictionary<LocalizationLanguage, string> Localizations)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            if (Localizations == null)
            {
                throw new LocalizationException("The " + nameof(Localizzer) + " cannot add null as " + nameof(Localizations));
            }

            if (_LocalizationLanguages.Count != Localizations.Count)
            {
                throw new LocalizationException("The " + nameof(Localizzer) + " knows " + _LocalizationLanguages.Count + " languages, but " + Localizations.Count + " were given.");
            }

            if (_LocalizationData.ContainsKey(LocalizationID))
            {
                throw new LocalizationException("The " + nameof(Localizzer) + " already contains Localizations for the ID " + LocalizationID + ".");
            }

            foreach (LocalizationLanguage langInData in _LocalizationLanguages)
            {
                if (!Localizations.ContainsKey(langInData) || Localizations[langInData] == null || Localizations[langInData].Trim().Equals(""))
                {
                    throw new LocalizationException("The Localization for the language \"" + langInData.Name + "\" cannot be empty!");
                }
            }
            _LocalizationData.Add(LocalizationID, Localizations);
        }

        /// <summary>
        /// Adds a new <see cref="LocalizationScope"/> to the system.
        /// </summary>
        /// <param name="scopeName"></param>
        /// <exception cref="LocalizationException"></exception>
        public void AddScope(string scopeName)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            if (scopeName == null || scopeName.Trim() == "")
            {
                throw new LocalizationException("The given scope cannot be empty!");
            }

            LocalizationScope newScope = new LocalizationScope();
            newScope.Name = scopeName.Trim();

            if (newScope.Equals(_defaultScope) || _LocalizationScopes.Contains(newScope))
            {
                throw new LocalizationException("The given scope already exists!");
            }
            else
            {
                _LocalizationScopes.Add(newScope);
            }
        }

        /// <summary>
        /// Adds a new <see cref="LocalizationLanguage"/> to the system.
        /// </summary>
        /// <param name="languageName"></param>
        /// <param name="languageShortName"></param>
        /// <exception cref="LocalizationException"></exception>
        public void AddLanguage(string languageName, string languageShortName)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            if (languageName == null || languageName.Trim() == "" || languageShortName == null || languageShortName.Trim() == "")
            {
                throw new LocalizationException("The given language name and shortname cannot be empty!");
            }

            LocalizationLanguage newLang = new LocalizationLanguage();
            newLang.Name = languageName.Trim();
            newLang.ShortName = languageShortName.Trim();

            if (_LocalizationLanguages.Contains(newLang))
            {
                throw new LocalizationException("The given language already exists!");
            }
            else
            {
                _LocalizationLanguages.Add(newLang);
            }
        }

        /// <summary>
        /// Removes a <see cref="LocalizationLanguage"/> from the system.
        /// </summary>
        /// <param name="language"></param>
        /// <exception cref="LocalizationException"></exception>
        public void RemoveLanguage(LocalizationLanguage language)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            if (_LocalizationLanguages.Count <= 1)
            {
                throw new LocalizationException("At least one language must persist!");
            }

            foreach (KeyValuePair<LocalizationID, Dictionary<LocalizationLanguage, string>> pair
                in _LocalizationData)
            {
                _LocalizationData[pair.Key].Remove(language);
            }
            OnLanguageEdited(language);
            _LocalizationLanguages.Remove(language);
        }

        /// <summary>
        /// Removes a <see cref="LocalizationScope"/> from the system.
        /// </summary>
        /// <param name="scope"></param>
        /// <exception cref="LocalizationException"></exception>
        public void RemoveScope(LocalizationScope scope)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            if (scope.Equals(_defaultScope))
            {
                throw new LocalizationException("The default scope cannot be removed.");
            }

            foreach (KeyValuePair<LocalizationID, Dictionary<LocalizationLanguage, string>> pair
                in _LocalizationData.Where(locaSet => locaSet.Key.Scope.Equals(scope)).ToList())
            {
                LocalizationID newID = new LocalizationID();
                newID.Scope = _defaultScope;
                newID.Name = pair.Key.Name;

                _LocalizationData.Remove(pair.Key);
                _LocalizationData.Add(newID, pair.Value);
            }

            OnScopeEdited(scope);
            _LocalizationScopes.Remove(scope);
        }

        /// <summary>
        /// Removes a Localization, that is a <see cref="LocalizationID"/> with all its content, from the system.
        /// </summary>
        /// <param name="LocalizationID"></param>
        public void RemoveLocalization(LocalizationID LocalizationID)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            _LocalizationData.Remove(LocalizationID);
            OnLocalizationEdited(LocalizationID);
        }

        /// <summary>
        /// Edits an existing <see cref="LocalizationID"/> by replacing the IDs, but keeping its content.
        /// </summary>
        /// <param name="oldID"></param>
        /// <param name="newID"></param>
        /// <exception cref="LocalizationException"></exception>
        public void EditLocalizationID(LocalizationID oldID, LocalizationID newID)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            if (!_LocalizationData.ContainsKey(newID))
            {
                _LocalizationData.Add(newID, _LocalizationData[oldID]);
                _LocalizationData.Remove(oldID);
            }
            else
            {
                throw new LocalizationException("The edited Localization ID already exists!");
            }

            OnLocalizationEdited(oldID);
        }

        /// <summary>
        /// Replaces a <see cref="LocalizationScope"/> with a new one for all available data.
        /// </summary>
        /// <param name="oldScope"></param>
        /// <param name="newScopeName"></param>
        /// <exception cref="LocalizationException"></exception>
        public void EditScope(LocalizationScope oldScope, string newScopeName)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            if (newScopeName == null || newScopeName.Trim() == "")
            {
                throw new LocalizationException("The edited scope cannot be empty!");
            }

            LocalizationScope newScope = new LocalizationScope();
            newScope.Name = newScopeName.Trim();

            if (newScope.Equals(_defaultScope) || _LocalizationScopes.Contains(newScope) || !_LocalizationScopes.Contains(oldScope))
            {
                throw new LocalizationException("The edited scope already exists!");
            }
            else
            {
                _LocalizationScopes.Add(newScope);
                OnScopeEdited(oldScope);
                _LocalizationScopes.Remove(oldScope);
                foreach (KeyValuePair<LocalizationID, Dictionary<LocalizationLanguage, string>> pair
                    in _LocalizationData.Where(locaSet => locaSet.Key.Scope.Equals(oldScope)).ToList())
                {
                    LocalizationID newID = new LocalizationID();
                    newID.Scope = newScope;
                    newID.Name = pair.Key.Name;

                    _LocalizationData.Remove(pair.Key);
                    _LocalizationData.Add(newID, pair.Value);
                }
            }
        }

        /// <summary>
        /// Replaces a <see cref="LocalizationLanguage"/> with a new one for all available data.
        /// </summary>
        /// <param name="oldLanguage"></param>
        /// <param name="newLanguageName"></param>
        /// <param name="newLanguageShortName"></param>
        /// <exception cref="LocalizationException"></exception>
        public void EditLanguage(LocalizationLanguage oldLanguage, string newLanguageName, string newLanguageShortName)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            if (newLanguageName == null || newLanguageName.Trim() == "" || newLanguageShortName == null || newLanguageShortName.Trim() == "" || !_LocalizationLanguages.Contains(oldLanguage))
            {
                throw new LocalizationException("The edited language name and shortname cannot be empty!");
            }

            LocalizationLanguage newLang = new LocalizationLanguage();
            newLang.Name = newLanguageName.Trim();
            newLang.ShortName = newLanguageShortName.Trim();

            if (_LocalizationLanguages.Contains(newLang))
            {
                throw new LocalizationException("The edited language already exists!");
            }
            else
            {

                _LocalizationLanguages.Remove(oldLanguage);
                _LocalizationLanguages.Add(newLang);
                foreach (KeyValuePair<LocalizationID, Dictionary<LocalizationLanguage, string>> pair
                    in _LocalizationData)
                {
                    _LocalizationData[pair.Key].Add(newLang, _LocalizationData[pair.Key][oldLanguage]);
                    _LocalizationData[pair.Key].Remove(oldLanguage);
                }
                OnLanguageEdited(oldLanguage);
            }
        }

        /// <summary>
        /// Edits a given Localization found by the <see cref="LocalizationID"/> by replacing the current content with <paramref name="Localizations"/>.
        /// </summary>
        /// <param name="LocalizationID"></param>
        /// <param name="Localizations"></param>
        /// <exception cref="LocalizationException"></exception>
        public void EditLocalization(LocalizationID LocalizationID, Dictionary<LocalizationLanguage, string> Localizations)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            if (Localizations == null)
            {
                throw new LocalizationException("The " + nameof(Localizzer) + " cannot edit null as " + nameof(Localizations));
            }

            if (_LocalizationLanguages.Count != Localizations.Count)
            {
                throw new LocalizationException("The " + nameof(Localizzer) + " knows " + _LocalizationLanguages.Count + " languages, but " + Localizations.Count + " were given.");
            }

            if (!_LocalizationData.ContainsKey(LocalizationID))
            {
                throw new LocalizationException("The " + nameof(Localizzer) + " does not contain Localizations for the ID " + LocalizationID + ".");
            }

            foreach (LocalizationLanguage langInData in _LocalizationLanguages)
            {
                if (!Localizations.ContainsKey(langInData) || Localizations[langInData] == null || Localizations[langInData].Trim().Equals(""))
                {
                    throw new LocalizationException("The Localization for the language \"" + langInData.Name + "\" cannot be empty!");
                }
            }

            _LocalizationData[LocalizationID] = Localizations;
            OnLocalizationEdited(LocalizationID);
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

            if (_LocalizationData != null)
            {
                List<KeyValuePair<LocalizationID, List<KeyValuePair<LocalizationLanguage, string>>>> serializableDictionary = new List<KeyValuePair<LocalizationID, List<KeyValuePair<LocalizationLanguage, string>>>>();

                foreach (KeyValuePair<LocalizationID, Dictionary<LocalizationLanguage, string>> pair in _LocalizationData)
                {
                    serializableDictionary.Add(new KeyValuePair<LocalizationID, List<KeyValuePair<LocalizationLanguage, string>>>(pair.Key, new List<KeyValuePair<LocalizationLanguage, string>>()));
                    foreach (KeyValuePair<LocalizationLanguage, string> locaPair in pair.Value)
                    {
                        serializableDictionary[serializableDictionary.Count - 1].Value.Add(locaPair);
                    }
                }
                ResourcesUtil.WriteFile(ProjectPrefKeys.LocalizATIONSAVEPATH, _dataPath, serializableDictionary);
                ResourcesUtil.WriteFile(ProjectPrefKeys.LocalizATIONSAVEPATH, _languagesDataPath, _LocalizationLanguages);
                ResourcesUtil.WriteFile(ProjectPrefKeys.LocalizATIONSAVEPATH, _scopesDataPath, _LocalizationScopes);
            }
        }

        /// <summary>
        /// Initializes the <see cref="Localizzer"/>. This is required due to its dependency on a valid path.
        /// </summary>
        public void Initialize()
        {
            _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            _LocalizationData = new Dictionary<LocalizationID, Dictionary<LocalizationLanguage, string>>();
            _LocalizationScopes = new HashSet<LocalizationScope>();
            _LocalizationLanguages = new HashSet<LocalizationLanguage>();

            _defaultScope = new LocalizationScope();
            _defaultScope.Name = "DefaultScope";
            _LocalizationScopes.Add(_defaultScope);

            if (ProjectPrefs.GetString(ProjectPrefKeys.LocalizATIONSAVEPATH) == null || ProjectPrefs.GetString(ProjectPrefKeys.LocalizATIONSAVEPATH).Equals("")
                || (Application.isEditor && !ResourcesUtil.IsFullPathValid(Application.dataPath + "/" + ProjectPrefs.GetString(ProjectPrefKeys.LocalizATIONSAVEPATH))))
            {
                _isInitialized = false;
                return;
            }

            _isInitialized = true;

            _LocalizationLanguages = ResourcesUtil.GetFileData<HashSet<LocalizationLanguage>>(ProjectPrefKeys.LocalizATIONSAVEPATH, _languagesDataPath);

            if (_LocalizationLanguages == null)
            {
                _LocalizationLanguages = new HashSet<LocalizationLanguage>();
                return;
            }

            List<KeyValuePair<LocalizationID, List<KeyValuePair<LocalizationLanguage, string>>>> serializableDictionary
                = ResourcesUtil.GetFileData<List<KeyValuePair<LocalizationID, List<KeyValuePair<LocalizationLanguage, string>>>>>(ProjectPrefKeys.LocalizATIONSAVEPATH, _dataPath);
            _LocalizationScopes = ResourcesUtil.GetFileData<HashSet<LocalizationScope>>(ProjectPrefKeys.LocalizATIONSAVEPATH, _scopesDataPath);

            foreach (KeyValuePair<LocalizationID, List<KeyValuePair<LocalizationLanguage, string>>> pair in serializableDictionary)
            {
                _LocalizationData.Add(pair.Key, new Dictionary<LocalizationLanguage, string>());
                foreach (KeyValuePair<LocalizationLanguage, string> locaPair in pair.Value)
                {
                    _LocalizationData[pair.Key].Add(locaPair.Key, locaPair.Value);
                }
            }
        }

        //Events

        /// <summary>
        /// Callback for when a <see cref="LocalizationScope"/> is edited or removed.
        /// </summary>
        /// <param name="oldScope"></param>
        private void OnScopeEdited(LocalizationScope oldScope)
        {
            ScopeEdited?.Invoke(oldScope);
        }

        /// <summary>
        /// Callback for when a <see cref="LocalizationLanguage"/> is edited or removed.
        /// </summary>
        /// <param name="oldLanguage"></param>
        private void OnLanguageEdited(LocalizationLanguage oldLanguage)
        {
            LanguageEdited?.Invoke(oldLanguage);
        }

        /// <summary>
        /// Callback for when a <see cref="LocalizationID"/> is edited or removed.
        /// </summary>
        /// <param name="oldLocalizationID"></param>
        private void OnLocalizationEdited(LocalizationID oldLocalizationID)
        {
            LocalizationIDEdited?.Invoke(oldLocalizationID);
        }
    }

}