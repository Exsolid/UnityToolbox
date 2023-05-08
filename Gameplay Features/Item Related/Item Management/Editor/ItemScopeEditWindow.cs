using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
        ItemScopeEditWindow window = (ItemScopeEditWindow) GetWindow(typeof(ItemScopeEditWindow));
        window.titleContent = new GUIContent("Edit Scope");
        window.ShowUtility();
        window.minSize = new Vector2(100, 70);
        window.Scope = scope;
    }

    private void Awake()
    {
        ItemManager.OnScopeEdited += ScopeEdited;
    }

    public void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Scope name to edit: ");
        _newScopeName = GUILayout.TextField(_newScopeName, GUILayout.Width(200));
        GUILayout.EndHorizontal();

        GUILayout.Label(_status);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Cancel"))
        {
            ItemManager.OnScopeEdited -= ScopeEdited;
            Close();
        }

        if (GUILayout.Button("Save"))
        {
            ItemManager.OnScopeEdited -= ScopeEdited;
            Close();

            _status = "Could not edit the scope, does the name already exist?";
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void ScopeEdited(ItemScope scope)
    {
        if (_scope.Equals(scope))
        {
            ItemManager.OnScopeEdited -= ScopeEdited;
            Close();
        }
    }
}
