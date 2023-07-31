using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This script is placed on a slider and sets all changes for a given <see cref="SliderOption"/> to the <see cref="SettingsManager"/>.
/// </summary>
public class UnitySilderSetting : MonoBehaviour
{
    [SerializeField] private AudioMixer _clickSounds;
    public enum SliderOption { Effects, Music, Ambience, Mouse_Sensitivity}

    [SerializeField] private SliderOption _option;

    private Slider _slider;

    private string _pref; 

    private bool _isEnabled;

    public void Awake()
    {
        GetComponentInParent<Menu>().OnActiveChanged += (isActive) =>
        {
            _isEnabled = isActive;
        };
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

        _slider = gameObject.GetComponent<Slider>();
        if (PlayerPrefs.HasKey(_pref))
        {
            _slider.value = PlayerPrefs.GetFloat(_pref) * _slider.maxValue;
        }
        else
        {
            _slider.value = _slider.maxValue / 2f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _slider.interactable = _isEnabled;
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetFloat(_pref, _slider.value / _slider.maxValue);
        switch (_option)
        {
            case SliderOption.Mouse_Sensitivity:
                ModuleManager.GetModule<SettingsManager>().SenseValueChanged(_slider.value / _slider.maxValue);
                break;
            case SliderOption.Music:
                ModuleManager.GetModule<SettingsManager>().SoundValueChanged(AudioType.Music, _slider.value / _slider.maxValue);
                break;
            case SliderOption.Effects:
                ModuleManager.GetModule<SettingsManager>().SoundValueChanged(AudioType.Effects, _slider.value / _slider.maxValue);
                break;
            case SliderOption.Ambience:
                ModuleManager.GetModule<SettingsManager>().SoundValueChanged(AudioType.Ambience, _slider.value / _slider.maxValue);
                break;
        }
    }

    /// <summary>
    /// Updates the values set with the slider to the preferences and manager.
    /// To set up the slider, set a <see cref="EventTrigger"/> to execute this on end drag.
    /// </summary>
    public void UpdateValues()
    {
        if (_clickSounds != null)
        {
            _clickSounds.PlayRandomSource();
        }

        PlayerPrefs.SetFloat(_pref, _slider.value / _slider.maxValue);
        switch (_option)
        {
            case SliderOption.Mouse_Sensitivity:
                ModuleManager.GetModule<SettingsManager>().SenseValueChanged(_slider.value / _slider.maxValue);
                break;
            case SliderOption.Music:
                ModuleManager.GetModule<SettingsManager>().SoundValueChanged(AudioType.Music, _slider.value / _slider.maxValue);
                break;
            case SliderOption.Effects:
                ModuleManager.GetModule<SettingsManager>().SoundValueChanged(AudioType.Effects, _slider.value / _slider.maxValue);
                break;
            case SliderOption.Ambience:
                ModuleManager.GetModule<SettingsManager>().SoundValueChanged(AudioType.Ambience, _slider.value / _slider.maxValue);
                break;
        }
    }
}

