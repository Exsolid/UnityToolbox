using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Displays all relevant data on a canvas as soon as a dialog is triggered.
/// Requires <see cref="DialogManager"/> and <see cref="UIEventManager"/> to work.
/// </summary>
public class DisplayDialog : MonoBehaviour
{
    [SerializeField] private Text _title;
    [SerializeField] private Image _titleBackground;
    [SerializeField] private Text _description;
    [SerializeField] private Image _descriptionBackground;
    [SerializeField] private Image _spriteToShow;
    [SerializeField] private List<Text> _options;
    [SerializeField] private List<Image> _optionBackgrounds;
    [SerializeField] private Image _nextDialogBackground;

    [SerializeField] [DropDown(nameof(MenuTypes))] private int _menuType;
    private List<string> MenuTypes;
    // Start is called before the first frame update
    void Awake()
    {
        ModuleManager.GetModule<UIEventManager>().OnDialogNodeChanged += UpdateDialog;
    }

    /// <summary>
    /// Updates the UI elements to the information given by the <paramref name="currentNode"/>.
    /// </summary>
    /// <param name="currentNode"></param>
    public void UpdateDialog(DialogNodeData currentNode)
    {
        if(_title != null)
        {
            _title.text = "";
            _titleBackground.enabled = false;
        }

        if (_titleBackground != null)
        {
            _titleBackground.enabled = false;
        }

        if (_description != null)
        {
            _description.text = "";
        }

        if (_descriptionBackground != null)
        {
            _descriptionBackground.enabled = false;
        }

        for (int i = 0; i < _options.Count; i++)
        {
            _options[i].text = "";

            if (_optionBackgrounds[i] != null)
            {
                _optionBackgrounds[i].enabled = false;
            }
        }

        if (_nextDialogBackground != null)
        {
            _nextDialogBackground.enabled = false;
        }

        if (_spriteToShow != null)
        {
            _spriteToShow.enabled = false;
        }

        if (currentNode != null)
        {
            if(ModuleManager.GetModule<MenuManager>().CurrentActiveMenuList == null)
            {
                ModuleManager.GetModule<MenuManager>().ToggleMenu(_menuType, true);
            }

            if (_spriteToShow != null && currentNode.Avatar != null)
            {
                _spriteToShow.enabled = true;
                _spriteToShow.sprite = Sprite.Create(currentNode.Avatar, new Rect(0,0,currentNode.Avatar.width,currentNode.Avatar.height), new Vector2(0.5f, 0.5f));
            }

            if (_title != null)
            {
                _title.text = currentNode.Title;
            }

            if (_titleBackground != null)
            {
                _titleBackground.enabled = true;
            }

            if (_description != null)
            {
                _description.text = currentNode.Text;
            }

            if (_descriptionBackground != null)
            {
                _descriptionBackground.enabled = true;
            }

            if (currentNode.Options != null && currentNode.Options.Count > _options.Count)
            {
                Debug.LogWarning("Not all options can be displayed! Missing sufficient textboxes.");
            }

            for (int i = 0; i < _options.Count; i++)
            {
                if (currentNode.Options.Count > i)
                {
                    _options[i].text = currentNode.Options[i];
                    _optionBackgrounds[i].enabled = true;
                } 
            }

            if(_nextDialogBackground != null && currentNode.Options.Count == 0)
            {
                _nextDialogBackground.enabled = true;
            }
        }
        else
        {
            if (ModuleManager.GetModule<MenuManager>().CurrentActiveMenuList != null && ModuleManager.GetModule<MenuManager>().CurrentActiveMenuList.MenuTypeID == _menuType)
            {
                ModuleManager.GetModule<MenuManager>().ToggleMenu(_menuType, true);
            }
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
