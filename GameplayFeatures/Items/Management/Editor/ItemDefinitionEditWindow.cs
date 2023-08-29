using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.Reflection;

namespace UnityToolbox.Item.Management.Editor
{
    public class ItemDefinitionEditWindow : EditorWindow
    {
        private ItemDefinition _currentItemDefinition;
        public ItemDefinition CurrentItemDefinition
        {
            set
            {
                Init(value);
            }
        }

        private Vector2 _scrollPosCreation;
        private Type _type;

        private List<bool> _selectionBoolValues;
        private List<string> _selectionStringValues;
        private List<float> _selectionFloatValues;
        private List<int> _selectionIntValues;
        private List<FieldInfo> _selectionFields;

        private string _newItemDefinitionName;
        private int _newSelectedScope;
        private Texture2D _newItemDefinitionIcon;
        private GameObject _newItemDefinitionPrefab;
        private int _newMaxStackCount;



        private string _status;

        public static void Open(ItemDefinition currentItemDefinition)
        {
            ItemDefinitionEditWindow window = (ItemDefinitionEditWindow)GetWindow(typeof(ItemDefinitionEditWindow));
            window.titleContent = new GUIContent("Edit Item Definition");
            window.ShowUtility();
            window.minSize = new Vector2(600, 170);
            window.maxSize = new Vector2(600, 170);
            window.CurrentItemDefinition = currentItemDefinition;
        }

        private void Awake()
        {
            Itemizer.Instance.OnItemDefinitionEdited += ItemDefinitionEdited;
            Itemizer.Instance.OnItemScopeEdited += ScopeEdited;
            UpdateStatus("");
        }

