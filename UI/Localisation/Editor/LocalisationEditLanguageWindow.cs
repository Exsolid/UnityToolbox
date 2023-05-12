using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LocalisationEditLanguageWindow :EditorWindow
{
    private LocalisationLanguage _language;
    public LocalisationLanguage Language
    {
        set
        {
            _language = value;
            _newLanguageName = value.Name;
            _newLanguageShort = value.ShortName;
        }
    }

    private string _newLanguageName;
    private string _newLanguageShort;

    private string _status;

    public static void Open(LocalisationLanguage language)
    {
        LocalisationEditLanguageWindow window = (LocalisationEditLanguageWindow) GetWindow(typeof(LocalisationEditLanguageWindow));
        window.titleContent = new GUIContent("Edit Language");
        window.ShowUtility();
        window.minSize = new Vector2(100, 100);
        window.Language = language;
    }

    private void Awake()
    {
        Localizer.Instance.LanguageEdited += LanguageEdited;
    }

    public void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Language name to edit: ");
        _newLanguageName = GUILayout.TextField(_newLanguageName, GUILayout.Width(200));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Language short to edit: ");
        _newLanguageShort = GUILayout.TextField(_newLanguageShort, GUILayout.Width(200));
        GUILayout.EndHorizontal();

        GUILayout.Label(_status);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Cancel"))
        {
            Localizer.Instance.LanguageEdited -= LanguageEdited;
            Close();
        }

        if (GUILayout.Button("Save"))
        {
            if (Localizer.Instance.EditLanguage(_language, _newLanguageName, _newLanguageShort))
            {
                Localizer.Instance.LanguageEdited -= LanguageEdited;
                Localizer.Instance.WriteData();
                AssetDatabase.Refresh();
                Close();
            }
            _status = "Could not edit the language, does the name already exist?";
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void LanguageEdited(LocalisationLanguage language)
    {
        if (_language.Equals(language))
        {
            Localizer.Instance.LanguageEdited -= LanguageEdited;
            Close();
        }
    }
}
