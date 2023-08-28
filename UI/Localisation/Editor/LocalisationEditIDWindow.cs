using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace UnityToolbox.UI.Localisation.Editor
{
    public class LocalisationEditIDWindow : EditorWindow
    {
        private LocalisationID _localisationID;
        public LocalisationID LocalisationID
        {
            set
            {
                _localisationID = value;
                _newLocalisations = Localizer.Instance.LocalisationData[value].ToDictionary(entry => entry.Key, entry => entry.Value);
                _newIDName = value.Name;
                int i = 0;
                foreach (LocalisationScope scope in Localizer.Instance.LocalisationScopes)
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

        private Dictionary<LocalisationLanguage, string> _newLocalisations;
        private string _newIDName;
        private int _newSelectedScope;

        private string _status;

        public static void Open(LocalisationID localisationID)
        {
            LocalisationEditIDWindow window = (LocalisationEditIDWindow)GetWindow(typeof(LocalisationEditIDWindow));
            window.titleContent = new GUIContent("Edit Language");
            window.ShowUtility();
            window.minSize = new Vector2(600, 120);
            window.LocalisationID = localisationID;
        }

        private void Awake()
        {
            Localizer.Instance.LanguageEdited += LanguageEdited;
            Localizer.Instance.ScopeEdited += ScopeEdited;
            Localizer.Instance.LocalisationIDEdited += LocalisationIDEdited;
            UpdateStatus("");
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical();
            DrawLine();
            GUILayout.Label(_status);
            DrawLine();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Add localisation with ID: ");
            _newIDName = GUILayout.TextField(_newIDName, GUILayout.Width(200));
            GUILayout.Label(LocalisationID.DEVIDER.ToString(), GUILayout.Width(10));
            string[] scopes = Localizer.Instance.LocalisationScopes.Select(x => x.Name).ToArray();
            _newSelectedScope = EditorGUILayout.Popup("", _newSelectedScope, scopes);
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
                    LocalisationID newID = new LocalisationID();
                    newID.Name = _newIDName;
                    newID.Scope = Localizer.Instance.LocalisationScopes.ElementAt(_newSelectedScope);
                    if (!newID.Equals(_localisationID))
                    {
                        Localizer.Instance.EditLocalisationID(_localisationID, newID);
                    }

                    Localizer.Instance.EditLocalisation(newID, _newLocalisations);

                    Localizer.Instance.WriteData();
                    AssetDatabase.Refresh();
                    Close();
                }
                catch (LocalisationException ex)
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

        private void LanguageEdited(LocalisationLanguage language)
        {
            Close();
        }

        private void ScopeEdited(LocalisationScope scope)
        {
            Close();
        }

        private void LocalisationIDEdited(LocalisationID ID)
        {
            Close();
        }

        new public void Close()
        {
            Localizer.Instance.LanguageEdited -= LanguageEdited;
            Localizer.Instance.LocalisationIDEdited -= LocalisationIDEdited;
            Localizer.Instance.ScopeEdited -= ScopeEdited;
            base.Close();
        }
    } 
}
