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
    private const string FILENAME = "DialogData.txt";
    private DialogGraphView _graphView;

    [MenuItem("UnityToolbox/Dialog Graph")]
    public static void Open()
    {
        DialogGraphWindow window = GetWindow<DialogGraphWindow>("Dialog Graph");
        window.OnEnable();
    }

    private void AddGraphView()
    {
        _graphView = new DialogGraphView();
        rootVisualElement.Add(_graphView);
        _graphView.StretchToParentSize();
    }

    public void UpdateData()
    {
        _graphView.DeleteElements(_graphView.nodes.ToList());

        List<DialogNodeData> nodes = ResourcesUtil.GetFileData<List<DialogNodeData>>(ProjectPrefKeys.DIALOGSAVEPATH, FILENAME);

        if(nodes != null)
        {
            foreach (DialogNodeData node in nodes)
            {
                DialogNode dNode = new DialogNode(node);
                _graphView.AddNode(dNode);
            }

            foreach (Node node in _graphView.nodes.ToList())
            {
                DialogNode dNode = (DialogNode)node;
                foreach (int i in dNode.OutputIDs)
                {
                    DialogNode nextNode = (DialogNode)_graphView.nodes.ToList().Where(n => ((DialogNode)n).ID == i).FirstOrDefault();
                    int number = dNode.OutputPort.connections.Count();
                    nextNode.UpdateInputConnectionLabel(number);

                    Edge edge = dNode.OutputPort.ConnectTo(nextNode.InputPort);
                    _graphView.Add(edge);
                }
            }
        }
    }

    private void WriteData()
    {
        if (Directory.Exists(ResourcesUtil.GetLocalPath(ProjectPrefKeys.DIALOGSAVEPATH)))
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
                data.StateForDialogIndentifier = dNode.StateForDialogIndentifier;
                data.GamestateToComplete = dNode.GamestateToComplete;

                data.AvatarReference = AssetDatabase.GetAssetPath(dNode.Avatar);

                nodes.Add(data);
            }
            ResourcesUtil.WriteFile(ProjectPrefKeys.DIALOGSAVEPATH, FILENAME, nodes);
            AssetDatabase.Refresh();
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

        if (ProjectPrefs.GetString(ProjectPrefKeys.DIALOGSAVEPATH) == null || ProjectPrefs.GetString(ProjectPrefKeys.DIALOGSAVEPATH).Equals(""))
        {
            DialogGraphPathSelectionWindow.Open(this);
        }
        else
        {
            List<DialogNodeData> nodes = ResourcesUtil.GetFileData<List<DialogNodeData>>(ProjectPrefKeys.DIALOGSAVEPATH, FILENAME);
            if (nodes != null)
            {
                UpdateData();
            }
            else
            {
                DialogGraphPathSelectionWindow.Open(this);
            }
        }

        AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
    }

    private void OnDisable()
    {
        AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
    }
}
