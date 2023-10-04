using System.Collections;
using System.Collections.Generic;
using UnityToolbox.UI.Localization;
using UnityEngine;
using UnityEditor;

namespace UnityToolbox.UI.Localization.Editor
{
    public class LocalizationEditLanguageWindow : EditorWindow
    {
        private LocalizationLanguage _language;
        public LocalizationLanguage Language
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

        public static void Open(LocalizationLanguage language)
        {
            LocalizationEditLanguageWindow window = (LocalizationEditLanguageWindow)GetWindow(typeof(LocalizationEditLanguageWindow));
            window.titleContent = new GUIContent("Edit Language");
            window.ShowUtility();
            window.minSize = new Vector2(450, 120);
            window.maxSize = new Vector2(450, 120);
            window.Language = language;
        }

        private void Awake()
        {
            Localizzer.Instance.LanguageEdited += LanguageEdited;
            UpdateStatus("");
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical();

            DrawLine();
            GUILayout.Label(_status);
            DrawLine();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Language name to edit: ");
            _newLanguageName = GUILayout.TextField(_newLanguageName, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Language short to edit: ");
            _newLanguageShort = GUILayout.TextField(_newLanguageShort, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            DrawLine();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Cancel"))
            {
                Localizzer.Instance.LanguageEdited -= LanguageEdited;
                Close();
            }

            if (GUILayout.Button("Save"))
            {
                try
                {
                    Localizzer.Instance.EditLanguage(_language, _newLanguageName, _newLanguageShort);
                    Localizzer.Instance.LanguageEdited -= LanguageEdited;
                    Localizzer.Instance.WriteData();
                    AssetDatabase.Refresh();
                    Close();
                }
                catch (LocalizationException ex)
                {
                    UpdateStatus(ex.Message);
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void UpdateStatus(string status)
        {
            _status = "Status:     " + status;
        }

        private void DrawLine()
        {
            EditorGUILayout.Space();
            Rect rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private void LanguageEdited(LocalizationLanguage language)
        {
            if (_language.Equals(language))
            {
                Localizzer.Instance.LanguageEdited -= LanguageEdited;
                Close();
            }
        }
    } 
}
