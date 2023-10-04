using System;
using System.Linq;
using System.Collections.Generic;
using UnityToolbox.UI.Dialog;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using UnityToolbox.UI.Localization.Editor;

namespace UnityToolbox.UI.Dialog.Editor
{
    /// <summary>
    /// The node which is used within the dialog graph.
    /// </summary>
    public class DialogNode : DialogNodeBase
    {
        private string _dialogTitle;
        public string DialogTitle
        {
            get { return _dialogTitle; }
            set { _dialogTitle = value; }
        }

        private List<string> _options;
        public List<string> Options
        {
            get { return _options; }
            set { _options = value; }
        }

        private string _dialogText;
        public string DialogText
        {
            get { return _dialogText; }
            set { _dialogText = value; }
        }

        private TextField _dialogTitelTextField;
        private TextField _dialogTextTextField;
        private List<TextField> _optionsTextFields;

        public DialogNode(Vector2 position, int id) : base(position, id)
        {
            _dialogTitle = "Default Title";
            _dialogText = "Default Text";
            _options = new List<string>();
            _optionsTextFields = new List<TextField>();
        }

        public DialogNode(DialogNodeData data) : base(data)
        {
            _dialogTitle = data.Title;
            _dialogText = data.Text;

            _options = new List<string>();
            for (int i = data.Options.Count - 1; i >= 0; i--)
            {
                _options.Add(data.Options[i]);
            }

            _optionsTextFields = new List<TextField>();
        }

        /// <summary>
        /// Creates all relevant UI elements to display its data.
        /// </summary>
        public override void Draw()
        {
            _dialogTitelTextField = new TextField()
            {
                value = _dialogTitle
            };

            _dialogTitelTextField.StretchToParentWidth();
            titleContainer.Insert(0, _dialogTitelTextField);

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

            foldout = new Foldout()
            {
                text = "Dialog Text"
            };

            _dialogTextTextField = new TextField()
            {
                value = _dialogText,
                multiline = true

            };

            foldout.Insert(0, _dialogTextTextField);

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
            _options.Insert(0,"Default Option");

            UpdateOptionFoldout();
        }

        private void RemoveOption()
        {
            if (_options.Count != 0)
            {
                _options.RemoveAt(0);
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

            if (_optionsTextFields.Count <= _options.Count)
            {
                int i = _options.Count - _optionsTextFields.Count;
                foreach (TextField field in _optionsTextFields)
                {
                    _options[i] = field.value;
                    i++;
                }
            }
            else
            {
                int j = 0;
                for (int i = _optionsTextFields.Count - _options.Count; i < _optionsTextFields.Count; i++)
                {
                    _options[j] = _optionsTextFields[i].value;
                    j++;
                }
            }

            _optionsTextFields.Clear();
            int optionNum = _options.Count-1;
            foreach (string option in _options)
            {
                VisualElement container = new VisualElement();
                container.style.flexDirection = FlexDirection.Row;
                Label optionLabel = new Label();
                optionLabel.text = "[Option: "+ optionNum + "]";
                optionLabel.style.alignSelf = Align.FlexStart;

                container.Add(optionLabel);

                TextField dialogOptionText = new TextField()
                {
                    value = option,
                    multiline = true
                };

                dialogOptionText.style.alignSelf = Align.Stretch;

                container.Add(dialogOptionText);

                _optionsTextFields.Add(dialogOptionText);
                _optionFoldout.Insert(0, container);
                optionNum--;
            }

            _optionFoldout.Insert(0, addOption);
            _optionFoldout.Insert(0, removeOption);
        }

        /// <summary>
        /// Updates all variables with the data written within the dialog node UI elements.
        /// </summary>
        public override void UpdateValues()
        {
            _gamestateToComplete = _completionToSetTextField.value;
            _dialogTitle = _dialogTitelTextField.value;
            _dialogText = _dialogTextTextField.value;
            if (_dialogIndentifierTextField != null)
            {
                _dialogIdentifier = _dialogIndentifierTextField.value;
                _stateForDialogIndentifier = _stateForDialogIndentifierTextField.value;
            }

            _avatar = (Texture2D)_avatarObjectField.value;

            _options.Clear();

            for (int i = _optionsTextFields.Count - 1; i >= 0; i--)
            {
                _options.Add(_optionsTextFields[i].value);
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
