using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;

public class ItemManagerWindow : EditorWindow
{
    private int _selectedTab = 0;
    private const int IDS = 0;
    private const int SCOPES = 1;
    private const int SETTINGS = 2;

    private string _assetPathInProject;
    private bool _isValidPath;

    private string _status;

    [MenuItem("UnityToolbox/Item Manager")]
    private static void DisplayWindow()
    {
        ItemManagerWindow window = (ItemManagerWindow) GetWindowWithRect(typeof(ItemManagerWindow), new Rect(0, 0, 600, 400));
        window.titleContent = new GUIContent("Item Manager");
        window.ShowUtility();
    }

    public static void Open()
    {
        LocalisationManagerWindow window = (LocalisationManagerWindow)GetWindow(typeof(LocalisationManagerWindow));
        window.titleContent = new GUIContent("Localisation Manager");

        Vector2 mouse = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        Rect r = new Rect(mouse.x - 450, mouse.y + 10, 10, 10);

        window.ShowAsDropDown(r, new Vector2(600, 400));
    }

    private void InitializeWindow()
    {
        _assetPathInProject = ProjectPrefs.GetString(ProjectPrefKeys.ITEMDATASAVEPATH);
        _isValidPath = IsPathValid(Application.dataPath + _assetPathInProject);
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
        if (_isValidPath)
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
        else
        {
            DisplaySettingsTab();
        }

        GUILayout.EndVertical();
    }

    private void DisplaySettingsTab()
    {
        GUILayout.Label("To update the " + nameof(ItemManager) + " path. Please enter a valid path below. \nIt is required that it containes \"Resources/\".");
        DrawLineHorizontal();
        GUILayout.BeginHorizontal();
        Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(Application.dataPath));
        GUILayout.Label(Application.dataPath);
        _assetPathInProject = GUILayout.TextField(_assetPathInProject, GUILayout.Width(585 - textDimensions.x));
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Refresh"))
        {
            if (!Directory.Exists(Application.dataPath + _assetPathInProject))
            {
                UpdateStatus("The path \"" + _assetPathInProject + "\" could not be found!");
            }else if (!_assetPathInProject.Contains("Resources/"))
            {
                UpdateStatus("The path \"" + _assetPathInProject + "\" is not a \"Resources/\" directory.");
            }
            else
            {
                ProjectPrefs.SetString(ProjectPrefKeys.ITEMDATASAVEPATH, _assetPathInProject);
                _isValidPath = true;
                UpdateStatus("Path updated.");
            }
        }
    }

    private void DisplayScopesTab()
    {
        
    }

    private void DisplayItemsTab()
    {
        
        
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

    private bool IsPathValid(string path)
    {
        return Directory.Exists(path) && path.Contains("Resources/");
    }
}
