using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowCurrentDialog : MonoBehaviour
{
    [SerializeField] private Text title;
    [SerializeField] private Text description;
    [SerializeField] private List<Text> options;

    [SerializeField] private Canvas parentCanvas;

    // Start is called before the first frame update
    void Awake()
    {
        ModuleManager.get<UIEventManager>().dialogNodeChanged += updateDialog;
    }

    public void updateDialog(DialogNode currentNode)
    {
        title.text = "";
        description.text = "";
        for (int i = 0; i < options.Count; i++)
        {
            options[i].text = "";
        }
        if (currentNode != null)
        {
            parentCanvas.enabled = true;
            title.text = currentNode.title;
            description.text = currentNode.description;
            if (currentNode.options.Count > options.Count) Debug.LogWarning("Not all options can be displayed! Missing sufficient textboxes.");
            for (int i = 0; i < options.Count; i++)
            {
                if (currentNode.options.Count > i) options[i].text = currentNode.options[i];
                else options[i].text = "";
            }
        }
        else
        {
            parentCanvas.enabled = false;
        }
    }

    private void OnDestroy()
    {
        ModuleManager.get<UIEventManager>().dialogNodeChanged -= updateDialog;
    }
}
