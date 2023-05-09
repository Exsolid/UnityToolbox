using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;

/// <summary>
/// The <see cref="DialogManager"/> is a <see cref="Module"/> which manages the existing dialogs saved by the dialog graph.
/// It requires <see cref="DisplayDialog"/> and <see cref="UIEventManager"/> to display the current dialog node on a canavas.
/// </summary>
public class DialogManager : Module
{
    private string _fullPath;
    public const string FILENAME = "DialogData.txt";
    private JsonSerializerSettings _settings;

    private Dictionary<DialogNodeData, List<DialogNodeData>> _dialogNodes;
    private HashSet<DialogNodeData> _dialogStartNodes;

    private DialogNodeData _currentNode;

    public override void Awake()
    {
        base.Awake();

        _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        _fullPath = ProjectPrefs.GetString(ProjectPrefKeys.DIALOGSAVEPATH);
        _dialogNodes = new Dictionary<DialogNodeData, List<DialogNodeData>>();
        _dialogStartNodes = new HashSet<DialogNodeData>();

        TextAsset text = Resources.Load(_fullPath.Split("Resources/").Last() + FILENAME.Replace(".txt", "")) as TextAsset;

        if (text != null)
        {
            List<DialogNodeData> nodes = JsonConvert.DeserializeObject<List<DialogNodeData>>(text.text, _settings);
            List<int> ids = new List<int>();

            foreach (DialogNodeData node in nodes)
            {
                _dialogNodes.Add(node, new List<DialogNodeData>());

                node.Avatar = Resources.Load(node.AvatarReference.Split("Resources/").Last()) as Texture2D;
                
                if (node.DialogIndentifier != null && !node.DialogIndentifier.Trim().Equals(""))
                {
                    _dialogStartNodes.Add(node);
                }
            }

            foreach (DialogNodeData node in nodes)
            {
                foreach(int id in node.OutputIDs)
                {
                    DialogNodeData nextNode = nodes.Where(n => n.ID.Equals(id)).FirstOrDefault();
                    _dialogNodes[node].Add(nextNode);
                }
            }
        }
    }

    /// <summary>
    /// Starts a dialog with the given <paramref name="dialogID"/>.
    /// </summary>
    /// <param name="dialogID"> The ID which is set at a root node within the dialog graph.</param>
    public void StartDialog(string dialogID)
    {
        if(_currentNode != null)
        {
            return;
        }

        IEnumerable<DialogNodeData> sequence = _dialogStartNodes.Where(node => node.DialogIndentifier.Equals(dialogID));
        if (sequence.Any())
        {
            _currentNode = sequence.First();
            ModuleManager.GetModule<UIEventManager>().DialogNodeChanged(_currentNode);
        }
        else
        {
            Debug.LogError("No dialog node found for " + nameof(dialogID) + ": " + dialogID);
        }
    }

    /// <summary>
    /// Proceeds with the current dialog and selects the next node based on the <paramref name="option"/>.
    /// If the option cannot be found, the dialog will proceed with <see cref="DialogManager.NextNode()"/>.
    /// Sets the current nodes completion info as well.
    /// </summary>
    /// <param name="option">The selected option of the current dialog node.</param>
    public void NextNode(int option)
    {
        if (_currentNode != null && _currentNode.CompletionToSet != null && !_currentNode.CompletionToSet.Equals(""))
        {
            ModuleManager.GetModule<SaveGameManager>().SetCompletionInfo(_currentNode.CompletionToSet, true);
        }
        if (_currentNode != null && option < _dialogNodes[_currentNode].Count() && option >= 0)
        {
            _currentNode = _dialogNodes[_currentNode][option];
            ModuleManager.GetModule<UIEventManager>().DialogNodeChanged(_currentNode);
        }
    }

    /// <summary>
    /// Proceeds with the current dialog and selects the next linear node. If no next node exists the dialog is stopped.
    /// Sets the current nodes completion info as well.
    /// </summary>
    public void NextNode()
    {
        if(_currentNode.Options.Count > 0)
        {
            return;
        }

        if (_currentNode != null && _currentNode.CompletionToSet != null && !_currentNode.CompletionToSet.Equals(""))
        {
            ModuleManager.GetModule<SaveGameManager>().SetCompletionInfo(_currentNode.CompletionToSet, true);
        }

        if (_currentNode != null && _dialogNodes[_currentNode].Any())
        {
            _currentNode = _dialogNodes[_currentNode][0];
            ModuleManager.GetModule<UIEventManager>().DialogNodeChanged(_currentNode);
        }
        else if (_currentNode != null)
        {
            _currentNode = null;
            ModuleManager.GetModule<UIEventManager>().DialogNodeChanged(_currentNode);
        }
    }
}
