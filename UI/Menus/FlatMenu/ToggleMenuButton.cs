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
    private List<string> _menuTypes;

    public void Awake()
    {
        GetComponentInParent<Menu>().OnActiveChanged += (isActive) => 
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

        ModuleManager.GetModule<MenuManager>().ToggleMenu(_menuType, true);
    }

    private void OnValidate()
    {
        _menuTypes = MenuManager.MenuTypesForEditor;
    }
}
