using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.Reflection;

namespace UnityToolbox.Item.Management.Editor
{
    public class ItemDefinitionErrorWindow : EditorWindow
    {
        public event Action OnClose;

        private Vector2 _scrollPos;
        private string _searchString;
        private bool _foldoutSearch;
        private string _status;
        private int _selectedSearch;
        private List<Type> _itemDefinitionTypes;
        private List<bool> _selectionBoolValues;
        private List<float> _selectionFloatValues;
        private List<int> _selectionIntValues;
        private List<string> _selectionStringValues;
        private List<FieldInfo> _selectionFields;

        public static ItemDefinitionErrorWindow Open()
        {
            ItemDefinitionErrorWindow window = (ItemDefinitionErrorWindow)GetWindow(typeof(ItemDefinitionErrorWindow));
            window.minSize = new Vector2(600, 400);
            window.maxSize = new Vector2(600, 400);
            window.titleContent = new GUIContent("Faulty ItemDefinitons");

            Vector2 mouse = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            Rect rect = new Rect(mouse.x - 550, mouse.y + 10, 10, 10);

            window.ShowAsDropDown(rect, new Vector2(600, 200));

            return window;
        }

        private void Awake()
        {
            Itemizer.Instance.Initialize();
            _itemDefinitionTypes = new List<Type>();
            _searchString = "";
            _itemDefinitionTypes.Clear();
            _itemDefinitionTypes.Add(typeof(ItemDefinition));
            _selectedSearch = 0;
            if (Itemizer.Instance.Initialized)
            {
                _itemDefinitionTypes.AddRange(Itemizer.Instance.GetAllInheritedItemDefinitionTypes());
            }
            SetupListsForItemType(_itemDefinitionTypes[_selectedSearch]);
            _searchString = "";
            UpdateStatus("");
        }

        private void OnGUI()
        {
            if(Itemizer.Instance.FaultyItemDefinitions.Count == 0)
            {
                Close();
            }

            DisplayItemsTab();
        }

        private void DisplayItemsTab()
        {
            DrawLineHorizontal();
            GUILayout.Label(_status);
            DrawLineHorizontal();
            GUILayout.Label("All objects listed here are faulty. Were the prefab or icon moved? They need to be fixed!");
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

                EditorGUILayout.BeginHorizontal();
                List<string> typeSelection = new List<string>();
                typeSelection.Add("All");
                typeSelection.AddRange(_itemDefinitionTypes.Select(t => t.Name));
                if (_selectedSearch >= typeSelection.Count())
                {
                    _selectedSearch = 0;
                }
                GUILayout.Label("Of Type:");
                _selectedSearch = EditorGUILayout.Popup(_selectedSearch, typeSelection.ToArray(), GUILayout.Width(402));
                GUILayout.EndHorizontal();
            }

            DrawListItems();
        }

        private void DrawListItems()
        {
            DrawLineHorizontal();
            HashSet<ItemDefinition> filtered = Itemizer.Instance.FaultyItemDefinitions;
            if (!_searchString.Trim().Equals(""))
            {
                filtered = filtered.Where(itemDefinition => itemDefinition.GetQualifiedName().ToLower().Contains(_searchString.Trim().ToLower())).ToHashSet();
            }

            if (_selectedSearch != 0)
            {
                filtered = filtered.Where(itemDefinition => itemDefinition.GetType().Name.Equals(_itemDefinitionTypes[_selectedSearch - 1].Name)).ToHashSet();
            }

            GUILayout.BeginVertical();
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            foreach (ItemDefinition itemDefinition in filtered)
            {
                GUILayout.BeginHorizontal("Box");

                GUILayout.Label(itemDefinition.GetQualifiedName(), GUILayout.Width((EditorGUIUtility.currentViewWidth - 68) / 2));
                GUILayout.Label(itemDefinition.GetType().Name);

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

        private void SetupListsForItemType(Type type)
        {
            _selectionBoolValues = new List<bool>();
            _selectionFloatValues = new List<float>();
            _selectionIntValues = new List<int>();
            _selectionStringValues = new List<string>();
            _selectionFields = new List<FieldInfo>();

            if (type.Equals(typeof(ItemDefinition)))
            {
                return;
            }

            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (field.FieldType.Equals(typeof(int)))
                {
                    _selectionIntValues.Add(0);
                }
                else if (field.FieldType.Equals(typeof(float)))
                {
                    _selectionFloatValues.Add(0);
                }
                else if (field.FieldType.Equals(typeof(bool)))
                {
                    _selectionBoolValues.Add(false);
                }
                else if (field.FieldType.Equals(typeof(string)))
                {
                    _selectionStringValues.Add("");
                }
                _selectionFields.Add(field);
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

        public new void Close()
        {
            base.Close();
            OnClose?.Invoke();
        }
    }
}

