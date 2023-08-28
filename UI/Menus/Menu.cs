using System;
using UnityEngine;

namespace UnityToolbox.UI.Menus
{
    /// <summary>
    /// A script which is placed on a canvas to be recognized by the <see cref="MenuManager"/>.
    /// </summary>
    public class Menu : MonoBehaviour
    {
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnActiveChanged?.Invoke(value);
            }
        }

        [SerializeField][ReadOnly] private bool _isActive;

        public event Action<bool> OnActiveChanged;

        public void Start()
        {
            if (!ModuleManager.ModuleRegistered<MenuManager>())
            {
                IsActive = true;
            }
        }
    } 
}
