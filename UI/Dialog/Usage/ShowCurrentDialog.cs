using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowCurrentDialog : MonoBehaviour
{
    [SerializeField] private Text title;
    [SerializeField] private Text description;
    [SerializeField] private List<Text> options;
    private bool _isEnabled;

    // Start is called before the first frame update
    void Awake()
    {
        ModuleManager.GetModule<UIEventManager>().dialogNodeChanged += UpdateDialog;
        GetComponentInParent<Menu>().activeChanged += (isActive) => { _isEnabled = isActive; };
    }

    public void UpdateDialog(DialogNode currentNode)
    {
        title.text = "";
        description.text = "";
        for (int i = 0; i < options.Count; i++)
        {
            options[i].text = "";
        }
        if (currentNode != null)
        {
            ModuleManager.GetModule<MenuManager>().ToggleMenu(MenuType.Dialog, true);
            title.text = currentNode.title;
            description.text = currentNode.description;
            if (currentNode.options != null && currentNode.options.Count > options.Count) Debug.LogWarning("Not all options can be displayed! Missing sufficient textboxes.");
            for (int i = 0; i < options.Count; i++)
            {
                if (currentNode.options.Count > i) options[i].text = currentNode.options[i];
                else options[i].text = "";
            }
        }
        else
        {
            ModuleManager.GetModule<MenuManager>().ToggleMenu(MenuType.Dialog, false);
        }
    }

    private void OnDestroy()
    {
        ModuleManager.GetModule<UIEventManager>().dialogNodeChanged -= UpdateDialog;
    }
}
