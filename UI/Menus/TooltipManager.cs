using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityToolbox.General.Management;
using UnityToolbox.General.Preferences;
using UnityToolbox.UI.Localization;

namespace UnityToolbox.UI.Menus
{
    /// <summary>
    /// A manager which is used to display tooltips.
    /// </summary>
    public class TooltipManager : Module
    {
        private List<LocalizationLanguage> _allLanguages;
        private string _languagePref;

        [SerializeField] private Text _tooltipText;
        [SerializeField] private List<Image> _tooltipBackgrounds;
        private Object _callContext;

        // Start is called before the first frame update
        void Start()
        {
            if (_tooltipText == null)
            {
                _tooltipText = GetComponent<Text>();
            }
            _tooltipText.text = "";

            foreach (Image image in _tooltipBackgrounds)
            {
                image.enabled = false;
            }

            _languagePref = ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.LANGUAGE);

            if (!Localizzer.Instance.IsInitialized)
            {
                Localizzer.Instance.Initialize();
            }

            _allLanguages = Localizzer.Instance.LocalizationLanguages.ToList();
        }

        /// <summary>
        /// Updates the current tooltip. A new <paramref name="text"/> can only be set if the old has been updated to empty by the same <paramref name="callContext"/>.
        /// </summary>
        /// <param name="keyToDisplay">A key to display.)</param>
        /// <param name="text">The text to display.</param>
        /// <param name="callContext">The caller which sets and is then allowed to reset the tooltip.</param>
        public void UpdateTooltip(string keyToDisplay, string text, Object callContext)
        {
            if (_tooltipText != null && text != "")
            {
                _callContext = callContext;
                if (keyToDisplay != null && !keyToDisplay.Equals(""))
                {
                    _tooltipText.text = "[" + keyToDisplay + "] " + text;
                }
                else
                {
                    _tooltipText.text = text;
                }

                foreach (Image image in _tooltipBackgrounds)
                {
                    image.enabled = true;
                }
            }
            else if (callContext.Equals(_callContext))
            {
                _callContext = null;

                foreach (Image image in _tooltipBackgrounds)
                {
                    image.enabled = false;
                }

                if (_tooltipText != null)
                {
                    _tooltipText.text = "";
                }
            }
        }

        /// <summary>
        /// Updates the current tooltip. A new <paramref name="text"/> can only be set if the old has been updated to empty by the same <paramref name="callContext"/>.
        /// </summary>
        /// <param name="keyToDisplay">A key to display.)</param>
        /// <param name="LocalizationID">The Localizzed text to display.</param>
        /// <param name="callContext">The caller which sets and is then allowed to reset the tooltip.</param>
        public void UpdateTooltip(string keyToDisplay, LocalizationID LocalizationID, Object callContext)
        {
            string text = GetLocalizzedText(LocalizationID);
            if (_tooltipText != null && text != "")
            {
                _callContext = callContext;
                if (keyToDisplay != null && !keyToDisplay.Equals(""))
                {
                    _tooltipText.text = "[" + keyToDisplay + "] " + text;
                }
                else
                {
                    _tooltipText.text = text;
                }

                foreach (Image image in _tooltipBackgrounds)
                {
                    image.enabled = true;
                }
            }
            else if (callContext.Equals(_callContext))
            {
                _callContext = null;

                foreach (Image image in _tooltipBackgrounds)
                {
                    image.enabled = false;
                }

                if (_tooltipText != null)
                {
                    _tooltipText.text = "";
                }
            }
        }

        private string GetLocalizzedText(LocalizationID LocalizationID)
        {
            KeyValuePair<LocalizationID, Dictionary<LocalizationLanguage, string>> temp = Localizzer.Instance.LocalizationData.Where(e => e.Key.Equals(LocalizationID)).FirstOrDefault();
            string displayString = temp.Value == null ? "LocalizationID not valid!" : temp.Value[_allLanguages.ElementAt(PlayerPrefs.GetInt(_languagePref))];

            return displayString;
        }
    }

}