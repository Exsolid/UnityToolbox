using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

namespace UnityToolbox.GameplayFeatures.Gamestates.Editor
{
    /// <summary>
    /// The node which is used within the gamestate graph.
    /// </summary>
    public class GamestateNode : Node
    {
        private string _gamestateName;
        public string GamestateName
        {
            get { return _gamestateName; }
            set { _gamestateName = value; }
        }

        private int _id;
        public int ID
        {
            get { return _id; }
        }

        private List<int> _inputIDs;
        public List<int> InputIDs
        {
            get { return _inputIDs.ToList(); }
        }

        private List<int> _outputIDs;
        public List<int> OutputIDs
        {
            get { return _outputIDs.ToList(); }
        }

        private Label _gamestateNameLabel;
        private TextField _gamestateNameTextField;
        private Port _inputPort;
        public Port InputPort
        {
            get { return _inputPort; }
        }

        private Port _outputPort;
        public Port OutputPort
        {
            get { return _outputPort; }
        }

        /// <summary>
        /// Creates a new node based on a <see cref="DialogNodeData"/>.
        /// </summary>
        /// <param name="data"></param>
        public GamestateNode(GamestateNodeData data)
        {
            _outputIDs = data.OutputIDs;
            _inputIDs = data.InputIDs;
            _id = data.ID;
            _gamestateName = data.Name;

            SetPosition(new Rect(data.Position.x, data.Position.y, 0, 0));
        }

        /// <summary>
        /// Creates a node based with a given <paramref name="position"/> within the graph and a given <paramref name="id"/>.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="id"></param>
        public GamestateNode(Vector2 position, int id)
        {
            _outputIDs = new List<int>();
            _inputIDs = new List<int>();
            _id = id;
            _gamestateName = "Enter Unique Name";

            SetPosition(new Rect(position, Vector2.zero));
        }

        /// <summary>
        /// Creates all relevant UI elements to display its data.
        /// </summary>
        public void Draw()
        {
            _inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(GamestateNode));
            _inputPort.portName = "Previous";
            inputContainer.Add(_inputPort);

            _outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(GamestateNode));
            _outputPort.portName = "Next";
            outputContainer.Add(_outputPort);

            _gamestateNameTextField = new TextField()
            {
                value = _gamestateName
            };
            _gamestateNameTextField.StretchToParentWidth();

            titleContainer.Add(_gamestateNameTextField);

            RefreshExpandedState();
        }

        /// <summary>
        /// Updates all variables with the data written within the dialog node UI elements.
        /// </summary>
        public void UpdateValues()
        {
            _gamestateName = _gamestateNameTextField.value;

            if (_inputPort != null)
            {
                _inputIDs.Clear();
                foreach (Edge edge in _inputPort.connections)
                {
                    _inputIDs.Add(((GamestateNode)edge.output.node).ID);
                }
            }

            _outputIDs.Clear();
            foreach (Edge edge in _outputPort.connections)
            {
                _outputIDs.Add(((GamestateNode)edge.input.node).ID);
            }
        }
    }
}