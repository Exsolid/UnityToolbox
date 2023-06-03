using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;
using System;
using System.Reflection;


namespace UnityToolbox.Item.Management
{
    public class ItemManagerWindow : EditorWindow
    {
        private int _selectedTab = 0;
        private const int IDS = 0;
        private const int SCOPES = 1;
        private const int SETTINGS = 2;

        private string _assetPathInProject;

        private Vector2 _scrollPosScope;
        private string _scopeName;

        private Vector2 _scrollPosItemDefinition;
        private string _itemDefinitionName;
        private int _selectedScope;
        private Texture2D _itemDefinitionIcon;
        private GameObject _itemDefinitionPrefab;
        private int _maxStackCount;
        private int _selectedType;
        private List<Type> _itemDefinitionTypes;

        private Vector2 _scrollPosCreation;
        private List<bool> _selectionBoolValues;
        private List<string> _selectionStringValues;
        private List<float> _selectionFloatValues;
        private List<int> _selectionIntValues;
        private List<FieldInfo> _selectionFields;

        private string _searchIDString;
        private int _selectedSearch;
        private bool _foldoutAdd;
        private bool _foldoutSearch;

        private string _status;

        private ItemDefinitionErrorWindow _errorWindow;

        [MenuItem("UnityToolbox/Item Manager")]
        private static void DisplayWindow()
        {
            ItemManagerWindow window = (ItemManagerWindow)GetWindow(typeof(ItemManagerWindow));
            window.titleContent = new GUIContent("Item Manager");
            window.maxSize = new Vector2(600, 400);
            window.minSize = new Vector2(600, 400);
            window.ShowUtility();
        }

        public static void Open()
        {
            ItemManagerWindow window = (ItemManagerWindow)GetWindow(typeof(ItemManagerWindow));
            window.titleContent = new GUIContent("Item Manager");

            Vector2 mouse = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            Rect r = new Rect(mouse.x - 450, mouse.y + 10, 10, 10);

            window.maxSize = new Vector2(600, 400);
            window.minSize = new Vector2(600, 400);

            window.ShowAsDropDown(r, new Vector2(600, 400));
        }

        private void InitializeWindow()
        {
            _itemDefinitionTypes = new List<Type>();
            _assetPathInProject = ResourcesUtil.GetProjectPath(ProjectPrefKeys.ITEMDATASAVEPATH);
            _searchIDString = "";
            _itemDefinitionTypes.Clear();
            _itemDefinitionTypes.Add(typeof(ItemDefinition));
            _selectedType = 0;
            Itemizer.Instance.Initialize();
            if (Itemizer.Instance.Initialized)
            {
                _itemDefinitionTypes.AddRange(Itemizer.Instance.GetAllInheritedItemDefinitionTypes());
            }
            SetupListsForItemType(_itemDefinitionTypes[_selectedType]);
            UpdateStatus("");
        }

        private void Awake()
        {
            InitializeWindow();
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();

            DrawLineHorizontal();
            GUILayout.Label(_status);
            DrawLineHorizontal();
            if (Itemizer.Instance.Initialized)
            {
                if (Itemizer.Instance.FaultyItemDefinitions.Count != 0 && _errorWindow == null)
                {
                    _errorWindow = ItemDefinitionErrorWindow.Open();
                    _errorWindow.OnClose += delegate
                    {
                        _errorWindow = null;
                        UpdateStatus("All faulty ItemDefinitions were fixed.");
                    };

                    UpdateStatus("Fix the faulty ItemDefinitions first!");
                }
                else if(_errorWindow == null)
                {
                    _selectedTab = GUILayout.Toolbar(_selectedTab, new string[] { "Items", "Scopes", "Settings" });
                    DrawLineHorizontal();
                    switch (_selectedTab)
                    {
                        case IDS:
                            DisplayItemsTab();
                            break;
                        case SCOPES:
                            DisplayScopesTab();
                            break;
                        case SETTINGS:
                            DisplaySettingsTab();
                            break;
                    }
                }
            }
            else
            {
                DisplaySettingsTab();
            }

            GUILayout.EndVertical();
        }

