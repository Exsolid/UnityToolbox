using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

public class ItemDefinitionSelectionWindow : EditorWindow
{
    private Vector2 _scrollPos;
    private string _searchString;
    private bool _foldoutSearch;
    private string _status;

    public event Action<ItemDefinition> OnItemDefinitionSelected;

    public static ItemDefinitionSelectionWindow Open()
    {
        ItemDefinitionSelectionWindow window = (ItemDefinitionSelectionWindow)GetWindow(typeof(ItemDefinitionSelectionWindow));
        window.minSize = new Vector2(600, 400);
        window.maxSize = new Vector2(600, 400);
        window.titleContent = new GUIContent("Dropdown Selection");

        Vector2 mouse = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        Rect rect = new Rect(mouse.x - 550, mouse.y + 10, 10, 10);

        window.ShowAsDropDown(rect, new Vector2(600, 200));

        return window;
    }

    private void Awake()
    {
        Itemizer.Instance.Initialize();
        _searchString = "";
        UpdateStatus("");
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();

        DrawLineHorizontal();
        GUILayout.Label(_status);
        DrawLineHorizontal();

        GUILayout.BeginHorizontal();
        _foldoutSearch = EditorGUILayout.Foldout(_foldoutSearch, "Search");
        GUILayout.EndHorizontal();

        if (_foldoutSearch)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Contains:");
            _searchString = EditorGUILayout.TextField(_searchString, GUILayout.Width(400));
            GUILayout.EndHorizontal();
        }

        DrawLineHorizontal();
        HashSet<ItemDefinition> filtered = Itemizer.Instance.ItemDefinitions;
        if (!_searchString.Trim().Equals(""))
        {
            filtered = filtered.Where(itemDefinition => itemDefinition.GetQualifiedName().ToLower().Contains(_searchString.Trim().ToLower())).ToHashSet();
        }

        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        _scrollPos = GUILayout.BeginScrollView(_scrollPos);

        foreach (ItemDefinition itemDefinition in filtered)
        {
            GUILayout.BeginHorizontal("Box");

            GUILayout.Label(itemDefinition.GetQualifiedName());

            if (GUILayout.Button("^", GUILayout.Width(20)))
            {
                OnItemDefinitionSelected?.Invoke(itemDefinition);
            }

            if (GUILayout.Button("*", GUILayout.Width(20)))
            {
                ItemDefinitionEditWindow.Open(itemDefinition);
            }

            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                if (EditorUtility.DisplayDialog("Remove ItemDefinition", "Are you sure you want to delete the Item Definition \"" + itemDefinition.GetQualifiedName() + "\"?", "Yes"))
                {
                    Itemizer.Instance.RemoveItemDefinition(itemDefinition);
                    Itemizer.Instance.WriteData();
                    AssetDatabase.Refresh();
                    UpdateStatus("Successfully removed the Item Definition \"" + itemDefinition.GetQualifiedName() + "\".");
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
}

