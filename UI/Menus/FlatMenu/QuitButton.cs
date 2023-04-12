using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// A button which quits the application to desktop.
/// </summary>
public class QuitButton : MonoBehaviour, IPointerDownHandler
{
    private bool _isEnabled;
    [SerializeField] private AudioMixer _clickSounds;

    public void Awake()
    {
        GetComponentInParent<Menu>().OnActiveChanged += (isActive) => 
        {
            _isEnabled = isActive;
        };
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (_isEnabled)
        {
            if (_clickSounds != null)
            {
                _clickSounds.PlayRandomSource();
            }

            Application.Quit();
        }
    }
}
