using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderSetting : MonoBehaviour
{
    public enum SliderOption { Effects, Music, Mouse_Sensitivity}

    public SliderOption _option;

    private Slider _slider;
    private float _timer;

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
                _pref = ModuleManager.GetModule<PlayerPrefKeys>().getPrefereceKey(PlayerPrefKeys.MOUSE_SENSITIVITY);
                break;
            case SliderOption.Music:
                _pref = ModuleManager.GetModule<PlayerPrefKeys>().getPrefereceKey(PlayerPrefKeys.MUSIC_VOLUME);
                break;
            case SliderOption.Effects:
                _pref = ModuleManager.GetModule<PlayerPrefKeys>().getPrefereceKey(PlayerPrefKeys.EFFECTS_VOLUME);
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

        _slider.onValueChanged.AddListener(delegate
        {
            _timer = 0.5f;
        });
    }

    // Update is called once per frame
    void Update()
    {
        _slider.interactable = _isEnabled;
        if (_timer > 0) _timer -= Time.deltaTime;
        if (_timer < 0 && _timer != -10)
        {
            PlayerPrefs.SetFloat(_pref, _slider.value / _slider.maxValue);
            UpdateValuesToManager();
             _timer = -10;
        }
    }

    private void OnDestroy()
    {
        if (_timer < 0)
        {
            PlayerPrefs.SetFloat(_pref, _slider.value / _slider.maxValue);
            UpdateValuesToManager();
        }
    }

    private void UpdateValuesToManager()
    {
        switch (_option)
        {
            case SliderOption.Mouse_Sensitivity:
                ModuleManager.GetModule<SettingsManager>().SenseValueChanged(_slider.value / _slider.maxValue);
                break;
            case SliderOption.Music:
                ModuleManager.GetModule<SettingsManager>().SoundValueChanged(SoundType.Music, _slider.value / _slider.maxValue);
                break;
            case SliderOption.Effects:
                ModuleManager.GetModule<SettingsManager>().SoundValueChanged(SoundType.Effects, _slider.value / _slider.maxValue);
                break;
        }
    }
}

