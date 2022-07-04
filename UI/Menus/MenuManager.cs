using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MenuManager : Module
{
    [SerializeField] private bool _hideOverlayOnOtherMenus;
    [SerializeField] private List<MenuList> _menuList;
    private bool _isPaused;
    private bool _isEnabled;
    public bool IsEnabled { get { return _isEnabled; } }
    private Menu _currentActivMenu;
    private MenuList _currentActivMenuList;
    public MenuList CurrentActiveMenuList { get { return _currentActivMenuList; } }
    private MenuList _overlayMenu;

    public Action<bool> inMenuChanged;

    // Start is called before the first frame update
    void Start()
    {
        _isEnabled = true;
        foreach (var menuList in _menuList)
        {
            if(menuList.MenuType.Equals(MenuType.Overlay)) _overlayMenu = menuList;
            foreach (var menu in menuList.Menus)
            {
                menu.IsActive = false;
                menu.enabled = false;
            }
        }
        if (_overlayMenu.Menus != null)
        {
            _currentActivMenu = _overlayMenu.Menus[0];
            _currentActivMenu.GetComponent<Canvas>().enabled = true;
            _currentActivMenu.IsActive = true;
        }
    }

    public void setActiveMenu(Menu menu)
    {
        _currentActivMenu.GetComponent<Canvas>().enabled = false;
        _currentActivMenu = menu;
        _currentActivMenu.enabled = true;
        _currentActivMenu.IsActive = true;
        _currentActivMenu.transform.SetAsLastSibling();
    }

    public void ToggleMenu(MenuType type, bool userInteraction)
    {
        if (_currentActivMenu != null && (_currentActivMenu.MayUserToogle ^ userInteraction) || !_isEnabled) return;
        if (type.Equals(_currentActivMenuList.MenuType) && _isPaused || !_isPaused)
        {
            _isPaused = !_isPaused;
            if (_isPaused)
            {
                var foundMenuByType = _menuList.Where(list => list.MenuType.Equals(type));
                if (foundMenuByType.Any())
                {
                    MenuList menuList = foundMenuByType.First();

                    if (_overlayMenu.Menus != null)
                    {
                        if(_hideOverlayOnOtherMenus)
                            _currentActivMenu.GetComponent<Canvas>().enabled = false;
                        _currentActivMenu.gameObject.GetComponent<Menu>().IsActive = false;
                    }
                    _currentActivMenu = menuList.Menus[0];
                    _currentActivMenu.GetComponent<Canvas>().enabled = true;
                    _currentActivMenu.IsActive = true;
                    _currentActivMenuList = menuList;
                    _currentActivMenu.transform.SetAsLastSibling();
                }
            }
            else
            {
                if(_currentActivMenu != null)
                {
                    if (_overlayMenu.Menus != null)
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
                    }
                }
            }
            ModuleManager.GetModule<UIEventManager>().TogglePaused(_isPaused, type);
        }
    }

    public void ToggleMenuManager()
    {
        _isEnabled = !_isEnabled;
        _currentActivMenu.GetComponent<Canvas>().enabled = _isEnabled;
    }

    public void ToggleMenuManager(bool isEnabled)
    {
        _isEnabled = isEnabled;
        _currentActivMenu.GetComponent<Canvas>().enabled = _isEnabled;
    }
}

[Serializable]
public struct MenuList
{
    public MenuType MenuType;
    public List<Menu> Menus;
}

public enum MenuType
{
    None,
    Pause,
    Inventory,
    Overlay,
    Dialog
}
