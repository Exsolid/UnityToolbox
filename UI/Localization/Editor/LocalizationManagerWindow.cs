using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityToolbox.General.Management;
using UnityToolbox.General.Preferences;
using UnityToolbox.UI.Localization;

namespace UnityToolbox.UI.Localization.Editor
{
    public class LocalizationManagerWindow : EditorWindow
    {
        private int _selectedTab = 0;
        private const int IDS = 0;
        private const int LANGUAGES = 1;
        private const int SCOPES = 2;
        private const int SETTINGS = 3;

        private string _assetPathInProject;

        private string _status;

        private Vector2 _scrollPosScope;
        private string _scopeName;

        private Vector2 _scrollPosLanguage;
        private string _languageName;
        private string _languageShortName;

        private Vector2 _scrollPosID;
        private string _IDName;
        private int _selectedScope;
        private Dictionary<LocalizationLanguage, string> _newLocalizations;
        private int _selectedLanguage;
        private string _searchIDString;
        private int _selectedSearch;
        private bool _foldoutAdd;
        private bool _foldoutSearch;

        [MenuItem("UnityToolbox/Localization System")]
        private static void DisplayWindow()
        {
            LocalizationManagerWindow window = (LocalizationManagerWindow)GetWindow(typeof(LocalizationManagerWindow));
            window.maxSize = new Vector2(600, 400);
            window.minSize = new Vector2(600, 400);

            window.titleContent = new GUIContent("Localization Manager");
            window.ShowUtility();
        }

        public static void Open()
        {
            LocalizationManagerWindow window = (LocalizationManagerWindow)GetWindow(typeof(LocalizationManagerWindow));
            window.titleContent = new GUIContent("Localization Manager");

            Vector2 mouse = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            Rect r = new Rect(mouse.x - 450, mouse.y + 10, 10, 10);

            window.ShowAsDropDown(r, new Vector2(600, 400));
        }

        private void InitializeWindow()
        {
            Localizer.Instance.Initialize();
            _assetPathInProject = ResourcesUtil.GetProjectPath(ProjectPrefKeys.LOCALIZATIONSAVEPATH);
            _status = "Status: -";
            _searchIDString = "";
            _scrollPosScope = Vector2.zero;
            _scrollPosLanguage = Vector2.zero;
            _scrollPosID = Vector2.zero;
            _newLocalizations = new Dictionary<LocalizationLanguage, string>();
        }

        private void Awake()
        {
            InitializeWindow();
            Localizer.Instance.Initialize();
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();

            DrawLineHorizontal();
            GUILayout.Label(_status);
            DrawLineHorizontal();

            if (Localizer.Instance.IsInitialized)
            {
                _selectedTab = GUILayout.Toolbar(_selectedTab, new string[] { "IDs", "Languages", "Scopes", "Settings" });
                DrawLineHorizontal();
                switch (_selectedTab)
                {
                    case IDS:
                        DisplayIDsTab();
                        break;
                    case LANGUAGES:
                        DisplayLanguagesTab();
                        break;
                    case SCOPES:
                        DisplayScopesTab();
                        break;
                    case SETTINGS:
                        DisplaySettingsTab();
                        break;
                }
            }
            else
            {
                DisplaySettingsTab();
            }

            GUILayout.EndVertical();
        }

        private void DisplaySettingsTab()
        {
            GUILayout.Label("To update the " + nameof(Localizer) + " path. Please enter a valid path below. \nIt is required that it containes \"Resources\".");
            DrawLineHorizontal();
            GUILayout.BeginHorizontal();
            Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(Application.dataPath));
            GUILayout.Label(Application.dataPath + "/");
            _assetPathInProject = GUILayout.TextField(_assetPathInProject, GUILayout.Width(585 - textDimensions.x));
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Refresh"))
            {
                if (ResourcesUtil.TrySetValidPath(Application.dataPath + "/" + _assetPathInProject, ProjectPrefKeys.LOCALIZATIONSAVEPATH))
                {
                    AssetDatabase.Refresh();
                    Localizer.Instance.Initialize();
                    if (Localizer.Instance.IsInitialized)
                    {
                        UpdateStatus("Path updated.");
                    }
                    else
                    {
                        UpdateStatus("The given path was not valid.");
                    }
                }
                else
                {
                    UpdateStatus("The given path was not valid.");
                }
            }
        }

