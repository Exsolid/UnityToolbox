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

public class DialogGraphWindow : EditorWindow
{
    private string _fullPath;
    private const string FILENAME = "/DialogData.dat";
    private JsonSerializerSettings _settings;
    private DialogGraphView _graphView;

    [MenuItem("UnityToolbox/Dialog Graph")]
    public static void Open()
    {
        DialogGraphWindow window = GetWindow<DialogGraphWindow>("Dialog Graph");
        if(ProjectPrefs.GetString(ProjectPrefKeys.DIALOGSAVEPATH) == null || !File.Exists(Application.dataPath + ProjectPrefs.GetString(ProjectPrefKeys.DIALOGSAVEPATH) + FILENAME))
        {
            DialogGraphPathSelectionWindow.Open(window);
        }
        else
        {
            window.UpdateData();
        }
    }

    private void AddGraphView()
    {
        _graphView = new DialogGraphView();
        rootVisualElement.Add(_graphView);
        _graphView.StretchToParentSize();
    }

    public void UpdateData()
    {
        _fullPath = Application.dataPath + ProjectPrefs.GetString(ProjectPrefKeys.DIALOGSAVEPATH);
        if (File.Exists(_fullPath + FILENAME))
        {
            string data = File.ReadAllText(_fullPath + FILENAME);
            List<DialogNodeData> nodes = JsonConvert.DeserializeObject<List<DialogNodeData>>(data, _settings);
            List<int> ids = new List<int>();

            foreach (DialogNodeData node in nodes)
            {
                DialogNode dNode = new DialogNode(node);
                _graphView.AddNode(dNode);
            }

            foreach (Node node in _graphView.nodes.ToList())
            {
                DialogNode dNode = (DialogNode)node;
                foreach(int i in dNode.OutputIDs)
                {
                    DialogNode nextNode = (DialogNode) _graphView.nodes.ToList().Where(n => ((DialogNode)n).ID == i).FirstOrDefault();
                    int number = dNode.OutputPort.connections.Count();
                    nextNode.UpdateInputConnectionLabel(number);

                    Edge edge = dNode.OutputPort.ConnectTo(nextNode.InputPort);
                    _graphView.Add(edge);
                }
            }
        }
    }

    public void WriteData()
    {
        if (Directory.Exists(_fullPath))
        {
            List<DialogNodeData> nodes = new List<DialogNodeData>();

            foreach (Node node in _graphView.nodes.ToList())
            {
                DialogNode dNode = (DialogNode)node;
                dNode.UpdateValues();

                DialogNodeData data = new DialogNodeData();
                data.Title = dNode.DialogTitle;
                data.Text = dNode.DialogText;
                data.Position = new VectorData(dNode.GetPosition().position);
                data.Options = dNode.Options;
                data.ID = dNode.ID;
                data.InputIDs = dNode.InputIDs;
                data.OutputIDs = dNode.OutputIDs;
                data.DialogIndentifier = dNode.DialogIndentifier;
                data.CompletionToSet = dNode.CompletionToSet;

                data.AvatarReference = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(dNode.Avatar));

                nodes.Add(data);
            }
            string localisationData = JsonConvert.SerializeObject(nodes, _settings);
            File.WriteAllText(_fullPath + FILENAME, localisationData);
        }
    }

    public void OnAfterAssemblyReload()
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
        _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        _fullPath = Application.dataPath + ProjectPrefs.GetString(ProjectPrefKeys.DIALOGSAVEPATH);
        AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
    }

    private void OnDisable()
    {
        AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
    }
}
