using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

public class DialogManager : Module
{
    [SerializeField] private BaseGraph dialogGraph;
    private DialogNode currentNode;

    public void startDialog(string referenceID)
    {
        var sequence = dialogGraph.nodes.Where(node => node.GetType().Equals(typeof(DialogRootNode)) && ((DialogRootNode)node).referenceID.Equals(referenceID));
        if (sequence.Any())
        {
            currentNode = (DialogNode)sequence.First();
            ModuleManager.get<UIEventManager>().DialogNodeChanged(currentNode);
        }
        else
        {
            Debug.LogError("No dialog node found for referenceID " + referenceID);
        }
    }

    public void nextNode(int option)
    {
        if (currentNode != null && option < currentNode.GetOutputNodes().Count() && option >= 0)
        {
            currentNode = (DialogNode)currentNode.GetOutputNodes().ToList()[option];
            ModuleManager.get<UIEventManager>().DialogNodeChanged(currentNode);
        }
        else
        {
            nextNode();
        }
    }

    public void nextNode()
    {
        if (currentNode != null && currentNode.GetOutputNodes().Any())
        {
            currentNode = (DialogNode)currentNode.GetOutputNodes().ToList()[0];
            ModuleManager.get<UIEventManager>().DialogNodeChanged(currentNode);
        }
        else if (currentNode != null)
        {
            currentNode = null;
            ModuleManager.get<UIEventManager>().DialogNodeChanged(currentNode);
        }
    }
}
