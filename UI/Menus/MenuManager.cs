using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// This manager takes care of all menu related tasks. That is, sorting within a hierarchy and switching between visible canvases.
/// The menus are called with types also defined here.
/// Requires the <see cref="UIEventManager"/>.
/// </summary>
public class MenuManager : Module, ISerializationCallbackReceiver
{
    [SerializeField] private List<MenuList> _menuList;
    [SerializeField] public List<string> _menuTypes;
    public List<string> MenuTypes
    {
        get { return _menuTypes.ToList(); }
    }

    /// <summary>
    /// All menu types defined by the manager.
    /// </summary>
    public static List<string> MenuTypesForEditor = new List<string> 
    {
        "None",
        "Pause",
        "Inventory",
        "Overlay",
        "Dialog",
        "GameOver"
    };

    [DropDown(nameof(_menuTypes))] public int OverlayType;

    [SerializeField] private bool _hideOverlayOnOtherMenus;

    private bool _isPaused;
    private bool _isEnabled;
    public bool IsEnabled 
    { 
        get { return _isEnabled; } 
    }

    private Menu _currentActivMenu;
    private MenuList _overlayMenu;
    private MenuList _currentActivMenuList;
    public MenuList CurrentActiveMenuList 
    { 
        get { return _currentActivMenuList; } 
    }

    private bool _inMenu;
    public bool InMenu
    {
        get { return _inMenu; }
    }

    // Start is called before the first frame update
    void Start()
    {
        _isEnabled = true;
        foreach (var menuList in _menuList)
        {
            if (menuList.MenuTypeID.Equals(OverlayType))
            {
                _overlayMenu = menuList;
            } 
            foreach (var menu in menuList.Menus)
            {
                menu.IsActive = false;
                menu.enabled = false;
            }
        }
        if (_overlayMenu != null && _overlayMenu.Menus != null)
        {
            _currentActivMenuList = _overlayMenu;
            _currentActivMenu = _overlayMenu.Menus[0];
            _currentActivMenu.GetComponent<Canvas>().enabled = true;
            _currentActivMenu.IsActive = true;
        }
    }

    /// <summary>
    /// Sets the active menu with a given <see cref="Menu". Disables the current if there is one and ignores the hierarchy.
    /// </summary>
    /// <param name="menu"></param>
    public void SetActiveMenu(Menu menu)
    {
        _currentActivMenu.GetComponent<Canvas>().enabled = false;
        _currentActivMenu.IsActive = false;
        _currentActivMenu = menu;
        _currentActivMenu.GetComponent<Canvas>().enabled = true;
        _currentActivMenu.IsActive = true;
        _currentActivMenu.transform.SetAsLastSibling();
    }

