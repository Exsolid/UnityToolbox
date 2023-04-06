using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

public class LocalisationSelectionWindow : EditorWindow
{
    private Vector2 _scrollPos;
    private int _selectedSearch;
    private int _selectedLanguage;
    private string _searchIDString;
    private bool _foldoutSearch;

    public event Action<LocalisationID> OnIDSelected;

    public static LocalisationSelectionWindow Open()
    {
        LocalisationSelectionWindow window = (LocalisationSelectionWindow) GetWindowWithRect(typeof(LocalisationSelectionWindow), new Rect(0, 0, 600, 400));
        window.titleContent = new GUIContent("Dropdown Selection");

        Vector2 mouse = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        Rect rect = new Rect(mouse.x - 550, mouse.y + 10, 10, 10);

        window.ShowAsDropDown(rect, new Vector2(600, 200));

        return window;
    }

    private void InitializeWindow()
    {
        string assetPathInProject = ProjectPrefs.GetString(ProjectPrefKeys.LOCALISATIONSAVEPATH);
        Localizer.Instance.AssetPathInProject = assetPathInProject;
        Localizer.Instance.Initialize();
        _searchIDString = "";
    }

    private void Awake()
    {
        InitializeWindow();
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        _foldoutSearch = EditorGUILayout.Foldout(_foldoutSearch, "Search");
        GUILayout.EndHorizontal();

        _scrollPos = GUILayout.BeginScrollView(_scrollPos);

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

        foreach (KeyValuePair<LocalisationID, Dictionary<LocalisationLanguage, string>> pair
               in filtered)
        {
            GUILayout.BeginHorizontal("Box");

            GUILayout.Label(pair.Key.Name + LocalisationID.DEVIDER + pair.Key.Scope.Name, GUILayout.Width(EditorGUIUtility.currentViewWidth / 3)); 
            GUILayout.Label(pair.Value.ContainsKey(Localizer.Instance.LocalisationLanguages.ElementAt(_selectedLanguage)) ? pair.Value[Localizer.Instance.LocalisationLanguages.ElementAt(_selectedLanguage)] : "", GUILayout.Width(EditorGUIUtility.currentViewWidth / 3));

            if (pair.Value.Count != Localizer.Instance.LocalisationLanguages.Count)
            {
                GUILayout.Label("[REQUIRES EDIT]", GUILayout.Width(EditorGUIUtility.currentViewWidth / 3 - 95));
            }
            else
            {
                GUILayout.Label("", GUILayout.Width(EditorGUIUtility.currentViewWidth / 3 - 95));
            }

            if (GUILayout.Button("^", GUILayout.Width(20)))
            {
                OnIDSelected?.Invoke(pair.Key);
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
}
