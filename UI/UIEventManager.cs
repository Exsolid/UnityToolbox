using System;
using UnityToolbox.General.Management;
using UnityToolbox.UI.Dialog;
using UnityToolbox.UI.Localization;

namespace UnityToolbox.UI
{
    /// <summary>
    /// This module manages all UI events and is required for almost all UI related scripts in this toolbox.
    /// </summary>
    public class UIEventManager : Module
    {
        /// <summary>
        /// Called once the <see cref="MenuWheel"/> should change its current item to the previous.
        /// </summary>
        public event Action OnMenuWheelPrevious;
        /// <summary>
        /// Called once the <see cref="MenuWheel"/> should change its current item to the next.
        /// </summary>
        public event Action OnMenuWheelNext;

        /// <summary>
        /// Called once the <see cref="LanguageSetting"/> should change its current item to the next.
        /// </summary>
        public event Action OnLanguagePrevious;
        /// <summary>
        /// Called once the <see cref="LanguageSetting"/> should change its current item to the next.
        /// </summary>
        public event Action OnLanguageNext;
        /// <summary>
        /// Called once the current language is updated.
        /// </summary>
        public event Action<LocalizationLanguage> OnLanguageUpdated;

        /// <summary>
        /// Called once the game is paused by a menu. The int defines the menu type it is called from.
        /// </summary>
        public event Action<bool, int> OnTogglePaused;

        /// <summary>
        /// Called once the dialog node is changed.
        /// </summary>
        public event Action<DialogNodeData> OnDialogNodeChanged;

        /// <summary>
        /// Called to update the current binding status. Required so that only one key is bound at a time.
        /// </summary>
        public event Action<bool> OnBindingKey;

        /// <summary>
        /// Update the current <see cref="MenuWheel"/> item to the previous.
        /// </summary>
        public void MenuWheelPrevious()
        {
            OnMenuWheelPrevious?.Invoke();
        }

        /// <summary>
        /// Update the current <see cref="MenuWheel"/> item to the next.
        /// </summary>
        public void MenuWheelNext()
        {
            OnMenuWheelNext?.Invoke();
        }

        /// <summary>
        /// Update the current <see cref="LanguageSetting"/> item to the previous.
        /// </summary>
        public void LanguagePrevious()
        {
            OnLanguagePrevious?.Invoke();
        }

        /// <summary>
        /// Update the current <see cref="LanguageSetting"/> item to the next.
        /// </summary>
        public void LanguageNext()
        {
            OnLanguageNext?.Invoke();
        }

        /// <summary>
        /// Call to indicate that the language was updated.
        /// </summary>
        /// <param name="language">The new language.</param>
        public void LanguageUpdated(LocalizationLanguage language)
        {
            OnLanguageUpdated?.Invoke(language);
        }

        /// <summary>
        /// Call to indicate that the 
        /// </summary>
        /// <param name="isPaused"></param>
        /// <param name="typeID"></param>
        public void TogglePaused(bool isPaused, int typeID)
        {
            OnTogglePaused?.Invoke(isPaused, typeID);
        }

        public void DialogNodeChanged(DialogNodeData currentNode)
        {
            OnDialogNodeChanged?.Invoke(currentNode);
        }

        public void BindingKey(bool isBindingKey)
        {
            OnBindingKey?.Invoke(isBindingKey);
        }
    } 
}