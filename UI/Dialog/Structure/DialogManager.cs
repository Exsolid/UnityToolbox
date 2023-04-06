using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;

public class DialogManager : Module
{
    //[SerializeField] private BaseGraph _dialogGraph;
    //private DialogNodeDec _currentNode;

    private string _fullPath;
    private const string FILENAME = "/DialogData.dat";
    private JsonSerializerSettings _settings;

    private Dictionary<DialogNodeData, List<DialogNodeData>> _dialogNodes;
    private HashSet<DialogNodeData> _dialogStartNodes;

    private DialogNodeData _currentNode;

    public override void Awake()
    {
        base.Awake();

        _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        _fullPath = Application.dataPath + ProjectPrefs.GetString(ProjectPrefKeys.DIALOGSAVEPATH);
        _dialogNodes = new Dictionary<DialogNodeData, List<DialogNodeData>>();
        _dialogStartNodes = new HashSet<DialogNodeData>();

        if (File.Exists(_fullPath + FILENAME))
        {
            string data = File.ReadAllText(_fullPath + FILENAME);
            List<DialogNodeData> nodes = JsonConvert.DeserializeObject<List<DialogNodeData>>(data, _settings);
            List<int> ids = new List<int>();

            foreach (DialogNodeData node in nodes)
            {
                _dialogNodes.Add(node, new List<DialogNodeData>());

                string path = AssetDatabase.GUIDToAssetPath(node.AvatarReference);
                node.Avatar = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
                
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

    public void StartDialog(string dialogID)
    {
        IEnumerable<DialogNodeData> sequence = _dialogStartNodes.Where(node => node.DialogIndentifier.Equals(dialogID));
        if (sequence.Any())
        {
            _currentNode = sequence.First();
            ModuleManager.GetModule<UIEventManager>().DialogNodeChanged(_currentNode);
        }
        else
        {
            Debug.LogError("No dialog node found for" + nameof(dialogID) + ": " + dialogID);
        }
    }

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
        else
        {
            NextNode();
        }
    }

    public void NextNode()
    {
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
