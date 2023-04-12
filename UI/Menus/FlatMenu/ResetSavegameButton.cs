using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// A button which clears all saved data.
/// </summary>
public class ResetSavegameButton : MonoBehaviour, IPointerDownHandler
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

            ModuleManager.GetModule<SaveGameManager>().ResetAll();
            System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe"));
            Application.Quit();
        }
    }
}
