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
using UnityEditor.UIElements;
using UnityToolbox.GameplayFeatures.SerializationData;
using UnityToolbox.General.Management;
using UnityToolbox.General.Preferences;
using UnityToolbox.UI.Dialog;

namespace UnityToolbox.UI.Dialog.Editor
{
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

        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();
            rootVisualElement.Add(toolbar);

            Button saveButton = new Button()
            {
                text = "Save"
            };

            saveButton.clicked += WriteData;

            Button settingsButton = new Button()
            {
                text = "Directory Settings"
            };

            settingsButton.clicked += () => 
            {
                DialogGraphPathSelectionWindow.Open(this);
            };

            toolbar.Add(saveButton);
            toolbar.Add(settingsButton);
        }

        public void UpdateData()
        {
            _graphView.DeleteElements(_graphView.nodes.ToList());

            List<DialogNodeData> nodes = ResourcesUtil.GetFileData<List<DialogNodeData>>(ProjectPrefKeys.DIALOGSAVEPATH, FILENAME);

            if (nodes != null)
            {
                foreach (DialogNodeData node in nodes)
                {
                    DialogNodeBase dNode;
                    if (node.IsLocalized)
                    {
                        dNode = new DialogNodeLocalized(node);
                    }
                    else
                    {
                        dNode = new DialogNode(node);
                    }

                    _graphView.AddNode(dNode);
                }

                foreach (Node node in _graphView.nodes.ToList())
                {
                    DialogNodeBase dNode = (DialogNodeBase)node;
                    foreach (int i in dNode.OutputIDs)
                    {
                        DialogNodeBase nextNode = (DialogNodeBase)_graphView.nodes.ToList().Where(n => ((DialogNodeBase)n).ID == i).FirstOrDefault();
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
                    DialogNodeBase dNode = (DialogNodeBase) node;
                    dNode.UpdateValues();

                    DialogNodeData data = new DialogNodeData
                    {
                        Position = new VectorData(dNode.GetPosition().position),
                        ID = dNode.ID,
                        InputIDs = dNode.InputIDs,
                        OutputIDs = dNode.OutputIDs,
                        DialogIdentifier = dNode.dialogIdentifier,
                        StateForDialogIdentifier = dNode.StateForDialogIndentifier,
                        GamestateToComplete = dNode.GamestateToComplete,

                        AvatarReference = AssetDatabase.GetAssetPath(dNode.Avatar)
                    };

                    if (dNode.GetType() == typeof(DialogNodeLocalized))
                    {
                        DialogNodeLocalized dNodeLocalized = (DialogNodeLocalized) node;

                        data.IsLocalized = true;
                        data.OptionsLocalized = dNodeLocalized.Options;
                        data.TitleLocalized = dNodeLocalized.DialogTitle;
                        data.TextLocalized = dNodeLocalized.DialogText;
                    }
                    else if (dNode.GetType() == typeof(DialogNode))
                    {
                        DialogNode dNodeDefault = (DialogNode) node;

                        data.Options = dNodeDefault.Options;
                        data.Title = dNodeDefault.DialogTitle;
                        data.Text = dNodeDefault.DialogText;
                    }

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
            rootVisualElement.Clear();
            AddGraphView();
            AddToolbar();

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
}
