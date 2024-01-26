using System.Collections;
using System.Collections.Generic;
using UnityToolbox.UI.Localization;
using UnityEngine;
using UnityEditor;

namespace UnityToolbox.UI.Localization.Editor
{
    public class LocalizationEditScopeWindow : EditorWindow
    {
        private LocalizationScope _scope;
        public LocalizationScope Scope
        {
            set
            {
                _scope = value;
                _newScopeName = value.Name;
            }
        }

        private string _newScopeName;

        private string _status;

        public static void Open(LocalizationScope scope)
        {
            LocalizationEditScopeWindow window = (LocalizationEditScopeWindow)GetWindow(typeof(LocalizationEditScopeWindow));
            window.titleContent = new GUIContent("Edit Scope");
            window.ShowUtility();
            window.minSize = new Vector2(450, 100);
            window.maxSize = new Vector2(450, 100);
            window.Scope = scope;
        }

        private void Awake()
        {
            Localizer.Instance.ScopeEdited += ScopeEdited;
            UpdateStatus("");
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical();

            DrawLine();
            GUILayout.Label(_status);
            DrawLine();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Scope name to edit: ");
            _newScopeName = GUILayout.TextField(_newScopeName, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            DrawLine();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Cancel"))
            {
                Localizer.Instance.ScopeEdited -= ScopeEdited;
                Close();
            }

            if (GUILayout.Button("Save"))
            {
                try
                {
                    Localizer.Instance.EditScope(_scope, _newScopeName);
                    Localizer.Instance.ScopeEdited -= ScopeEdited;
                    Localizer.Instance.WriteData();
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

        private void ScopeEdited(LocalizationScope scope)
        {
            if (_scope.Equals(scope))
            {
                Localizer.Instance.ScopeEdited -= ScopeEdited;
                Close();
            }
        }
    }

}