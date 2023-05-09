using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ToggleMenuButton : MonoBehaviour, IPointerClickHandler
{
    private bool _isEnabled;
    [SerializeField] private AudioMixer _clickSounds;
    [SerializeField] [DropDown(nameof(_menuTypes))] private int _menuType;
    [SerializeField] [DropDown(nameof(_menusOfType))] private int _menuOfType;
    private List<string> _menuTypes;
    private List<string> _menusOfType;

    public void Awake()
    {
        Menu menu = GetComponentInParent<Menu>();
        menu.OnActiveChanged += (isActive) => 
        {
            _isEnabled = isActive; 
        };
    }

    public void OnPointerClick(PointerEventData data)
    {
        if (!_isEnabled)
        {
            return;
        }

        if (_clickSounds != null)
        {
            _clickSounds.PlayRandomSource();
        }

        ModuleManager.GetModule<MenuManager>().ToggleMenu(_menuType, _menuOfType);
    }

    private void OnValidate()
    {
        _menuTypes = MenuManager.MenuTypeNamesForEditor;
        _menusOfType = MenuManager.GetAllMenusOfType(_menuType);
    }
}
