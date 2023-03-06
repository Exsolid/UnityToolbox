using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class VolumeControl : MonoBehaviour
{
    [SerializeField] private SoundType _type;
    private List<AudioSource> _audioSources;

    // Start is called before the first frame update
    void Start()
    {
        _audioSources = GetComponents<AudioSource>().ToList();
        ModuleManager.GetModule<SettingsManager>().OnSoundValueChanged += OnSoundChanged;

        string pref = "";
        switch (_type)
        {
            case SoundType.Effects:
                pref = ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.EFFECTS_VOLUME);
                break;
            case SoundType.Music:
                pref = ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.MUSIC_VOLUME);
                break;
        }

        float value = PlayerPrefs.HasKey(pref) ? PlayerPrefs.GetFloat(pref) : 0.5f;
        foreach (AudioSource source in _audioSources)
        {
            if (source != null)
            {
                source.volume = value;
            }
        }
    }

    private void OnDestroy()
    {
        ModuleManager.GetModule<SettingsManager>().OnSoundValueChanged -= OnSoundChanged;
    }

    private void OnSoundChanged(SoundType type, float newValue)
    {
        if (type == _type)
        {
            foreach (AudioSource source in _audioSources)
            {
                if(source != null)
                {
                    source.volume = newValue;
                }
            }
        }
    }
}