    /// <summary>
    /// Toogles a menu by the given ID which references to a type. See <see cref="MenuManager.MenuTypesForEditor"/>.
    /// </summary>
    /// <param name="type">The ID which references to a type.</param>
    /// <param name="userInteraction">Whether the call came form a player interaction.</param>
    public void ToggleMenu(int type, bool userInteraction)
    {
        if (_currentActivMenu != null && (_currentActivMenu.MayUserToogle ^ userInteraction) || !_isEnabled) return;
        if (_currentActivMenuList != null && type.Equals(_currentActivMenuList.MenuTypeID) && _isPaused || !_isPaused || (_currentActivMenuList != null && OverlayType != 0 && OverlayType.Equals(_currentActivMenuList.MenuTypeID)))
        {
            _isPaused = !_isPaused;
            if (_isPaused)
            {
                var foundMenuByType = _menuList.Where(list => list.MenuTypeID.Equals(type));
                if (foundMenuByType.Any())
                {
                    MenuList menuList = foundMenuByType.First();

                    if (_overlayMenu != null && _overlayMenu.Menus != null)
                    {
                        if (_hideOverlayOnOtherMenus)
                        {
                            _currentActivMenu.GetComponent<Canvas>().enabled = false;
                        }
                        _currentActivMenu.gameObject.GetComponent<Menu>().IsActive = false;
                    }
                    _currentActivMenu = menuList.Menus[0];
                    _currentActivMenu.GetComponent<Canvas>().enabled = true;
                    _currentActivMenu.IsActive = true;
                    _currentActivMenuList = menuList;
                    _currentActivMenu.transform.SetAsLastSibling();
                    _inMenu = true;
                }
            }
            else
            {
                if(_currentActivMenu != null)
                {
                    if (_overlayMenu != null && _overlayMenu.Menus != null)
                    {
                        _currentActivMenu.GetComponent<Canvas>().enabled = false;
                        _currentActivMenu.IsActive = false;
                        _currentActivMenu = _overlayMenu.Menus[0];
                        _currentActivMenu.GetComponent<Canvas>().enabled = true;
                        _currentActivMenu.IsActive = true;
                        _currentActivMenuList = _overlayMenu;
                        _currentActivMenu.transform.SetAsLastSibling();
                    }
                    else
                    {
                        _currentActivMenu.GetComponent<Canvas>().enabled = false;
                        _currentActivMenu.IsActive = false;
                        _currentActivMenu = null;
                    }
                    _inMenu = false;
                }
            }
            ModuleManager.GetModule<UIEventManager>().TogglePaused(_isPaused, type);
        }
    }
    /// <summary>
    /// Toogles a menu. Disables the current if there is one.
    /// </summary>
    /// <param name="menu"></param>
    /// <param name="userInteraction">Whether the call came form a player interaction.</param>
    public void ToggleMenu(Menu menu, bool userInteraction)
    {
        if (_currentActivMenu != null && (_currentActivMenu.MayUserToogle ^ userInteraction) || !_isEnabled) return;
        if (menu.Equals(_currentActivMenu) && _isPaused || !_isPaused || (OverlayType != 0 && _overlayMenu != null && _overlayMenu.Equals(_currentActivMenu)))
        {
            _isPaused = !_isPaused;
            if (_isPaused)
            {
                if (_overlayMenu != null && _overlayMenu.Menus != null)
                {
                    if (_hideOverlayOnOtherMenus)
                    {
                        _currentActivMenu.GetComponent<Canvas>().enabled = false;
                    }
                    _currentActivMenu.gameObject.GetComponent<Menu>().IsActive = false;
                }
                _currentActivMenu = menu;
                _currentActivMenu.GetComponent<Canvas>().enabled = true;
                _currentActivMenu.IsActive = true;
                _currentActivMenuList = new MenuList();
                _currentActivMenu.transform.SetAsLastSibling();
                _inMenu = true;
            }
            else
            {
                if (_currentActivMenu != null)
                {
                    if (_overlayMenu != null && _overlayMenu.Menus != null)
                    {
                        _currentActivMenu.GetComponent<Canvas>().enabled = false;
                        _currentActivMenu.IsActive = false;
                        _currentActivMenu = _overlayMenu.Menus[0];
                        _currentActivMenu.GetComponent<Canvas>().enabled = true;
                        _currentActivMenu.IsActive = true;
                        _currentActivMenuList = _overlayMenu;
                        _currentActivMenu.transform.SetAsLastSibling();
                    }
                    else
                    {
                        _currentActivMenu.GetComponent<Canvas>().enabled = false;
                        _currentActivMenu.IsActive = false;
                        _currentActivMenu = null;
                    }
                    _inMenu = false;
                }
            }

            ModuleManager.GetModule<UIEventManager>().TogglePaused(_isPaused, -1);
        }
    }

    /// <summary>
    /// Toogles the <see cref="MenuManager"/>. Useful if UI interaction should be locked.
    /// </summary>
    public void ToggleMenuManager()
    {
        _isEnabled = !_isEnabled;
        _currentActivMenu.GetComponent<Canvas>().enabled = _isEnabled;
    }

    /// <summary>
    /// Toogles the <see cref="MenuManager"/>. Useful if UI interaction should be locked.
    /// </summary>
    /// <param name="isEnabled"></param>
    public void ToggleMenuManager(bool isEnabled)
    {
        _isEnabled = isEnabled;
        _currentActivMenu.GetComponent<Canvas>().enabled = _isEnabled;
    }

    public void OnBeforeSerialize()
    {
        _menuTypes = MenuTypesForEditor;
    }

    public void OnAfterDeserialize()
    {
        MenuTypesForEditor = _menuTypes;
    }
}

[Serializable]
public class MenuList
{
    [DropDown(nameof(MenuManager._menuTypes), true)] public int MenuTypeID;
    public List<Menu> Menus;
}
