using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LocalisationEditScopeWindow : EditorWindow
{
    private LocalisationScope _scope;
    public LocalisationScope Scope
    {
        set
        {
            _scope = value;
            _newScopeName = value.Name;
        }
    }

    private string _newScopeName;

    private string _status;

    public static void Open(LocalisationScope scope)
    {
        LocalisationEditScopeWindow window = (LocalisationEditScopeWindow) GetWindow(typeof(LocalisationEditScopeWindow));
        window.titleContent = new GUIContent("Edit Scope");
        window.ShowUtility();
        window.minSize = new Vector2(100, 70);
        window.Scope = scope;
    }

    private void Awake()
    {
        Localizer.Instance.ScopeEdited += ScopeEdited;
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
            Localizer.Instance.ScopeEdited -= ScopeEdited;
            Close();
        }

        if (GUILayout.Button("Save"))
        {
            if(Localizer.Instance.EditScope(_scope, _newScopeName))
            {
                Localizer.Instance.ScopeEdited -= ScopeEdited;
                Localizer.Instance.WriteData();
                AssetDatabase.Refresh();
                Close();
            }
            _status = "Could not edit the scope, does the name already exist?";
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void ScopeEdited(LocalisationScope scope)
    {
        if (_scope.Equals(scope))
        {
            Localizer.Instance.ScopeEdited -= ScopeEdited;
            Close();
        }
    }
}
