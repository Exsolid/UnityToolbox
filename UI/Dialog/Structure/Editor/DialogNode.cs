using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

/// <summary>
/// The node which is used within the dialog graph.
/// </summary>
public class DialogNode : Node
{
    private string _dialogIndentifier;
    public string DialogIndentifier
    {
        get { return _dialogIndentifier; }
        set { _dialogIndentifier = value; }
    }

    private string _stateForDialogIndentifier;
    public string StateForDialogIndentifier
    {
        get { return _stateForDialogIndentifier; }
        set { _stateForDialogIndentifier = value; }
    }

    private string _gamestateToComplete;
    public string GamestateToComplete
    {
        get { return _gamestateToComplete; }
        set { _gamestateToComplete = value; }
    }

    private string _dialogTitel;
    public string DialogTitle
    {
        get { return _dialogTitel; }
        set { _dialogTitel = value; }
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

    private Texture2D _avatar;
    public Texture2D Avatar
    {
        get { return _avatar; }
        set { _avatar = value; }
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

    private Foldout _optionFoldout;

    private ObjectField _avatarObjectField;
    private Label _optionNumberOfSelf;
    private TextField _completionToSetTextField;
    private TextField _dialogTitelTextField;
    private TextField _dialogIndentifierTextField;
    private TextField _stateForDialogIndentifierTextField;
    private TextField _dialogTextTextField;
    private List<TextField> _optionsTextFields;
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
    public DialogNode(DialogNodeData data)
    {
        _dialogTitel = data.Title;
        _dialogText = data.Text;

        _options = new List<string>();
        for (int i = data.Options.Count - 1; i >= 0; i--)
        {
            _options.Add(data.Options[i]);
        }

        _optionsTextFields = new List<TextField>();
        _outputIDs = data.OutputIDs;
        _inputIDs = data.InputIDs;
        _id = data.ID;
        _dialogIndentifier = data.DialogIndentifier;
        _gamestateToComplete = data.GamestateToComplete;
        _stateForDialogIndentifier = data.StateForDialogIndentifier;

        _avatar = AssetDatabase.LoadAssetAtPath(data.AvatarReference, typeof(Texture2D)) as Texture2D;

        SetPosition(new Rect(data.Position.x, data.Position.y, 0, 0));
    }

    /// <summary>
    /// Creates a node based with a given <paramref name="position"/> within the graph and a given <paramref name="id"/>.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="id"></param>
    public DialogNode(Vector2 position, int id)
    {
        _dialogTitel = "Default Titel";
        _dialogText = "Default Text";
        _options = new List<string>();
        _optionsTextFields = new List<TextField>();
        _outputIDs = new List<int>();
        _inputIDs = new List<int>();
        _id = id;

        SetPosition(new Rect(position, Vector2.zero));
    }

    /// <summary>
    /// Creates all relevant UI elements to display its data.
    /// </summary>
    public void Draw()
    {
        _dialogTitelTextField = new TextField()
        {
            value = _dialogTitel
        };

        _dialogTitelTextField.StretchToParentWidth();
        titleContainer.Insert(0, _dialogTitelTextField);

        if(_dialogIndentifier != null && !_dialogIndentifier.Trim().Equals(""))
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
        _options.Add("Default Option");

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
        _optionsTextFields.Clear();
        foreach (string option in _options)
        {
            TextField dialogOptionText = new TextField()
            {
                value = option,
                multiline = true
            };

            _optionsTextFields.Add(dialogOptionText);
            _optionFoldout.Insert(0, dialogOptionText);
        }

        _optionFoldout.Insert(0, addOption);
        _optionFoldout.Insert(0, removeOption);
    }

    /// <summary>
    /// Updates all variables with the data written within the dialog node UI elements.
    /// </summary>
    public void UpdateValues()
    {
        _gamestateToComplete = _completionToSetTextField.value;
        _dialogTitel = _dialogTitelTextField.value;
        _dialogText = _dialogTextTextField.value;
        if(_dialogIndentifierTextField != null)
        {
            _dialogIndentifier = _dialogIndentifierTextField.value;
            _stateForDialogIndentifier = _stateForDialogIndentifierTextField.value;
        }

        _avatar = (Texture2D) _avatarObjectField.value;

        _options.Clear();

        for(int i = _optionsTextFields.Count-1; i >= 0; i--)
        {
            _options.Add(_optionsTextFields[i].value);
        }

        if(_inputPort != null)
        {
            _inputIDs.Clear();
            foreach (Edge edge in _inputPort.connections)
            {
                _inputIDs.Add(((DialogNode)edge.output.node).ID);
            }
        }

        _outputIDs.Clear();
        foreach (Edge edge in _outputPort.connections)
        {
            _outputIDs.Add(((DialogNode)edge.input.node).ID);
        }
    }

    private void UpdateToRoot()
    {
        inputContainer.Clear();
        _inputIDs.Clear();
        _dialogIndentifierTextField = new TextField()
        {
            value = _dialogIndentifier,
            label = "Dialog Indentifier: "
        };

        _stateForDialogIndentifierTextField = new TextField()
        {
            value = _stateForDialogIndentifier,
            label = "State At Dialog: "

        };

        inputContainer.Add(_dialogIndentifierTextField);
        inputContainer.Add(_stateForDialogIndentifierTextField);
    }

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

        if(oldID > optionNumber)
        {
            oldID--;
        }

        _optionNumberOfSelf.text = "Is Option: " + oldID;
    }

    private void ValueChange(ChangeEvent<UnityEngine.Object> e)
    {
        if(e.newValue != null)
        {
            string path = AssetDatabase.GetAssetPath(e.newValue);
            if (!path.Contains("Resources/"))
            {
                _avatarObjectField.value = e.previousValue;
                Debug.LogError("The dialog node avatar cannot be set to values external to the resource folder.");
            }
        }
    }

    private class ConnectionListener : IEdgeConnectorListener
    {
        public void OnDrop(GraphView graphView, Edge edge)
        {
            ((DialogNode)edge.output.node).UpdateValues();
            ((DialogNode)edge.input.node).UpdateInputConnectionLabel(((DialogNode)edge.output.node).OutputIDs.Count-1);
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
        }
    }
}
