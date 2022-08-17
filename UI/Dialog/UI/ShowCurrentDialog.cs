using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowCurrentDialog : MonoBehaviour
{
    [SerializeField] private Text _title;
    [SerializeField] private Text _description;
    [SerializeField] private List<Text> _options;

    [SerializeField] [DropDown(nameof(MenuTypes))] private int _menuType;
    private List<string> MenuTypes;
    // Start is called before the first frame update
    void Awake()
    {
        ModuleManager.GetModule<UIEventManager>().OnDialogNodeChanged += UpdateDialog;
    }

    public void UpdateDialog(DialogNode currentNode)
    {
        _title.text = "";
        _description.text = "";
        for (int i = 0; i < _options.Count; i++)
        {
            _options[i].text = "";
        }

        if (currentNode != null)
        {
            ModuleManager.GetModule<MenuManager>().ToggleMenu(_menuType, true);
            _title.text = currentNode.Title;
            _description.text = currentNode.Description;
            if (currentNode.Options != null && currentNode.Options.Count > _options.Count)
            {
                Debug.LogWarning("Not all options can be displayed! Missing sufficient textboxes.");
            }

            for (int i = 0; i < _options.Count; i++)
            {
                if (currentNode.Options.Count > i)
                {
                    _options[i].text = currentNode.Options[i];
                }
                else
                {
                    _options[i].text = "";
                } 
            }
        }
        else
        {
            ModuleManager.GetModule<MenuManager>().ToggleMenu(_menuType, false);
        }
    }

    private void OnDestroy()
    {
        if(ModuleManager.ModuleRegistered<UIEventManager>())
        {
            ModuleManager.GetModule<UIEventManager>().OnDialogNodeChanged -= UpdateDialog;
        }
    }

    private void OnValidate()
    {
        MenuTypes = MenuManager.MenuTypesForEditor;
    }
}
