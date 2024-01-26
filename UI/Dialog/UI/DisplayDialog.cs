using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityToolbox.General.Attributes;
using UnityToolbox.General.Management;
using UnityToolbox.General.Preferences;
using UnityToolbox.UI.Localization;
using UnityToolbox.UI.Menus;

namespace UnityToolbox.UI.Dialog.UI
{
    /// <summary>
    /// Displays all relevant data on a canvas as soon as a dialog is triggered.
    /// Requires <see cref="DialogManager"/> and <see cref="UIEventManager"/> to work.
    /// </summary>
    public class DisplayDialog : MonoBehaviour
    {
        private List<LocalizationLanguage> _allLanguages;
        private string _languagePref;

        [SerializeField] private Text _title;
        [SerializeField] private Image _titleBackground;
        [SerializeField] private Text _description;
        [SerializeField] private Image _descriptionBackground;
        [SerializeField] private Image _spriteToShow;
        [SerializeField] private List<Text> _options;
        [SerializeField] private List<Image> _optionBackgrounds;
        [SerializeField] private Image _nextDialogBackground;

        [SerializeField][DropDown(nameof(_menuTypes))] private int _menuType;
        [SerializeField][DropDown(nameof(_menusOfType))] private int _menuOfType;
        private List<string> _menuTypes;
        private List<string> _menusOfType;
        // Start is called before the first frame update
        void Awake()
        {
            ModuleManager.GetModule<UIEventManager>().OnDialogNodeChanged += UpdateDialog;
            _languagePref = ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.LANGUAGE);

            if (!Localizer.Instance.IsInitialized)
            {
                Localizer.Instance.Initialize();
            }

            _allLanguages = Localizer.Instance.LocalizationLanguages.ToList();
        }

        /// <summary>
        /// Updates the UI elements to the information given by the <paramref name="currentNode"/>.
        /// </summary>
        /// <param name="currentNode"></param>
        public void UpdateDialog(DialogNodeData currentNode)
        {
            if (_title != null)
            {
                _title.text = "";
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

                if (_optionBackgrounds.Count > i)
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
                int optionCount = currentNode.IsLocalizzed
                    ? (currentNode.OptionsLocalizzed == null ? 0 : currentNode.OptionsLocalizzed.Count)
                    : (currentNode.Options == null ? 0 : currentNode.Options.Count);

                if (ModuleManager.GetModule<MenuManager>().CurrentActiveMenuType.MenuTypeID != _menuType)
                {
                    ModuleManager.GetModule<MenuManager>().ToggleMenu(_menuType, _menuOfType);
                }

                if (_spriteToShow != null && currentNode.Avatar != null)
                {
                    _spriteToShow.enabled = true;
                    _spriteToShow.sprite = Sprite.Create(currentNode.Avatar, new Rect(0, 0, currentNode.Avatar.width, currentNode.Avatar.height), new Vector2(0.5f, 0.5f));
                }

                if (_title != null)
                {
                    _title.text = currentNode.IsLocalizzed
                        ? GetLocalizzedText(currentNode.TitleLocalizzed)
                        : currentNode.Title;
                }

                if (_titleBackground != null && _title != null && !_title.text.Trim().Equals(""))
                {
                    _titleBackground.enabled = true;
                }

                if (_description != null)
                {
                    _description.text = currentNode.IsLocalizzed
                        ? GetLocalizzedText(currentNode.TextLocalizzed)
                        : currentNode.Text;
                }

                if (_descriptionBackground != null)
                {
                    _descriptionBackground.enabled = true;
                }

                if (optionCount > _options.Count)
                {
                    Debug.LogWarning("Not all options can be displayed! Missing sufficient textboxes.");
                }

                for (int i = 0; i < _options.Count; i++)
                {
                    if (optionCount > i)
                    {
                        _options[i].text = currentNode.IsLocalizzed
                            ? GetLocalizzedText(currentNode.OptionsLocalizzed[i])
                            : currentNode.Options[i];

                        if (_optionBackgrounds.Count > i)
                        {
                            _optionBackgrounds[i].enabled = true;
                        }
                    }
                }

                if (_nextDialogBackground != null && currentNode.Options.Count == 0)
                {
                    _nextDialogBackground.enabled = true;
                }
            }
            else
            {
                if (ModuleManager.GetModule<MenuManager>().CurrentActiveMenuType.MenuTypeID == _menuType)
                {
                    ModuleManager.GetModule<MenuManager>().ToggleMenu(_menuType, _menuOfType);
                }
            }
        }

        private string GetLocalizzedText(LocalizationID LocalizationID)
        {
            KeyValuePair<LocalizationID, Dictionary<LocalizationLanguage, string>> temp = Localizer.Instance.LocalizationData.Where(e => e.Key.Equals(LocalizationID)).FirstOrDefault();
            string displayString = temp.Value == null ? "LocalizationID not valid!" : temp.Value[_allLanguages.ElementAt(PlayerPrefs.GetInt(_languagePref))];

            return displayString;
        }

        private void OnDestroy()
        {
            if (ModuleManager.ModuleRegistered<UIEventManager>())
            {
                ModuleManager.GetModule<UIEventManager>().OnDialogNodeChanged -= UpdateDialog;
            }
        }

        private void OnValidate()
        {
            _menuTypes = MenuManager.MenuTypeNamesForEditor;
            _menusOfType = MenuManager.GetAllMenusOfType(_menuType);
        }
    } 
}
