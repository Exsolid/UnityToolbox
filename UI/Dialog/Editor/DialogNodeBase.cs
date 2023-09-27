using System;
using System.Linq;
using System.Collections.Generic;
using UnityToolbox.UI.Dialog;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

namespace UnityToolbox.UI.Dialog.Editor
{
    /// <summary>
    /// The node which is used within the dialog graph.
    /// </summary>
    public abstract class DialogNodeBase : Node
    {
        protected string _dialogIdentifier;
        public string dialogIdentifier
        {
            get { return _dialogIdentifier; }
            set { _dialogIdentifier = value; }
        }

        protected string _stateForDialogIndentifier;
        public string StateForDialogIndentifier
        {
            get { return _stateForDialogIndentifier; }
            set { _stateForDialogIndentifier = value; }
        }

        protected string _gamestateToComplete;
        public string GamestateToComplete
        {
            get { return _gamestateToComplete; }
            set { _gamestateToComplete = value; }
        }

        protected Texture2D _avatar;
        public Texture2D Avatar
        {
            get { return _avatar; }
            set { _avatar = value; }
        }

        protected int _id;
        public int ID
        {
            get { return _id; }
        }

        protected List<int> _inputIDs;
        public List<int> InputIDs
        {
            get { return _inputIDs.ToList(); }
        }

        protected List<int> _outputIDs;
        public List<int> OutputIDs
        {
            get { return _outputIDs.ToList(); }
        }


        protected Foldout _optionFoldout;

        protected TextField _dialogIndentifierTextField;
        protected ObjectField _avatarObjectField;
        protected Label _optionNumberOfSelf;
        protected TextField _completionToSetTextField;
        protected TextField _stateForDialogIndentifierTextField;
        protected Port _inputPort;
        public Port InputPort
        {
            get { return _inputPort; }
        }

        protected Port _outputPort;
        public Port OutputPort
        {
            get { return _outputPort; }
        }


        /// <summary>
        /// Creates a node based with a given <paramref name="position"/> within the graph and a given <paramref name="id"/>.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="id"></param>
        protected DialogNodeBase(Vector2 position, int id)
        {
            _outputIDs = new List<int>();
            _inputIDs = new List<int>();
            _id = id;

            SetPosition(new Rect(position, Vector2.zero));
        }

        /// <summary>
        /// Creates a new node based on a <see cref="DialogNodeData"/>.
        /// </summary>
        /// <param name="data"></param>
        protected DialogNodeBase(DialogNodeData data)
        {
            _outputIDs = data.OutputIDs;
            _inputIDs = data.InputIDs;
            _id = data.ID;
            _gamestateToComplete = data.GamestateToComplete;
            _stateForDialogIndentifier = data.StateForDialogIdentifier;
            _dialogIdentifier = data.DialogIdentifier;

            _avatar = AssetDatabase.LoadAssetAtPath(data.AvatarReference, typeof(Texture2D)) as Texture2D;

            SetPosition(new Rect(data.Position.x, data.Position.y, 0, 0));
        }

        /// <summary>
        /// Creates all relevant UI elements to display its data.
        /// </summary>
        public abstract void Draw();

        /// <summary>
        /// Updates all variables with the data written within the dialog node UI elements.
        /// </summary>
        public abstract void UpdateValues();
        
        /// <summary>
        /// Updates the label which informs the user which option of the pervious node the current is.
        /// </summary>
        /// <param name="optionNumber"></param>
        public void UpdateInputConnectionLabel(int optionNumber)
        {
            _optionNumberOfSelf.text = "Is Option: " + optionNumber;
        }

        /// <summary>
        /// Reduces the current <paramref name="optionNumber"/> by one. The usecase is the deletion of other nodes.
        /// </summary>
        /// <param name="optionNumber"></param>
        public void UpdateHigherInputConnectionLabel(int optionNumber)
        {
            int oldID = int.Parse(_optionNumberOfSelf.text.Replace("Is Option: ", ""));

            if (oldID > optionNumber)
            {
                oldID--;
            }

            _optionNumberOfSelf.text = "Is Option: " + oldID;
        }

        protected void ValueChange(ChangeEvent<UnityEngine.Object> e)
        {
            if (e.newValue != null)
            {
                string path = AssetDatabase.GetAssetPath(e.newValue);
                if (!path.Contains("Resources/"))
                {
                    _avatarObjectField.value = e.previousValue;
                    Debug.LogError("The dialog node avatar cannot be set to values external to the resource folder.");
                }
            }
        }

        protected class ConnectionListener : IEdgeConnectorListener
        {
            public void OnDrop(GraphView graphView, Edge edge)
            {
                ((DialogNodeBase)edge.output.node).UpdateValues();
                ((DialogNodeBase)edge.input.node).UpdateInputConnectionLabel(((DialogNodeBase)edge.output.node).OutputIDs.Count - 1);
            }

            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
            }
        }
    }
}

