using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderSetting : MonoBehaviour
{
    public enum SliderOption { Sound, Music, Mouse_Sensitivity}

    public SliderOption option;

    private Slider slider;
    private float timer;

    private string pref;
    [SerializeField] Canvas parentCanvas;
    // Start is called before the first frame update
    void Start()
    {
        switch (option)
        {
            case SliderOption.Mouse_Sensitivity:
                pref = ModuleManager.get<PlayerPrefKeys>().getPrefereceKey(PlayerPrefKeys.MOUSE_SENSITIVITY);
                break;
            case SliderOption.Music:
                pref = ModuleManager.get<PlayerPrefKeys>().getPrefereceKey(PlayerPrefKeys.MUSIC_VOLUME);
                break;
            case SliderOption.Sound:
                pref = ModuleManager.get<PlayerPrefKeys>().getPrefereceKey(PlayerPrefKeys.SOUND_VOLUME);
                break;
        }
        slider = gameObject.GetComponent<Slider>();
        if (PlayerPrefs.HasKey(pref)) slider.value = PlayerPrefs.GetFloat(pref) * slider.maxValue;
        slider.onValueChanged.AddListener(delegate
        {
            timer = 0.5f;
        });
    }

    // Update is called once per frame
    void Update()
    {
        slider.interactable = parentCanvas.enabled;
        if (timer > 0) timer -= Time.deltaTime;
        if (timer < 0 && timer != -10)
        {
            PlayerPrefs.SetFloat(pref, slider.value / slider.maxValue);
            timer = -10;
        }
    }

    private void OnDestroy()
    {
        if (timer < 0)
        {
            PlayerPrefs.SetFloat(pref, slider.value / slider.maxValue);
        }
    }
}