        private void Init(ItemDefinition value)
        {
            _currentItemDefinition = value;
            _newItemDefinitionName = value.Name;
            _newItemDefinitionIcon = Resources.Load(value.IconPath) as Texture2D;
            _newItemDefinitionPrefab = Resources.Load(value.PrefabPath) as GameObject;
            _newMaxStackCount = value.MaxStackCount;

            int i = 0;
            foreach (ItemScope scope in Itemizer.Instance.ItemScopes)
            {
                if (scope.Equals(value.Scope))
                {
                    _newSelectedScope = i;
                    break;
                }
                i++;
            }

            _selectionBoolValues = new List<bool>();
            _selectionFloatValues = new List<float>();
            _selectionIntValues = new List<int>();
            _selectionStringValues = new List<string>();
            _selectionFields = new List<FieldInfo>();

            _type = value.GetType();

            if (_type.Equals(typeof(ItemDefinition)))
            {
                return;
            }

            foreach (FieldInfo field in _type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (field.FieldType.Equals(typeof(int)))
                {
                    _selectionIntValues.Add((int)field.GetValue(value));
                }
                else if (field.FieldType.Equals(typeof(float)))
                {
                    _selectionFloatValues.Add((float)field.GetValue(value));
                }
                else if (field.FieldType.Equals(typeof(bool)))
                {
                    _selectionBoolValues.Add((bool)field.GetValue(value));
                }
                else if (field.FieldType.Equals(typeof(string)))
                {
                    _selectionStringValues.Add((string)field.GetValue(value));
                }
                _selectionFields.Add(field);
            }
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical();

            DrawLine();
            GUILayout.Label(_status);
            DrawLine();

            DrawCreationFields();
            DrawAddDefinedItem();

            GUILayout.EndVertical();
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

        private void DrawCreationFields()
        {
            GUILayout.BeginVertical(GUILayout.Height(110));
            _scrollPosCreation = GUILayout.BeginScrollView(_scrollPosCreation);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Item type: ");
            GUILayout.Label(_type.Name, GUILayout.Width(400));

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Item name: ");
            _newItemDefinitionName = GUILayout.TextField(_newItemDefinitionName, GUILayout.Width(200));
            GUILayout.Label(ItemDefinition.DEVIDER.ToString(), GUILayout.Width(10));
            string[] scopes = Itemizer.Instance.ItemScopes.Select(x => x.Name).ToArray();
            _newSelectedScope = EditorGUILayout.Popup("", _newSelectedScope, scopes, GUILayout.Width(181));

            if (_newSelectedScope >= scopes.Count())
            {
                _newSelectedScope = 0;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Max Stack Count: ");
            _newMaxStackCount = EditorGUILayout.IntField(_newMaxStackCount, GUILayout.Width(400));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Icon: ");
            _newItemDefinitionIcon = (Texture2D)EditorGUILayout.ObjectField(_newItemDefinitionIcon, typeof(Texture2D), false, GUILayout.Width(400));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Prefab: ");
            _newItemDefinitionPrefab = (GameObject)EditorGUILayout.ObjectField(_newItemDefinitionPrefab, typeof(GameObject), false, GUILayout.Width(400));
            GUILayout.EndHorizontal();

            int ints = 0;
            int strings = 0;
            int bools = 0;
            int floats = 0;
            foreach (FieldInfo field in _selectionFields)
            {
                GUILayout.BeginHorizontal();
                if (field.FieldType.Equals(typeof(int)))
                {
                    GUILayout.Label(ObjectNames.NicifyVariableName(field.Name) + ": ");
                    _selectionIntValues[ints] = EditorGUILayout.IntField(_selectionIntValues[ints], GUILayout.Width(400));
                    ints++;
                }
                else if (field.FieldType.Equals(typeof(float)))
                {
                    GUILayout.Label(ObjectNames.NicifyVariableName(field.Name) + ": ");
                    _selectionFloatValues[floats] = EditorGUILayout.FloatField(_selectionFloatValues[floats], GUILayout.Width(400));
                    floats++;
                }
                else if (field.FieldType.Equals(typeof(bool)))
                {
                    GUILayout.Label(ObjectNames.NicifyVariableName(field.Name) + ": ");
                    _selectionBoolValues[bools] = EditorGUILayout.Toggle(_selectionBoolValues[bools], GUILayout.Width(20));
                    bools++;
                }
                else if (field.FieldType.Equals(typeof(string)))
                {
                    GUILayout.Label(ObjectNames.NicifyVariableName(field.Name) + ": ");
                    _selectionStringValues[strings] = EditorGUILayout.TextField(_selectionStringValues[strings], GUILayout.Width(400));
                    strings++;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawAddDefinedItem()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Close"))
            {
                Close();
            }

            int ints = 0;
            int strings = 0;
            int bools = 0;
            int floats = 0;
            if (GUILayout.Button("Save"))
            {
                try
                {
                    if (_type.Equals(typeof(ItemDefinition)))
                    {
                        Itemizer.Instance.EditItemDefinition(_currentItemDefinition, Itemizer.Instance.ItemScopes.ElementAt(_newSelectedScope), _newItemDefinitionName, _newMaxStackCount,
                            AssetDatabase.GetAssetPath(_newItemDefinitionIcon).Split("Resources/").Last(), AssetDatabase.GetAssetPath(_newItemDefinitionPrefab).Split("Resources/").Last(),
                            AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(_newItemDefinitionIcon)).ToString(), AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(_newItemDefinitionPrefab)).ToString());
                    }
                    else
                    {
                        HashSet<ItemField> itemFields = new HashSet<ItemField>();

                        foreach (FieldInfo field in _selectionFields)
                        {
                            if (field.FieldType.Equals(typeof(int)))
                            {
                                itemFields.Add(new ItemField(field.Name, _selectionIntValues[ints]));
                                ints++;
                            }
                            else if (field.FieldType.Equals(typeof(float)))
                            {
                                itemFields.Add(new ItemField(field.Name, _selectionFloatValues[floats]));
                                floats++;
                            }
                            else if (field.FieldType.Equals(typeof(bool)))
                            {
                                itemFields.Add(new ItemField(field.Name, _selectionBoolValues[bools]));
                                bools++;
                            }
                            else if (field.FieldType.Equals(typeof(string)))
                            {
                                itemFields.Add(new ItemField(field.Name, _selectionStringValues[strings]));
                                strings++;
                            }
                        }

                        object[] paramArray = new object[]
                        {
                        _currentItemDefinition,
                        Itemizer.Instance.ItemScopes.ElementAt(_newSelectedScope),
                        _newItemDefinitionName, _newMaxStackCount,
                        AssetDatabase.GetAssetPath(_newItemDefinitionIcon).Split("Resources/").Last(),
                        AssetDatabase.GetAssetPath(_newItemDefinitionPrefab).Split("Resources/").Last(),
                        AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(_newItemDefinitionIcon)).ToString(),
                        AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(_newItemDefinitionPrefab)).ToString(),
                        itemFields
                        };

                        MethodInfo info = typeof(Itemizer).GetMethod(nameof(Itemizer.EditInheritedItemDefinition)).MakeGenericMethod(new Type[] { _type });
                        info.Invoke(Itemizer.Instance, paramArray);
                    }

                    Itemizer.Instance.WriteData();
                    AssetDatabase.Refresh();
                }
                catch (Exception e)
                {
                    if (e.InnerException != null && e.InnerException.GetType().Equals(typeof(ItemDefinitionException)))
                    {
                        UpdateStatus(e.InnerException.Message);
                    }
                    else if (e.GetType().Equals(typeof(ItemDefinitionException)))
                    {
                        UpdateStatus(e.Message);
                    }
                    else
                    {
                        throw e;
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        private void ItemDefinitionEdited(ItemDefinition oldItemDefinition, ItemDefinition newItemDefinition)
        {
            Close();
        }

        private void ScopeEdited(ItemScope oldScope, ItemScope newScope)
        {
            Close();
        }

        new public void Close()
        {
            Itemizer.Instance.OnItemDefinitionEdited -= ItemDefinitionEdited;
            Itemizer.Instance.OnItemScopeEdited -= ScopeEdited;
            base.Close();
        }


        private void UpdateStatus(string status)
        {
            _status = "Status:     " + status;
        }
    }
}
