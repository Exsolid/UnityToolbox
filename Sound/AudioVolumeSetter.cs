using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// This script sets the volume of a <see cref="AudioSource"/> and reads the data from the settings.
/// For more information on volume settings see <see cref="SettingsManager"/> and <see cref="UnitySilderSetting"/>.
/// Requires <see cref="PlayerPrefKeys"/> and <see cref="PlayerPrefKeys"/>.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioVolumeSetter : MonoBehaviour
{
    [SerializeField] private AudioType _type;
    private List<AudioSource> _audioSources;

    // Start is called before the first frame update
    void Start()
    {
        _audioSources = GetComponents<AudioSource>().ToList();
        ModuleManager.GetModule<SettingsManager>().OnSoundValueChanged += OnSoundChanged;

        string pref = "";
        switch (_type)
        {
            case AudioType.Effects:
                pref = ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.EFFECTS_VOLUME);
                break;
            case AudioType.Music:
                pref = ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.MUSIC_VOLUME);
                break;
            case AudioType.Ambience:
                pref = ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.AMBIENCE_VOLUME);
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

    private void OnSoundChanged(AudioType type, float newValue)
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

    public float GetSetVolume()
    {
        string pref = "";
        switch (_type)
        {
            case AudioType.Effects:
                pref = ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.EFFECTS_VOLUME);
                break;
            case AudioType.Music:
                pref = ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.MUSIC_VOLUME);
                break;
            case AudioType.Ambience:
                pref = ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.AMBIENCE_VOLUME);
                break;
        }

        return PlayerPrefs.HasKey(pref) ? PlayerPrefs.GetFloat(pref) : 0.5f;
    }
}
