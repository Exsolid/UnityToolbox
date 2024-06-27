using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityToolbox.General.Attributes;
using UnityToolbox.General.Management;

namespace UnityToolbox.UI.Menus
{
    /// <summary>
    /// This manager takes care of all menu related tasks. That is, sorting within a hierarchy and switching between visible canvases.
    /// The menus are called with types also defined here.
    /// Requires the <see cref="UIEventManager"/>.
    /// </summary>
    public class MenuManager : Module, ISerializationCallbackReceiver
    {
        [SerializeField] private List<MenuType> _menuList;
        [SerializeField] private List<string> _menuTypes;

        /// <summary>
        /// All menu types defined by the manager.
        /// </summary>
        public static List<MenuType> MenuTypesForEditor = new List<MenuType>();
        public static List<string> MenuTypeNamesForEditor = new List<string>();

        [DropDown(nameof(_menuTypes))] public int OverlayType;
        [DropDown(nameof(_menuTypes))] public int InitialMenu;

        [SerializeField] private bool _hideOverlayOnOtherMenus;

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
        }

        private Menu _currentActivMenu;
        private Menu _initialMenu;
        private MenuType _overlayMenuType;
        private MenuType _currentActivMenuType;
        public MenuType CurrentActiveMenuType
        {
            get { return _currentActivMenuType; }
        }

        private const string NONETYPENAME = "None";

        // Start is called before the first frame update
        void Start()
        {
            _isEnabled = true;
            foreach (var menuList in _menuList)
            {
                if (menuList.MenuTypeID.Equals(OverlayType))
                {
                    _overlayMenuType = menuList;
                }
                if(menuList.MenuTypeID.Equals(InitialMenu))
                {
                    _initialMenu = menuList.Menus.FirstOrDefault();
                }
                foreach (Menu menu in menuList.Menus)
                {
                    menu.IsActive = false;
                    menu.enabled = false;
                }
            }

            SetActiveMenu(null);
            if (_initialMenu != null)
            {
                SetActiveMenu(_initialMenu);
            }
            else
            {
                SetActiveMenu(null);
            }
        }

        /// <summary>
        /// Sets the active menu with a given <see cref="Menu". Disables the current if there is one.
        /// Null will disable any active menu and enable the overlay if there is one set.
        /// </summary>
        /// <param name="menu"></param>
        public void SetActiveMenu(Menu menu)
        {
            if (menu == null)
            {
                menu = _overlayMenuType.Menus.FirstOrDefault();
                _currentActivMenuType = _overlayMenuType;
            }

            if (_currentActivMenu != null)
            {
                _currentActivMenu.GetComponent<Canvas>().enabled = false;
                _currentActivMenu.IsActive = false;
            }

            _currentActivMenu = menu;

            if (_currentActivMenu != null)
            {
                _currentActivMenu.GetComponent<Canvas>().enabled = true;
                _currentActivMenu.IsActive = true;
                _currentActivMenu.transform.SetAsLastSibling();
            }

        }

        /// <summary>
        /// Toggles a menu by the given ID which references to a type. See <see cref="MenuManager.MenuTypesForEditor"/>.
        /// A menu can only be activated if there is no current already active.
        /// </summary>
        /// <param name="type">The ID which references to a type.</param>
        public void ToggleMenu(int type, int menuIndex)
        {
            MenuType menuTypeToCall = _menuList.Where(menuType => menuType.MenuTypeID.Equals(type)).FirstOrDefault();
            if (menuTypeToCall == null || !(_currentActivMenuType.MenuTypeID.Equals(0) || _currentActivMenuType.MenuTypeID.Equals(_overlayMenuType.MenuTypeID)) && !menuTypeToCall.Equals(_currentActivMenuType))
            {
                return;
            }

            Menu menuToCall = menuTypeToCall.Menus[menuIndex];
            if (menuToCall == null)
            {
                return;
            }

            if (!menuToCall.Equals(_currentActivMenu))
            {
                SetActiveMenu(menuToCall);
                _currentActivMenuType = menuTypeToCall;
            }
            else
            {
                SetActiveMenu(null);
            }

            ModuleManager.GetModule<UIEventManager>().TogglePaused(_currentActivMenuType == _overlayMenuType, type);
        }

        public static List<string> GetAllMenusOfType(int type)
        {
            List<string> result = new List<string>();
            MenuType menuType = MenuTypesForEditor.Where(menuType => menuType.MenuTypeID.Equals(type)).FirstOrDefault();

            if (menuType != null)
            {
                result = menuType.Menus.Select(menu => menu.gameObject.name).ToList();
            }

            return result;
        }

        /// <summary>
        /// Toogles the <see cref="MenuManager"/>. Useful if UI interaction should be locked.
        /// </summary>
        public void ToggleMenuManager()
        {
            _isEnabled = !_isEnabled;
        }

        /// <summary>
        /// Toogles the <see cref="MenuManager"/>. Useful if UI interaction should be locked.
        /// </summary>
        /// <param name="isEnabled"></param>
        public void ToggleMenuManager(bool isEnabled)
        {
            _isEnabled = isEnabled;
        }

        public void OnBeforeSerialize()
        {
            _menuList = MenuTypesForEditor;
            _menuTypes = MenuTypeNamesForEditor;
        }

        public void OnAfterDeserialize()
        {
            MenuTypesForEditor = _menuList;
            MenuTypeNamesForEditor = _menuTypes;
        }

        private void OnValidate()
        {
            if (_menuTypes == null)
            {
                return;
            }

            if (!_menuTypes.Contains(NONETYPENAME))
            {
                _menuTypes.Insert(0, NONETYPENAME);
            }

            if (!_menuTypes[0].Equals(NONETYPENAME))
            {
                _menuTypes.Remove(NONETYPENAME);
                _menuTypes.Insert(0, NONETYPENAME);
            }

            if (!_menuList.Where(menuType => menuType.MenuTypeID.Equals(0)).Any())
            {
                _menuList.Insert(0, new MenuType()
                {
                    MenuTypeID = 0,
                    Menus = new List<Menu>()
                });
            }

            if (_menuList[0].MenuTypeID != 0)
            {
                MenuType noneType = _menuList.Where(menuType => menuType.MenuTypeID.Equals(0)).FirstOrDefault();
                _menuList.Remove(noneType);
                _menuList.Insert(0, noneType);
            }

            if (_menuList[0].Menus.Count != 0)
            {
                _menuList[0].Menus = new List<Menu>();
            }
        }
    }

    [Serializable]
    public class MenuType
    {
        [DropDown(nameof(MenuManager.MenuTypeNamesForEditor), true)] public int MenuTypeID;
        public List<Menu> Menus;
    }

}