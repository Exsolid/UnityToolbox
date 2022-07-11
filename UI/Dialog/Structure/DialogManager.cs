using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

public class DialogManager : Module
{
    [SerializeField] private BaseGraph _dialogGraph;
    private DialogNode _currentNode;

    public void StartDialog(string referenceID)
    {
        var sequence = _dialogGraph.nodes.Where(node => node.GetType().Equals(typeof(DialogRootNode)) && ((DialogRootNode)node).ReferenceID.Equals(referenceID));
        if (sequence.Any())
        {
            _currentNode = (DialogNode)sequence.First();
            ModuleManager.GetModule<UIEventManager>().DialogNodeChanged(_currentNode);
        }
        else
        {
            Debug.LogError("No dialog node found for referenceID " + referenceID);
        }
    }

    public void NextNode(int option)
    {
        if (_currentNode != null && _currentNode.CompletionToSet != null && !_currentNode.CompletionToSet.Equals(""))
        {
            ModuleManager.GetModule<SaveGameManager>().SetCompletionInfo(_currentNode.CompletionToSet, true);
        }
        if (_currentNode != null && option < _currentNode.GetOutputNodes().Count() && option >= 0)
        {
            _currentNode = (DialogNode)_currentNode.GetOutputNodes().ToList()[option];
            ModuleManager.GetModule<UIEventManager>().DialogNodeChanged(_currentNode);
        }
        else
        {
            NextNode();
        }
    }

    public void NextNode()
    {
        if(_currentNode != null && _currentNode.CompletionToSet != null && !_currentNode.CompletionToSet.Equals(""))
        {
            ModuleManager.GetModule<SaveGameManager>().SetCompletionInfo(_currentNode.CompletionToSet, true);
        }
        if (_currentNode != null && _currentNode.GetOutputNodes().Any())
        {
            _currentNode = (DialogNode)_currentNode.GetOutputNodes().ToList()[0];
            ModuleManager.GetModule<UIEventManager>().DialogNodeChanged(_currentNode);
        }
        else if (_currentNode != null)
        {
            _currentNode = null;
            ModuleManager.GetModule<UIEventManager>().DialogNodeChanged(_currentNode);
        }
    }
}
