using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace UnityToolbox.Item.Management.Editor
{
    public class ItemScopeEditWindow : EditorWindow
    {
        private ItemScope _scope;
        public ItemScope Scope
        {
            set
            {
                _scope = value;
                _newScopeName = value.Name;
            }
        }

        private string _newScopeName;

        private string _status;

        public static void Open(ItemScope scope)
        {
            ItemScopeEditWindow window = (ItemScopeEditWindow)GetWindow(typeof(ItemScopeEditWindow));
            window.titleContent = new GUIContent("Edit Scope");
            window.ShowUtility();
            window.minSize = new Vector2(400, 100);
            window.maxSize = new Vector2(400, 100);
            window.Scope = scope;
        }

        private void Awake()
        {
            UpdateStatus("");
            Itemizer.Instance.OnItemScopeEdited += ScopeEdited;
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical();

            DrawLineHorizontal();
            GUILayout.Label(_status);
            DrawLineHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Scope name to edit: ");
            _newScopeName = GUILayout.TextField(_newScopeName, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            DrawLineHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Cancel"))
            {
                Itemizer.Instance.OnItemScopeEdited -= ScopeEdited;
                Close();
            }

            if (GUILayout.Button("Save"))
            {
                Itemizer.Instance.OnItemScopeEdited -= ScopeEdited;
                try
                {
                    Itemizer.Instance.EditItemScope(_scope, _newScopeName);
                    Itemizer.Instance.WriteData();
                    AssetDatabase.Refresh();
                    Close();
                }
                catch (Exception e)
                {
                    UpdateStatus(e.Message);
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void ScopeEdited(ItemScope scope, ItemScope newScope)
        {
            if (_scope.Equals(scope))
            {
                Itemizer.Instance.OnItemScopeEdited -= ScopeEdited;
                Close();
            }
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
    }
}
