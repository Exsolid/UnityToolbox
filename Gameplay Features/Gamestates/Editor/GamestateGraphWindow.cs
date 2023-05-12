using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

public class GamestateGraphWindow : EditorWindow
{
    private const string FILENAME = "GamestateData.txt";
    private GamestateGraphView _graphView;

    [MenuItem("UnityToolbox/Gamestate Graph")]
    public static void Open()
    {
        GamestateGraphWindow window = GetWindow<GamestateGraphWindow>("Gamestate Graph");
        window.OnEnable();
    }

    private void AddGraphView()
    {
        _graphView = new GamestateGraphView();
        rootVisualElement.Add(_graphView);
        _graphView.StretchToParentSize();
    }

    public void UpdateData()
    {
        _graphView.DeleteElements(_graphView.nodes.ToList());

        List<GamestateNodeData> nodes = ResourcesUtil.GetFileData<List<GamestateNodeData>>(ProjectPrefKeys.GAMESTATEDATASAVEPATH, FILENAME);

        if (nodes != null)
        {
            foreach (GamestateNodeData node in nodes)
            {
                GamestateNode gNode = new GamestateNode(node);
                _graphView.AddNode(gNode);
            }

            foreach (Node node in _graphView.nodes.ToList())
            {
                GamestateNode gNode = (GamestateNode)node;
                foreach (int i in gNode.OutputIDs)
                {
                    GamestateNode nextNode = (GamestateNode)_graphView.nodes.ToList().Where(n => ((GamestateNode)n).ID == i).FirstOrDefault();
                    int number = gNode.OutputPort.connections.Count();

                    Edge edge = gNode.OutputPort.ConnectTo(nextNode.InputPort);
                    _graphView.Add(edge);
                }
            }        
        }
    }

    private void WriteData()
    {
        if (Directory.Exists(ResourcesUtil.GetLocalPath(ProjectPrefKeys.GAMESTATEDATASAVEPATH)))
        {
            List<GamestateNodeData> nodes = new List<GamestateNodeData>();

            foreach (Node node in _graphView.nodes.ToList())
            {
                GamestateNode gNode = (GamestateNode)node;
                gNode.UpdateValues();

                GamestateNodeData data = new GamestateNodeData();

                string uniqueName = gNode.GamestateName;
                int i = 0;
                while(nodes.Where(n => n.Name.Equals(uniqueName)).Any())
                {
                    uniqueName = gNode.GamestateName + i;
                    i++;
                }

                if(i != 0)
                {
                    Debug.LogWarning("The defined name " + gNode.GamestateName + " already exists and is renamed to " + uniqueName + ".");
                }

                data.Name = uniqueName;
                data.Position = new VectorData(gNode.GetPosition().position);
                data.ID = gNode.ID;
                data.InputIDs = gNode.InputIDs;
                data.OutputIDs = gNode.OutputIDs;

                nodes.Add(data);
            }
            ResourcesUtil.WriteFile(ProjectPrefKeys.GAMESTATEDATASAVEPATH, FILENAME, nodes);
            AssetDatabase.Refresh();
        }
    }

    private void OnAfterAssemblyReload()
    {
            UpdateData();
    }

    private void OnDestroy()
    {
        WriteData();
    }

    private void OnEnable()
    {
        AddGraphView();

        if (ProjectPrefs.GetString(ProjectPrefKeys.GAMESTATEDATASAVEPATH) == null || ProjectPrefs.GetString(ProjectPrefKeys.GAMESTATEDATASAVEPATH).Equals(""))
        {
            GamestateGraphPathSelectionWindow.Open(this);
        }
        else
        {
            List<GamestateNodeData> nodes = ResourcesUtil.GetFileData<List<GamestateNodeData>>(ProjectPrefKeys.GAMESTATEDATASAVEPATH, FILENAME);
            if (nodes != null)
            {
                UpdateData();
            }
            else
            {
                GamestateGraphPathSelectionWindow.Open(this);
            }
        }

        AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
    }

    private void OnDisable()
    {
        AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
    }
}