        private void DisplayScopesTab()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Add scope with name: ");
            _scopeName = GUILayout.TextField(_scopeName, GUILayout.Width(200));
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                try
                {
                    Localizer.Instance.AddScope(_scopeName);
                    Localizer.Instance.WriteData();
                    AssetDatabase.Refresh();
                    UpdateStatus("Successfully added a new scope.");
                }
                catch (LocalizationException ex)
                {
                    UpdateStatus(ex.Message);
                }
            }
            GUILayout.EndHorizontal();
            DrawLineHorizontal();

            GUILayout.BeginVertical();
            _scrollPosScope = GUILayout.BeginScrollView(_scrollPosScope);
            foreach (LocalizationScope scope in Localizer.Instance.LocalizationScopes)
            {
                GUILayout.BeginHorizontal("Box");
                GUILayout.Label(scope.Name);
                if (!scope.Equals(Localizer.Instance.DefaultScope))
                {
                    if (GUILayout.Button("*", GUILayout.Width(20)))
                    {
                        LocalizationEditScopeWindow.Open(scope);
                    }

                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        if (EditorUtility.DisplayDialog("Remove Scope", "Are you sure you want to delete the scope \"" + scope.Name + "\"? \nIDs using this scope will have it replaced with the default scope.", "Yes"))
                        {
                            Localizer.Instance.RemoveScope(scope);
                            Localizer.Instance.WriteData();
                            AssetDatabase.Refresh();
                            UpdateStatus("Successfully removed the scope \"" + scope.Name + "\".");
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DisplayLanguagesTab()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Add language with name: ");
            _languageName = GUILayout.TextField(_languageName, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("And short: ");
            _languageShortName = GUILayout.TextField(_languageShortName, GUILayout.Width(177));

            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                try
                {
                    Localizer.Instance.AddLanguage(_languageName, _languageShortName);
                    Localizer.Instance.WriteData();
                    AssetDatabase.Refresh();
                    UpdateStatus("Successfully added a new language.");
                }
                catch (LocalizationException ex)
                {
                    UpdateStatus(ex.Message);
                }
            }
            GUILayout.EndHorizontal();
            DrawLineHorizontal();

            GUILayout.BeginVertical();
            _scrollPosLanguage = GUILayout.BeginScrollView(_scrollPosLanguage);
            foreach (LocalizationLanguage language in Localizer.Instance.LocalizationLanguages)
            {
                GUILayout.BeginHorizontal("Box");
                GUILayout.Label(language.Name);
                GUILayout.Label(language.ShortName);

                if (GUILayout.Button("*", GUILayout.Width(20)))
                {
                    LocalizationEditLanguageWindow.Open(language);
                }

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("Remove Language", "Are you sure you want to delete the language \"'" + language.Name + "\"? \nThis will also delete every translation associated with it!", "Yes"))
                    {
                        try
                        {
                            Localizer.Instance.RemoveLanguage(language);
                            Localizer.Instance.WriteData();
                            AssetDatabase.Refresh();
                            UpdateStatus("Successfully removed the language \"" + language.Name + "\".");
                        }
                        catch (LocalizationException ex)
                        {
                            UpdateStatus(ex.Message);
                        }
                    }
                }

                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DisplayIDsTab()
        {
            DrawLocaIDCreation();
            DrawLocaIDList();
        }

        private void DrawLocaIDList()
        {
            GUILayout.BeginHorizontal();
            _foldoutSearch = EditorGUILayout.Foldout(_foldoutSearch, "Search");
            GUILayout.EndHorizontal();

            string[] languages = Localizer.Instance.LocalizationLanguages.Select(x => x.Name).ToArray();
            string[] searchScopes = new string[] { "ID" };
            searchScopes = languages.Concat(searchScopes).ToArray();

            if (_foldoutSearch)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Show Language:");
                _selectedLanguage = EditorGUILayout.Popup(_selectedLanguage, languages, GUILayout.Width(400));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Contains:");
                _searchIDString = EditorGUILayout.TextField(_searchIDString, GUILayout.Width(200));
                GUILayout.Label("in", GUILayout.Width(14));
                _selectedSearch = EditorGUILayout.Popup("", _selectedSearch, searchScopes, GUILayout.Width(178));
                EditorGUILayout.EndHorizontal();
            }

            DrawLineHorizontal();
            Dictionary<LocalizationID, Dictionary<LocalizationLanguage, string>> filtered = Localizer.Instance.LocalizationData.ToDictionary(entry => entry.Key, entry => entry.Value.ToDictionary(entry => entry.Key, entry => entry.Value));

            if (_selectedSearch == searchScopes.Count() - 1)
            {
                filtered = filtered.Where(entry => (entry.Key.Name + "_" + entry.Key.Scope.Name).Contains(_searchIDString)).ToDictionary(entry => entry.Key, entry => entry.Value);
            }
            else
            {
                filtered = filtered.Where(entry =>
                !entry.Value.ContainsKey(Localizer.Instance.LocalizationLanguages.ElementAt(_selectedSearch))
                || entry.Value[Localizer.Instance.LocalizationLanguages.ElementAt(_selectedSearch)]
                .Contains(_searchIDString)).ToDictionary(entry => entry.Key, entry => entry.Value);
            }

            GUILayout.BeginVertical();
            _scrollPosID = GUILayout.BeginScrollView(_scrollPosID);

            foreach (KeyValuePair<LocalizationID, Dictionary<LocalizationLanguage, string>> pair
                   in filtered)
            {
                GUILayout.BeginHorizontal("Box");

                GUILayout.Label(pair.Key.GetQualifiedName(), GUILayout.Width(EditorGUIUtility.currentViewWidth / 3));
                GUILayout.Label(pair.Value.ContainsKey(Localizer.Instance.LocalizationLanguages.ElementAt(_selectedLanguage)) ? pair.Value[Localizer.Instance.LocalizationLanguages.ElementAt(_selectedLanguage)] : "", GUILayout.Width(EditorGUIUtility.currentViewWidth / 3));
                if (pair.Value.Count != Localizer.Instance.LocalizationLanguages.Count)
                {
                    GUILayout.Label("[REQUIRES EDIT]", GUILayout.Width(EditorGUIUtility.currentViewWidth / 3 - 70));
                }
                else
                {
                    GUILayout.Label("", GUILayout.Width(EditorGUIUtility.currentViewWidth / 3 - 70));
                }

                if (GUILayout.Button("*", GUILayout.Width(20)))
                {
                    LocalizationEditIDWindow.Open(pair.Key);
                }

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("Remove Localization", "Are you sure you want to delete the Localization \"" + pair.Key.GetQualifiedName() + "\"?", "Yes"))
                    {
                        Localizer.Instance.RemoveLocalization(pair.Key);
                        Localizer.Instance.WriteData();
                        AssetDatabase.Refresh();
                        UpdateStatus("Successfully removed the scope \"" + pair.Key.GetQualifiedName() + "\".");
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawLocaIDCreation()
        {
            GUILayout.BeginHorizontal();
            _foldoutAdd = EditorGUILayout.Foldout(_foldoutAdd, "Add Localization ID");
            GUILayout.EndHorizontal();

            if (_foldoutAdd)
            {
                if (Localizer.Instance.LocalizationLanguages.Count != 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("ID: ");
                    _IDName = GUILayout.TextField(_IDName, GUILayout.Width(189));
                    GUILayout.Label(LocalizationID.DEVIDER.ToString(), GUILayout.Width(10));
                    string[] scopes = Localizer.Instance.LocalizationScopes.Select(x => x.Name).ToArray();
                    _selectedScope = EditorGUILayout.Popup("", _selectedScope, scopes, GUILayout.Width(170));

                    if (GUILayout.Button("+", GUILayout.Width(20)))
                    {
                        LocalizationID newID = new LocalizationID();
                        newID.Name = _IDName;
                        newID.Scope = Localizer.Instance.LocalizationScopes.ElementAt(_selectedScope);

                        try
                        {
                            Localizer.Instance.AddLocalization(newID, _newLocalizations.ToDictionary(entry => entry.Key, entry => entry.Value));
                            Localizer.Instance.WriteData();
                            AssetDatabase.Refresh();
                            UpdateStatus("Successfully added a new Localization.");
                        }
                        catch (LocalizationException ex)
                        {
                            UpdateStatus(ex.Message);
                        }
                    }
                    GUILayout.EndHorizontal();


                    foreach (LocalizationLanguage language in Localizer.Instance.LocalizationLanguages)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Translate " + language.Name + ":");
                        if (_newLocalizations.ContainsKey(language))
                        {
                            _newLocalizations[language] = GUILayout.TextField(_newLocalizations[language], GUILayout.Width(400));
                        }
                        else
                        {
                            _newLocalizations.Add(language, GUILayout.TextField("", GUILayout.Width(400)));
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    GUILayout.Label("Add a language first, to be able to add new IDs.");
                }
            }
            DrawLineHorizontal();
        }

        private void DrawLineHorizontal()
        {
            EditorGUILayout.Space();
            Rect rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private void UpdateStatus(string status)
        {
            _status = "Status:     " + status;
        }

        void OnEnable()
        {
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        void OnDisable()
        {
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        }

        public void OnAfterAssemblyReload()
        {
            InitializeWindow();
        }

        private bool IsPathValid(string path)
        {
            return Directory.Exists(path) && path.Contains("Resources/");
        }
    }

}