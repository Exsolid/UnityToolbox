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
        _assetPathInProject = ResourcesUtil.GetProjectPath(ProjectPrefKeys.DIALOGSAVEPATH);
    }

    public static void Open(DialogGraphWindow graph)
    {
        DialogGraphPathSelectionWindow window = (DialogGraphPathSelectionWindow) GetWindow(typeof(DialogGraphPathSelectionWindow));
        window.titleContent = new GUIContent("Edit Path");
        window.ShowUtility();
        window.maxSize = new Vector2(600, 100);
        window.minSize = new Vector2(600, 100);
        window.Graph = graph;
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        DisplaySetting();
        GUILayout.EndVertical();
    }

    private void DisplaySetting()
    {
        GUILayout.Label("No valid path or data can be found. Please updated to a valid path, otherwise nothing will be saved. \nIt is required that it containes \"Resources\".");

        GUILayout.Label(_status);
        GUILayout.BeginHorizontal();
        Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(Application.dataPath));
        GUILayout.Label(Application.dataPath + "/");
        _assetPathInProject = GUILayout.TextField(_assetPathInProject, GUILayout.Width(585 - textDimensions.x));
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Refresh"))
        {
            if (ResourcesUtil.TrySetValidPath(Application.dataPath + "/" + _assetPathInProject, ProjectPrefKeys.DIALOGSAVEPATH))
            {
                AssetDatabase.Refresh();
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
