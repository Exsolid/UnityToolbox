using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MenuManager : Module
{
    [SerializeField] private List<MenuList> _menuList;
    [SerializeField] private Canvas parent;
    [SerializeField] private bool hideOverlayOnOtherMenus;
    private Canvas currentActiv;
    private MenuList overlayMenu;

    public Action<bool> inMenuChanged;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var menuList in _menuList)
        {
            if(menuList.MenuType.Equals(MenuType.Overlay)) overlayMenu = menuList;
            foreach (var menu in menuList.Menus)
            {
                if(menu.gameObject.GetComponent<MenuEnable>() == null) menu.gameObject.AddComponent<MenuEnable>();
                else menu.gameObject.GetComponent<MenuEnable>().IsActive = false;
                menu.enabled = false;
            }
        }
        if (overlayMenu.Menus != null)
        {
            currentActiv = overlayMenu.Menus[0];
            currentActiv.enabled = true;
            currentActiv.gameObject.GetComponent<MenuEnable>().IsActive = true;
        }
        ModuleManager.GetModule<UIEventManager>().toggleMenu += toggleMenu;
    }

    private void OnDestroy()
    {
        ModuleManager.GetModule<UIEventManager>().toggleMenu -= toggleMenu;
    }

    public void setActiveMenu(Canvas menu)
    {
        currentActiv.enabled = false;
        currentActiv = menu;
        currentActiv.enabled = true;
        currentActiv.gameObject.GetComponent<MenuEnable>().IsActive = true;
        currentActiv.transform.SetAsLastSibling();
    }

    public void toggleMenu(bool isPaused, MenuType type)
    {
        if (isPaused)
        {
            var foundMenuByType = _menuList.Where(list => list.MenuType.Equals(type));
            if (foundMenuByType.Any())
            {
                MenuList menuList = foundMenuByType.First();

                currentActiv = menuList.Menus[0];
                currentActiv.enabled = true;
                currentActiv.gameObject.GetComponent<MenuEnable>().IsActive = true;
                currentActiv.transform.SetAsLastSibling();
            }
        }
        else
        {
            if(currentActiv != null)
            {
                if (overlayMenu.Menus != null)
                {
                    currentActiv.enabled = false;
                    currentActiv.gameObject.GetComponent<MenuEnable>().IsActive = false;
                    currentActiv = overlayMenu.Menus[0];
                    currentActiv.enabled = true;
                    currentActiv.gameObject.GetComponent<MenuEnable>().IsActive = true;
                    currentActiv.transform.SetAsLastSibling();
                }
                else
                {
                    currentActiv.enabled = false;
                    currentActiv.gameObject.GetComponent<MenuEnable>().IsActive = false;
                }
            }
        }
    }
}

[Serializable]
struct MenuList
{
    public MenuType MenuType;
    public List<Canvas> Menus;
}

public enum MenuType
{
    Pause,
    Inventory,
    Overlay
}