        private void DisplaySettingsTab()
        {
            GUILayout.Label("To update the " + nameof(Itemizer) + " path, please enter a valid path below. \nIt is required that it containes \"Resources\".");
            DrawLineHorizontal();
            GUILayout.BeginHorizontal();
            Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(Application.dataPath));
            GUILayout.Label(Application.dataPath + "/");
            _assetPathInProject = GUILayout.TextField(_assetPathInProject, GUILayout.Width(585 - textDimensions.x));
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Refresh"))
            {
                if (!ResourcesUtil.TrySetValidPath(Application.dataPath + "/" + _assetPathInProject, ProjectPrefKeys.ITEMDATASAVEPATH))
                {
                    UpdateStatus("\"" + Path.GetFullPath(Application.dataPath + "/" + _assetPathInProject) + "\" is not valid.");
                }
                else
                {
                    Itemizer.Instance.Initialize();
                    _itemDefinitionTypes.Clear();
                    _itemDefinitionTypes.Add(typeof(ItemDefinition));
                    _itemDefinitionTypes.AddRange(Itemizer.Instance.GetAllInheritedItemDefinitionTypes());
                    SetupListsForItemType(_itemDefinitionTypes[_selectedType]);
                    UpdateStatus("Path updated.");
                }
            }
        }

        private void DisplayScopesTab()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Add scope with name: ");
            _scopeName = GUILayout.TextField(_scopeName, GUILayout.Width(200));
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                try
                {
                    Itemizer.Instance.AddItemScope(_scopeName);
                    Itemizer.Instance.WriteData();
                    AssetDatabase.Refresh();
                    UpdateStatus("Successfully added a new scope.");
                }
                catch (ItemDefinitionException e)
                {
                    UpdateStatus(e.Message);
                }
            }
            GUILayout.EndHorizontal();
            DrawLineHorizontal();

            DrawListScopes();
        }

        private void DisplayItemsTab()
        {
            GUILayout.BeginHorizontal();
            _foldoutAdd = EditorGUILayout.Foldout(_foldoutAdd, "Add Item Definition");
            GUILayout.EndHorizontal();

            if (_foldoutAdd)
            {
                DrawCreationFields();
                DrawAddDefinedItem();
            }

            DrawLineHorizontal();

            GUILayout.BeginHorizontal();
            _foldoutSearch = EditorGUILayout.Foldout(_foldoutSearch, "Search");
            GUILayout.EndHorizontal();

            if (_foldoutSearch)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Contains:");
                _searchIDString = EditorGUILayout.TextField(_searchIDString, GUILayout.Width(400));
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

        private void DrawListScopes()
        {
            GUILayout.BeginVertical();
            _scrollPosScope = GUILayout.BeginScrollView(_scrollPosScope);
            foreach (ItemScope scope in Itemizer.Instance.ItemScopes)
            {
                GUILayout.BeginHorizontal("Box");
                GUILayout.Label(scope.Name);
                if (!scope.Equals(Itemizer.Instance.DefaultScope))
                {
                    if (GUILayout.Button("*", GUILayout.Width(20)))
                    {
                        ItemScopeEditWindow.Open(scope);
                    }

                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        if (EditorUtility.DisplayDialog("Remove Scope", "Are you sure you want to delete the scope '" + scope.Name + "'? \nItem Definitions using this scope will have it replaced with the default scope.", "Yes"))
                        {
                            try
                            {
                                Itemizer.Instance.RemoveItemScope(scope);
                                Itemizer.Instance.WriteData();
                                AssetDatabase.Refresh();
                                UpdateStatus("Successfully removed the scope '" + scope.Name + "'.");
                            }
                            catch (ItemDefinitionException e)
                            {
                                UpdateStatus(e.Message);
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawCreationFields()
        {
            GUILayout.BeginVertical(GUILayout.Height(110));
            _scrollPosCreation = GUILayout.BeginScrollView(_scrollPosCreation);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Item type: ");

            int changeCheck = _selectedType;
            _selectedType = EditorGUILayout.Popup("", _selectedType, _itemDefinitionTypes.Select(type => type.Name).ToArray(), GUILayout.Width(402));

            if (_selectedType >= _itemDefinitionTypes.Count())
            {
                _selectedType = 0;
            }

            if (changeCheck != _selectedType)
            {
                SetupListsForItemType(_itemDefinitionTypes[_selectedType]);
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Item name: ");
            _itemDefinitionName = GUILayout.TextField(_itemDefinitionName, GUILayout.Width(200));
            GUILayout.Label(ItemDefinition.DEVIDER.ToString(), GUILayout.Width(10));
            string[] scopes = Itemizer.Instance.ItemScopes.Select(x => x.Name).ToArray();
            _selectedScope = EditorGUILayout.Popup("", _selectedScope, scopes, GUILayout.Width(182));

            if (_selectedScope >= scopes.Count())
            {
                _selectedScope = 0;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Max Stack Count: ");
            _maxStackCount = EditorGUILayout.IntField(_maxStackCount, GUILayout.Width(400));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Icon: ");
            _itemDefinitionIcon = (Texture2D)EditorGUILayout.ObjectField(_itemDefinitionIcon, typeof(Texture2D), false, GUILayout.Width(400));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Prefab: ");
            _itemDefinitionPrefab = (GameObject)EditorGUILayout.ObjectField(_itemDefinitionPrefab, typeof(GameObject), false, GUILayout.Width(400));
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
                    _selectionBoolValues[bools] = EditorGUILayout.Toggle(_selectionBoolValues[bools], GUILayout.Width(400));
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

        private void DrawListItems()
        {
            DrawLineHorizontal();
            HashSet<ItemDefinition> filtered = Itemizer.Instance.ItemDefinitions;
            if (!_searchIDString.Trim().Equals(""))
            {
                filtered = filtered.Where(itemDefinition => itemDefinition.GetQualifiedName().ToLower().Contains(_searchIDString.Trim().ToLower())).ToHashSet();
            }

            if (_selectedSearch != 0)
            {
                filtered = filtered.Where(itemDefinition => itemDefinition.GetType().Name.Equals(_itemDefinitionTypes[_selectedSearch - 1].Name)).ToHashSet();
            }

            GUILayout.BeginVertical();
            _scrollPosItemDefinition = GUILayout.BeginScrollView(_scrollPosItemDefinition);

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

        private void DrawAddDefinedItem()
        {
            GUILayout.BeginHorizontal();
            int ints = 0;
            int strings = 0;
            int bools = 0;
            int floats = 0;
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                try
                {
                    if (_selectedType == 0)
                    {
                        Itemizer.Instance.AddItemDefinition(Itemizer.Instance.ItemScopes.ElementAt(_selectedScope), _itemDefinitionName, _maxStackCount,
                            AssetDatabase.GetAssetPath(_itemDefinitionIcon).Split("Resources/").Last(), AssetDatabase.GetAssetPath(_itemDefinitionPrefab).Split("Resources/").Last(),
                            AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(_itemDefinitionIcon)).ToString(), AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(_itemDefinitionPrefab)).ToString());
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
                            Itemizer.Instance.ItemScopes.ElementAt(_selectedScope),
                            _itemDefinitionName,
                            _maxStackCount,
                            AssetDatabase.GetAssetPath(_itemDefinitionIcon).Split("Resources/").Last(),
                            AssetDatabase.GetAssetPath(_itemDefinitionPrefab).Split("Resources/").Last(),
                            AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(_itemDefinitionIcon)).ToString(),
                            AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(_itemDefinitionPrefab)).ToString(),
                            itemFields
                        };

                        MethodInfo info = typeof(Itemizer).GetMethod(nameof(Itemizer.AddInheritedItemDefinition)).MakeGenericMethod(new Type[] { _itemDefinitionTypes[_selectedType] });
                        info.Invoke(Itemizer.Instance, paramArray);
                    }

                    Itemizer.Instance.WriteData();
                    AssetDatabase.Refresh();
                    UpdateStatus("Successfully added a new Item Definition.");
                }
                catch (Exception e)
                {
                    if(e.InnerException != null && e.InnerException.GetType().Equals(typeof(ItemDefinitionException)))
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

        void OnEnable()
        {
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        void OnDisable()
        {
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        }

        public void OnAfterAssemblyReload()
        {
            InitializeWindow();
        }
    }
}
