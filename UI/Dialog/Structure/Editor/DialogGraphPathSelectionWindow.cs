using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class DialogGraphPathSelectionWindow : EditorWindow
{
    private string _assetPathInProject;
    private string _status;
    private DialogGraphWindow _graph;
    public DialogGraphWindow Graph
    {
        set { _graph = value; }
    }

    private void OnEnable()
    {
        _assetPathInProject = ProjectPrefs.GetString(ProjectPrefKeys.DIALOGSAVEPATH);
    }

    public static void Open(DialogGraphWindow graph)
    {
        DialogGraphPathSelectionWindow window = (DialogGraphPathSelectionWindow) GetWindow(typeof(DialogGraphPathSelectionWindow));
        window.titleContent = new GUIContent("Edit Path");
        window.ShowUtility();
        window.maxSize = new Vector2(600, 75);
        window.minSize = new Vector2(600, 75);
        window.Graph = graph;
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        DisplaySetting();
        GUILayout.EndVertical();
    }

    public void DisplaySetting()
    {
        GUILayout.Label("No valid path or data can be found. Please updated to a valid path, otherwise nothing will be saved.");
        GUILayout.Label(_status);
        GUILayout.BeginHorizontal();
        Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(Application.dataPath));
        GUILayout.Label(Application.dataPath);
        _assetPathInProject = GUILayout.TextField(_assetPathInProject, GUILayout.Width(585 - textDimensions.x));
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Refresh"))
        {
            if (Directory.Exists(Application.dataPath+_assetPathInProject))
            {
                ProjectPrefs.SetString(ProjectPrefKeys.DIALOGSAVEPATH, _assetPathInProject);
                _graph.UpdateData();
                this.Close();
            }
            else
            {
                _status = "Path is not valid!";
            }
        }
    }
}
