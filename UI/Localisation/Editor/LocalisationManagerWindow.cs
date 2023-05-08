using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LocalisationManagerWindow : EditorWindow
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
    private Dictionary<LocalisationLanguage, string> _newLocalisations;
    private int _selectedLanguage;
    private string _searchIDString;
    private int _selectedSearch;
    private bool _foldoutAdd;
    private bool _foldoutSearch;

    [MenuItem("UnityToolbox/Localisation System")]
    private static void DisplayWindow()
    {
        LocalisationManagerWindow window =(LocalisationManagerWindow)GetWindowWithRect(typeof(LocalisationManagerWindow), new Rect(0, 0, 600, 400));
        window.titleContent = new GUIContent("Localisation Manager");
        window.ShowUtility();
    }

    public static void Open()
    {
        LocalisationManagerWindow window = (LocalisationManagerWindow)GetWindow(typeof(LocalisationManagerWindow));
        window.titleContent = new GUIContent("Localisation Manager");

        Vector2 mouse = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        Rect r = new Rect(mouse.x - 450, mouse.y +10, 10, 10);

        window.ShowAsDropDown(r, new Vector2(600, 400));
    }

    private void InitializeWindow()
    {
        _assetPathInProject = ProjectPrefs.GetString(ProjectPrefKeys.LOCALISATIONSAVEPATH);
        Localizer.Instance.AssetPathInProject = _assetPathInProject;
        Localizer.Instance.Initialize();
        _status = "Status: -";
        _searchIDString = "";
        _scrollPosScope = Vector2.zero;
        _scrollPosLanguage = Vector2.zero;
        _scrollPosID = Vector2.zero;
        _newLocalisations = new Dictionary<LocalisationLanguage, string>();
    }

    private void Awake()
    {
        InitializeWindow();
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
        GUILayout.Label("To update the " + nameof(Localizer) + " path. Please enter a valid path below. \nIt is required that it containes \"Resources/\".");
        DrawLineHorizontal();
        GUILayout.BeginHorizontal();
        Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(Application.dataPath));
        GUILayout.Label(Application.dataPath);
        _assetPathInProject = GUILayout.TextField(_assetPathInProject, GUILayout.Width(585 - textDimensions.x));
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Refresh"))
        {
            if (!Directory.Exists(Application.dataPath + _assetPathInProject))
            {
                UpdateStatus("The path \"" + _assetPathInProject + "\" could not be found!");
            }
            else if (!_assetPathInProject.Contains("Resources/"))
            {
                UpdateStatus("The path \"" + _assetPathInProject + "\" is not a \"Resources/\" directory.");
            }
            else
            {
                Localizer.Instance.AssetPathInProject = _assetPathInProject;
                Localizer.Instance.Initialize();
                if (Localizer.Instance.IsInitialized)
                {
                    ProjectPrefs.SetString(ProjectPrefKeys.LOCALISATIONSAVEPATH, _assetPathInProject);
                    UpdateStatus("Path updated.");
                }
                else
                {
                    UpdateStatus("The given path was not valid, please check the console for more information.");
                }
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
            if (Localizer.Instance.AddScope(_scopeName))
            {
                Localizer.Instance.WriteData();
                UpdateStatus("Successfully added a new scope.");
            }
            else
            {
                UpdateStatus("Could not add the new scope, does it already exist?");
            }
        }
        GUILayout.EndHorizontal();
        DrawLineHorizontal();

        GUILayout.BeginVertical();
        _scrollPosScope = GUILayout.BeginScrollView(_scrollPosScope);
        foreach(LocalisationScope scope in Localizer.Instance.LocalisationScopes)
        {
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label(scope.Name);
            if (!scope.Equals(Localizer.Instance.DefaultScope))
            {
                if (GUILayout.Button("*", GUILayout.Width(20)))
                {
                    LocalisationEditScopeWindow.Open(scope);
                }

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    if(EditorUtility.DisplayDialog("Remove Scope", "Are you sure you want to delete the scope '" + scope.Name+"'? \nIDs using this scope will have it replaced with the default scope.", "Yes"))
                    {
                        Localizer.Instance.RemoveScope(scope);
                        Localizer.Instance.WriteData();
                        UpdateStatus("Successfully removed the scope '" + scope.Name +"'.");
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
            if (Localizer.Instance.AddLanguage(_languageName, _languageShortName))
            {
                Localizer.Instance.WriteData();
                UpdateStatus("Successfully added a new language.");
            }
            else
            {
                UpdateStatus("Could not add the new language, does it already exist?");
            }
        }
        GUILayout.EndHorizontal();
        DrawLineHorizontal();

        GUILayout.BeginVertical();
        _scrollPosLanguage = GUILayout.BeginScrollView(_scrollPosLanguage);
        foreach (LocalisationLanguage language in Localizer.Instance.LocalisationLanguages)
        {
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label(language.Name);
            GUILayout.Label(language.ShortName);

            if (GUILayout.Button("*", GUILayout.Width(20)))
            {
                LocalisationEditLanguageWindow.Open(language);
            }

            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                if (EditorUtility.DisplayDialog("Remove Language", "Are you sure you want to delete the language '" + language.Name + "'? \nThis will also delete every translation associated with it!", "Yes"))
                {
                    Localizer.Instance.RemoveLanguage(language);
                    Localizer.Instance.WriteData();
                    UpdateStatus("Successfully removed the language '" + language.Name + "'.");
                }
            }

            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void DisplayIDsTab()
    {
        GUILayout.BeginHorizontal();
        _foldoutAdd = EditorGUILayout.Foldout(_foldoutAdd, "Add localisation ID");
        GUILayout.EndHorizontal();

        if (_foldoutAdd)
        {
            if(Localizer.Instance.LocalisationLanguages.Count != 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("ID: ");
                _IDName = GUILayout.TextField(_IDName, GUILayout.Width(200));
                GUILayout.Label(LocalisationID.DEVIDER.ToString(), GUILayout.Width(10));
                string[] scopes = Localizer.Instance.LocalisationScopes.Select(x => x.Name).ToArray();
                _selectedScope = EditorGUILayout.Popup("", _selectedScope, scopes);
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    LocalisationID newID = new LocalisationID();
                    newID.Name = _IDName;
                    newID.Scope = Localizer.Instance.LocalisationScopes.ElementAt(_selectedScope);

                    if (Localizer.Instance.AddLocalisation(newID, _newLocalisations.ToDictionary(entry => entry.Key, entry => entry.Value)))
                    {
                        Localizer.Instance.WriteData();
                        UpdateStatus("Successfully added a new localisation.");
                    }
                    else
                    {
                        UpdateStatus("Could not add the new localisation, does it already exist?");
                    }
                }
                GUILayout.EndHorizontal();


                foreach (LocalisationLanguage language in Localizer.Instance.LocalisationLanguages)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Translate " + language.Name + ":");
                    if (_newLocalisations.ContainsKey(language))
                    {
                        _newLocalisations[language] = GUILayout.TextField(_newLocalisations[language], GUILayout.Width(447));
                    }
                    else
                    {
                        _newLocalisations.Add(language, GUILayout.TextField("", GUILayout.Width(200)));
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

        GUILayout.BeginHorizontal();
        _foldoutSearch = EditorGUILayout.Foldout(_foldoutSearch, "Search");
        GUILayout.EndHorizontal();

        string[] languages = Localizer.Instance.LocalisationLanguages.Select(x => x.Name).ToArray();
        string[] searchScopes = new string[] { "ID" };
        searchScopes = languages.Concat(searchScopes).ToArray();

        if (_foldoutSearch)
        {
            EditorGUILayout.BeginHorizontal();
            _selectedLanguage = EditorGUILayout.Popup("Show Language:", _selectedLanguage, languages);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Contains:");
            _searchIDString = EditorGUILayout.TextField(_searchIDString);
            GUILayout.Label("in");
            _selectedSearch = EditorGUILayout.Popup("", _selectedSearch, searchScopes);
            EditorGUILayout.EndHorizontal();
        }

        DrawLineHorizontal();
        Dictionary<LocalisationID, Dictionary<LocalisationLanguage, string>> filtered = Localizer.Instance.LocalisationData.ToDictionary(entry => entry.Key, entry => entry.Value.ToDictionary(entry => entry.Key, entry => entry.Value));

        if (_selectedSearch == searchScopes.Count() - 1)
        {
            filtered = filtered.Where(entry => (entry.Key.Name + "_" + entry.Key.Scope.Name).Contains(_searchIDString)).ToDictionary(entry => entry.Key, entry => entry.Value);
        }
        else
        {
            filtered = filtered.Where(entry =>
            !entry.Value.ContainsKey(Localizer.Instance.LocalisationLanguages.ElementAt(_selectedSearch))
            || entry.Value[Localizer.Instance.LocalisationLanguages.ElementAt(_selectedSearch)]
            .Contains(_searchIDString)).ToDictionary(entry => entry.Key, entry => entry.Value);
        }

        GUILayout.BeginVertical();
        _scrollPosID = GUILayout.BeginScrollView(_scrollPosID);

        foreach (KeyValuePair<LocalisationID, Dictionary<LocalisationLanguage, string>> pair
               in filtered)
        {
            GUILayout.BeginHorizontal("Box");

            GUILayout.Label(pair.Key.Name+LocalisationID.DEVIDER+ pair.Key.Scope.Name, GUILayout.Width(EditorGUIUtility.currentViewWidth / 3));
            GUILayout.Label(pair.Value.ContainsKey(Localizer.Instance.LocalisationLanguages.ElementAt(_selectedLanguage)) ? pair.Value[Localizer.Instance.LocalisationLanguages.ElementAt(_selectedLanguage)] : "", GUILayout.Width(EditorGUIUtility.currentViewWidth / 3));
            if (pair.Value.Count != Localizer.Instance.LocalisationLanguages.Count)
            {
                GUILayout.Label("[REQUIRES EDIT]", GUILayout.Width(EditorGUIUtility.currentViewWidth / 3 - 70));
            }
            else
            {
                GUILayout.Label("", GUILayout.Width(EditorGUIUtility.currentViewWidth / 3 - 70));
            }

            if (GUILayout.Button("*", GUILayout.Width(20)))
            {
                LocalisationEditIDWindow.Open(pair.Key);
            }

            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                if (EditorUtility.DisplayDialog("Remove Localisation", "Are you sure you want to delete the localisation '" + pair.Key.Name + LocalisationID.DEVIDER + pair.Key.Scope.Name + "'?", "Yes"))
                {
                    Localizer.Instance.RemoveLocalisation(pair.Key);
                    Localizer.Instance.WriteData();
                    UpdateStatus("Successfully removed the scope '" + pair.Key.Name + LocalisationID.DEVIDER + pair.Key.Scope.Name + "'.");
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
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
