using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : Module
{
    [SerializeField] private List<Canvas> menuLists;
    [SerializeField] private Canvas parent;
    private Canvas currentActiv;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var menu in menuLists)
        {
            menu.enabled = false;
        }

        ModuleManager.get<UIEventManager>().togglePauseMenu += toggleMenu;
    }

    private void OnDestroy()
    {
        ModuleManager.get<UIEventManager>().togglePauseMenu -= toggleMenu;
    }

    public void setActiveMenu(Canvas menu)
    {
        currentActiv.enabled = false;
        currentActiv = menu;
        menu.enabled = true;
        menu.transform.SetAsLastSibling();
    }

    public void toggleMenu(bool isPaused)
    {
        if (isPaused)
        {
            foreach (var menu in menuLists)
            {
                menu.enabled = false;
            }

            currentActiv = menuLists[0];
            currentActiv.enabled = true;
            currentActiv.transform.SetAsLastSibling();
        }
        else
        {
            foreach (var menu in menuLists)
            {
                menu.enabled = false;
            }
        }
    }
}
