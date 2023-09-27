using System;
using System.Linq;
using System.Collections.Generic;
using UnityToolbox.UI.Dialog;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using UnityToolbox.UI.Localisation;
using UnityToolbox.UI.Localisation.Editor;

namespace UnityToolbox.UI.Dialog.Editor
{
    /// <summary>
    /// The node which is used within the dialog graph.
    /// </summary>
    public class DialogNodeLocalized : DialogNodeBase
    {
        private LocalisationID _dialogTitle;
        public LocalisationID DialogTitle
        {
            get { return _dialogTitle; }
            set { _dialogTitle = value; }
        }

        private List<LocalisationID> _options;
        public List<LocalisationID> Options
        {
            get { return _options; }
            set { _options = value; }
        }

        private LocalisationID _dialogText;
        public LocalisationID DialogText
        {
            get { return _dialogText; }
            set { _dialogText = value; }
        }

        private LocalisationDrawer _dialogTitelField;
        private LocalisationDrawer _dialogTextField;
        private List<LocalisationDrawer> _optionsFields;

        public DialogNodeLocalized(Vector2 position, int id) : base(position, id)
        {
            _options = new List<LocalisationID>();
            _optionsFields = new List<LocalisationDrawer>();
        }

        public DialogNodeLocalized(DialogNodeData data) : base(data)
        {
            _dialogTitle = data.TitleLocalized;
            _dialogText = data.TextLocalized;

            _options = new List<LocalisationID>();
            for (int i = data.OptionsLocalized.Count - 1; i >= 0; i--)
            {
                _options.Add(data.OptionsLocalized[i]);
            }

            _optionsFields = new List<LocalisationDrawer>();
        }

        /// <summary>
        /// Creates all relevant UI elements to display its data.
        /// </summary>
        public override void Draw()
        {
            _dialogTitelField = new LocalisationDrawer();
            _dialogTitelField.OnIDChanged += (id) =>
            {
                _dialogTitle = id;
            };
            VisualElement field = _dialogTitelField.CreateVisualElement(_dialogTitle);

            field.StretchToParentWidth();
            titleContainer.Insert(0, field);

            if (_dialogIdentifier != null && !_dialogIdentifier.Trim().Equals(""))
            {
                UpdateToRoot();
            }
            else
            {
                _inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(DialogNode));
                _inputPort.portName = "Previous";
                Button changeToRoot = new Button()
                {
                    text = "To Root"
                };
                changeToRoot.clicked += UpdateToRoot;
                inputContainer.Add(_inputPort);
                inputContainer.Add(changeToRoot);

                _inputPort.AddManipulator(new EdgeConnector<Edge>(new ConnectionListener()));
            }

            _outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(DialogNode));
            _outputPort.portName = "Next";
            outputContainer.Add(_outputPort);
            _outputPort.AddManipulator(new EdgeConnector<Edge>(new ConnectionListener()));

            VisualElement container = new VisualElement();

            Foldout foldout = new Foldout()
            {
                text = "Option IDs of Previous"
            };

            _optionNumberOfSelf = new Label()
            {
                text = "Is Option: "
            };

            foldout.Insert(0, _optionNumberOfSelf);
            container.Add(foldout);

            foldout = new Foldout()
            {
                text = "Avatar Texture"
            };

            _avatarObjectField = new ObjectField()
            {
                objectType = typeof(Texture2D),
                value = _avatar
            };
            _avatarObjectField.RegisterValueChangedCallback(e => ValueChange(e));

            foldout.Insert(0, _avatarObjectField);
            container.Add(foldout);

            _dialogTextField = new LocalisationDrawer();
            _dialogTextField.OnIDChanged += (id) =>
            {
                _dialogText = id;
            };

            field = _dialogTextField.CreateVisualElement(_dialogText);

            foldout = new Foldout()
            {
                text = "Dialog Text"
            };

            foldout.Insert(0, field);

            container.Add(foldout);

            _completionToSetTextField = new TextField()
            {
                label = "Gamestate To Complete:",
                value = _gamestateToComplete
            };

            foldout = new Foldout()
            {
                text = "Dialog Options",
                value = false
            };

            _optionFoldout = foldout;

            UpdateOptionFoldout();

            container.Add(foldout);
            extensionContainer.Add(_completionToSetTextField);
            extensionContainer.Add(container);

            RefreshExpandedState();
        }

        private void AddOption()
        {
            _options.Add(new LocalisationID());

            UpdateOptionFoldout();
        }

        private void RemoveOption()
        {
            if (_options.Count != 0)
            {
                _options.RemoveAt(_options.Count - 1);
            }

            UpdateOptionFoldout();
        }

        private void UpdateOptionFoldout()
        {
            _optionFoldout.Clear();

            Button addOption = new Button()
            {
                text = "+",
            };
            addOption.clicked += AddOption;

            Button removeOption = new Button()
            {
                text = "-",
            };

            removeOption.clicked += RemoveOption;

            _optionsFields.Clear();

            int i = 0;
            foreach (LocalisationID option in _options.ToList())
            {
                LocalisationDrawer drawer = new LocalisationDrawer();
                drawer.OnIDChanged += (id) =>
                {
                    _options[_options.IndexOf(option)] = id;
                };

                VisualElement field = drawer.CreateVisualElement(option);

                _optionsFields.Add(drawer);
                _optionFoldout.Insert(0, field);
                i++;
            }

            _optionFoldout.Insert(0, addOption);
            _optionFoldout.Insert(0, removeOption);
        }

        /// <summary>
        /// Updates all variables with the data written within the dialog node UI elements.
        /// </summary>
        public override void UpdateValues()
        {
            _dialogText = _dialogTextField.DrawerId;
            _dialogTitle = _dialogTitelField.DrawerId;

            _gamestateToComplete = _completionToSetTextField.value;
            if (_dialogIndentifierTextField != null)
            {
                _dialogIdentifier = _dialogIndentifierTextField.value;
                _stateForDialogIndentifier = _stateForDialogIndentifierTextField.value;
            }

            _avatar = (Texture2D)_avatarObjectField.value;

            int i = _options.Count-1;
            foreach (LocalisationDrawer drawer in _optionsFields)
            {
                _options[i] = drawer.DrawerId;
                i--;
            }

            if (_inputPort != null)
            {
                _inputIDs.Clear();
                foreach (Edge edge in _inputPort.connections)
                {
                    _inputIDs.Add(((DialogNodeBase)edge.output.node).ID);
                }
            }

            _outputIDs.Clear();
            foreach (Edge edge in _outputPort.connections)
            {
                _outputIDs.Add(((DialogNodeBase)edge.input.node).ID);
            }
        }

        private void UpdateToRoot()
        {
            if (_inputPort != null)
            {
                foreach (Edge edge in _inputPort.connections.ToList())
                {
                    ((DialogNodeBase)edge.output.node).OutputIDs.Remove(ID);
                    ((DialogNodeBase)edge.output.node).OutputPort.Disconnect(edge);
                    edge.output.node.RefreshPorts();
                    edge.parent.Remove(edge);
                }
            }

            inputContainer.Clear();
            _inputIDs.Clear();
            _dialogIndentifierTextField = new TextField()
            {
                value = _dialogIdentifier,
                label = "Dialog Identifier: "
            };

            _stateForDialogIndentifierTextField = new TextField()
            {
                value = _stateForDialogIndentifier,
                label = "State At Dialog: "

            };

            inputContainer.Add(_dialogIndentifierTextField);
            inputContainer.Add(_stateForDialogIndentifierTextField);
        }
    }
}
