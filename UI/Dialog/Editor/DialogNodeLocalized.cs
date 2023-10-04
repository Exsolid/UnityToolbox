using System;
using System.Linq;
using System.Collections.Generic;
using UnityToolbox.UI.Dialog;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using UnityToolbox.UI.Localization;
using UnityToolbox.UI.Localization.Editor;

namespace UnityToolbox.UI.Dialog.Editor
{
    /// <summary>
    /// The node which is used within the dialog graph.
    /// </summary>
    public class DialogNodeLocalizzed : DialogNodeBase
    {
        private LocalizationID _dialogTitle;
        public LocalizationID DialogTitle
        {
            get { return _dialogTitle; }
            set { _dialogTitle = value; }
        }

        private List<LocalizationID> _options;
        public List<LocalizationID> Options
        {
            get { return _options; }
            set { _options = value; }
        }

        private LocalizationID _dialogText;
        public LocalizationID DialogText
        {
            get { return _dialogText; }
            set { _dialogText = value; }
        }

        private LocalizationDrawer _dialogTitelField;
        private LocalizationDrawer _dialogTextField;
        private List<LocalizationDrawer> _optionsFields;

        public DialogNodeLocalizzed(Vector2 position, int id) : base(position, id)
        {
            _options = new List<LocalizationID>();
            _optionsFields = new List<LocalizationDrawer>();
        }

        public DialogNodeLocalizzed(DialogNodeData data) : base(data)
        {
            _dialogTitle = data.TitleLocalizzed;
            _dialogText = data.TextLocalizzed;

            _options = new List<LocalizationID>();
            for (int i = data.OptionsLocalizzed.Count - 1; i >= 0; i--)
            {
                _options.Add(data.OptionsLocalizzed[i]);
            }

            _optionsFields = new List<LocalizationDrawer>();
        }

        /// <summary>
        /// Creates all relevant UI elements to display its data.
        /// </summary>
        public override void Draw()
        {
            _dialogTitelField = new LocalizationDrawer();
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

            _dialogTextField = new LocalizationDrawer();
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
            _options.Insert(0, new LocalizationID());

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

            if (_optionsFields.Count <= _options.Count)
            {
                int i = _options.Count - _optionsFields.Count;
                foreach (LocalizationDrawer field in _optionsFields)
                {
                    _options[i] = field.DrawerId;
                    i++;
                }
            }
            else
            {
                int j = 0;
                for (int i = _optionsFields.Count - _options.Count; i < _optionsFields.Count; i++)
                {
                    _options[j] = _optionsFields[i].DrawerId;
                    j++;
                }
            }

            _optionsFields.Clear();
            int optionNum = _options.Count - 1;

            foreach (LocalizationID option in _options.ToList())
            {
                VisualElement container = new VisualElement();
                container.style.flexDirection = FlexDirection.Row;
                Label optionLabel = new Label();
                optionLabel.text = "[Option: " + optionNum + "]";
                optionLabel.style.alignSelf = Align.FlexStart;

                container.Add(optionLabel);

                LocalizationDrawer drawer = new LocalizationDrawer();
                VisualElement field = drawer.CreateVisualElement(option);

                container.Add(field);

                _optionsFields.Add(drawer);
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
            foreach (LocalizationDrawer drawer in _optionsFields)
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
