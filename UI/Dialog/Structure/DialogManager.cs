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
/// Requires <see cref="SaveGameManager"/> if game states are used for the dialogs.
/// </summary>
public class DialogManager : Module
{
    public const string FILENAME = "DialogData.txt";

    private Dictionary<DialogNodeData, List<DialogNodeData>> _dialogNodes;
    private HashSet<DialogNodeData> _dialogStartNodes;

    private DialogNodeData _currentNode;

    public override void Awake()
    {
        base.Awake();

        _dialogNodes = new Dictionary<DialogNodeData, List<DialogNodeData>>();
        _dialogStartNodes = new HashSet<DialogNodeData>();

        List<DialogNodeData> nodes = ResourcesUtil.GetFileData<List<DialogNodeData>>(ProjectPrefKeys.DIALOGSAVEPATH, FILENAME);

        if (nodes != null)
        {
            foreach (DialogNodeData node in nodes)
            {
                _dialogNodes.Add(node, new List<DialogNodeData>());
                string s = System.IO.Path.ChangeExtension(node.AvatarReference, null).Split("Resources/").Last();
                node.Avatar = Resources.Load(s) as Texture2D;
                
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
        
        if (sequence.Count() == 0)
        {
            throw new System.Exception("No dialog node found for " + nameof(dialogID) + ": " + dialogID);
        }

        sequence = sequence.Where(node => node.StateForDialogIndentifier == null || node.StateForDialogIndentifier.Equals("")
            || ModuleManager.GetModule<GamestateManager>().IsStateActive(node.StateForDialogIndentifier));
        if (sequence.Count() == 1)
        {
            _currentNode = sequence.First();

            ModuleManager.GetModule<UIEventManager>().DialogNodeChanged(_currentNode);
        }else if (sequence.Count() < 1)
        {
            throw new System.Exception("Multiple dialog nodes found for " + nameof(dialogID) + ": " + dialogID + " at the same gamestate.");
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
        if (_currentNode != null && _currentNode.GamestateToComplete != null && !_currentNode.GamestateToComplete.Equals(""))
        {
            ModuleManager.GetModule<GamestateManager>().GoToNextState(_currentNode.GamestateToComplete, option);
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

        if (_currentNode != null && _currentNode.GamestateToComplete != null && !_currentNode.GamestateToComplete.Equals(""))
        {
            ModuleManager.GetModule<GamestateManager>().GoToNextState(_currentNode.GamestateToComplete, 0);
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
