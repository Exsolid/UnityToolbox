using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class GamestateGraphPathSelectionWindow : EditorWindow
{
    private string _assetPathInProject;
    private string _status;
    private GamestateGraphWindow _graph;
    public GamestateGraphWindow Graph
    {
        set { _graph = value; }
    }

    private void OnEnable()
    {
        _assetPathInProject = ProjectPrefs.GetString(ProjectPrefKeys.GAMESTATEDATASAVEPATH);
    }

    public static void Open(GamestateGraphWindow graph)
    {
        GamestateGraphPathSelectionWindow window = (GamestateGraphPathSelectionWindow) GetWindow(typeof(GamestateGraphPathSelectionWindow));
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
            if (ResourcesUtil.TrySetValidPath(Application.dataPath + "/" + _assetPathInProject, ProjectPrefKeys.GAMESTATEDATASAVEPATH))
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
