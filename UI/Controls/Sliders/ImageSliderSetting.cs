using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

/// <summary>
/// An alternative to the Unity <see cref="Slider"/>. The fill area of this is a cropped instead of scaled image.
/// </summary>
public class ImageSliderSetting : MonoBehaviour
{
    [SerializeField] private AudioMixer _clickSounds;
    public enum SliderOption { Effects, Music, Ambience, Mouse_Sensitivity }

    [SerializeField] private SliderOption _option;

    [SerializeField] private ImageSliderHandle _handle;

    private string _pref;

    private bool _isEnabled;

    public void Awake()
    {
        GetComponentInParent<Menu>().OnActiveChanged += (isActive) =>
        {
            _isEnabled = isActive;
        };

        _handle.ValueUpdate += UpdateValues;
    }

    // Start is called before the first frame update
    void Start()
    {
        switch (_option)
        {
            case SliderOption.Mouse_Sensitivity:
                _pref = ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.MOUSE_SENSITIVITY);
                break;
            case SliderOption.Music:
                _pref = ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.MUSIC_VOLUME);
                break;
            case SliderOption.Effects:
                _pref = ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.EFFECTS_VOLUME);
                break;
            case SliderOption.Ambience:
                _pref = ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.AMBIENCE_VOLUME);
                break;
        }

        if (PlayerPrefs.HasKey(_pref))
        {
            _handle.SetValue(PlayerPrefs.GetFloat(_pref));
        }
        else
        {
            _handle.SetValue(0.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        _handle.Enabled = _isEnabled;
    }

    /// <summary>
    /// Updates the values set with the slider to the preferences and manager.
    /// To set up the slider, set a <see cref="EventTrigger"/> to execute this on end drag.
    /// </summary>
    public void UpdateValues(float value)
    {
        if (_clickSounds != null)
        {
            _clickSounds.PlayRandomSource();
        }

        PlayerPrefs.SetFloat(_pref, value);
        switch (_option)
        {
            case SliderOption.Mouse_Sensitivity:
                ModuleManager.GetModule<SettingsManager>().SenseValueChanged(value);
                break;
            case SliderOption.Music:
                ModuleManager.GetModule<SettingsManager>().SoundValueChanged(AudioType.Music, value);
                break;
            case SliderOption.Effects:
                ModuleManager.GetModule<SettingsManager>().SoundValueChanged(AudioType.Effects, value);
                break;
            case SliderOption.Ambience:
                ModuleManager.GetModule<SettingsManager>().SoundValueChanged(AudioType.Ambience, value);
                break;
        }
    }
}
