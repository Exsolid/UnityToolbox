using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MenuManager : Module
{
    [SerializeField] private List<MenuList> _menuList;
    [SerializeField] private Canvas parent;
    private Canvas currentActiv;

    public bool InMenu { get { return inMenu; } set { inMenu = value; if (inMenuChanged != null) inMenuChanged(inMenu); } }
    private bool inMenu;

    public Action<bool> inMenuChanged;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var menuList in _menuList)
        {
            foreach (var menu in menuList.Menus)
            {
                menu.enabled = false;
            }
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
        menu.enabled = true;
        menu.transform.SetAsLastSibling();
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
                currentActiv.transform.SetAsLastSibling();
            }
        }
        else
        {
            if(currentActiv != null)
            {
                currentActiv.enabled=false;
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
