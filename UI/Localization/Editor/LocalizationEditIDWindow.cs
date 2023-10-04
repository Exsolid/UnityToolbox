using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityToolbox.UI.Localization;

namespace UnityToolbox.UI.Localization.Editor
{
    public class LocalizationEditIDWindow : EditorWindow
    {
        private LocalizationID _LocalizationID;
        public LocalizationID LocalizationID
        {
            set
            {
                _LocalizationID = value;
                _newLocalizations = Localizzer.Instance.LocalizationData[value].ToDictionary(entry => entry.Key, entry => entry.Value);
                _newIDName = value.Name;
                int i = 0;
                foreach (LocalizationScope scope in Localizzer.Instance.LocalizationScopes)
                {
                    if (scope.Equals(value.Scope))
                    {
                        _newSelectedScope = i;
                        break;
                    }
                    i++;
                }
            }
        }

        private Dictionary<LocalizationLanguage, string> _newLocalizations;
        private string _newIDName;
        private int _newSelectedScope;

        private string _status;

        public static void Open(LocalizationID LocalizationID)
        {
            LocalizationEditIDWindow window = (LocalizationEditIDWindow)GetWindow(typeof(LocalizationEditIDWindow));
            window.titleContent = new GUIContent("Edit Language");
            window.ShowUtility();
            window.minSize = new Vector2(600, 120);
            window.LocalizationID = LocalizationID;
        }

        private void Awake()
        {
            Localizzer.Instance.LanguageEdited += LanguageEdited;
            Localizzer.Instance.ScopeEdited += ScopeEdited;
            Localizzer.Instance.LocalizationIDEdited += LocalizationIDEdited;
            UpdateStatus("");
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical();
            DrawLine();
            GUILayout.Label(_status);
            DrawLine();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Add Localization with ID: ");
            _newIDName = GUILayout.TextField(_newIDName, GUILayout.Width(200));
            GUILayout.Label(LocalizationID.DEVIDER.ToString(), GUILayout.Width(10));
            string[] scopes = Localizzer.Instance.LocalizationScopes.Select(x => x.Name).ToArray();
            _newSelectedScope = EditorGUILayout.Popup("", _newSelectedScope, scopes);
            GUILayout.EndHorizontal();

            foreach (LocalizationLanguage language in Localizzer.Instance.LocalizationLanguages)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Translate " + language.Name + ":");
                if (_newLocalizations.ContainsKey(language))
                {
                    _newLocalizations[language] = GUILayout.TextField(_newLocalizations[language], GUILayout.Width(447));
                }
                else
                {
                    _newLocalizations.Add(language, GUILayout.TextField("", GUILayout.Width(200)));
                }
                GUILayout.EndHorizontal();
            }

            DrawLine();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Cancel"))
            {
                Close();
            }

            if (GUILayout.Button("Save"))
            {
                try
                {
                    LocalizationID newID = new LocalizationID();
                    newID.Name = _newIDName;
                    newID.Scope = Localizzer.Instance.LocalizationScopes.ElementAt(_newSelectedScope);
                    if (!newID.Equals(_LocalizationID))
                    {
                        Localizzer.Instance.EditLocalizationID(_LocalizationID, newID);
                    }

                    Localizzer.Instance.EditLocalization(newID, _newLocalizations);

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
            Close();
        }

        private void ScopeEdited(LocalizationScope scope)
        {
            Close();
        }

        private void LocalizationIDEdited(LocalizationID ID)
        {
            Close();
        }

        new public void Close()
        {
            Localizzer.Instance.LanguageEdited -= LanguageEdited;
            Localizzer.Instance.LocalizationIDEdited -= LocalizationIDEdited;
            Localizzer.Instance.ScopeEdited -= ScopeEdited;
            base.Close();
        }
    } 
}
