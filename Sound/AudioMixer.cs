using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// A audio mixer which plays audio clips and with optional defined volume settings. See <see cref="AudioVolumeSetter"/>.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioMixer : MonoBehaviour
{
    [SerializeField] private bool _isPassive;
    public bool IsPassive
    {
        get { return _isPassive; }
        set { _isPassive = value; }
    }

    [SerializeField] private bool _fadeIn;
    [SerializeField] private bool _fadeOut;

    [SerializeField] private float _minDelayBetweenSounds;
    [SerializeField] private float _maxDelayBetweenSounds;

    [SerializeField] private List<AudioMixerItem> _items;

    private bool _internalPause;

    private float _totalProbability;

    private List<Coroutine> _coroutines;

    private float _timer;

    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _coroutines = new List<Coroutine>();
        _audioSource = GetComponent<AudioSource>();
        for (int i = 0; i < _items.Count; i++)
        {
            AudioMixerItem item = _items[i];
            _totalProbability += item.Probability;
            item.CountedProbability = _totalProbability;
            _items[i] = item;
        }

        _items.Sort(
            delegate(AudioMixerItem one, AudioMixerItem two)
                {
                    return one.CountedProbability.CompareTo(two.CountedProbability);
                }
            );
    }

    private void Update()
    {
        FadeEnd();
        if (_isPassive)
        {
            return;
        }

        if(_timer <= 0 && !_audioSource.isPlaying && !_internalPause)
        {
            PlayRandomSource();

            _timer = UnityEngine.Random.Range(_minDelayBetweenSounds, _maxDelayBetweenSounds);
        }
        else if(!_audioSource.isPlaying && !_internalPause)
        {
            _timer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Plays a random sound from the defined list.
    /// </summary>
    public void PlayRandomSource()
    {
        _internalPause = false;
        float randomSelected = UnityEngine.Random.Range(0, _totalProbability);
        AudioMixerItem item = _items.Where(a => a.CountedProbability > randomSelected).FirstOrDefault();
        if (item != null && item.Source != null)
        {
            _audioSource.clip = item.Source;
            if (_fadeIn)
            {
                FadeResume();
            }
            else
            {
                _audioSource.Play();
            }
        }
    }

    private void FadeEnd()
    {
        if(_audioSource.clip == null)
        {
            return;
        }

        if (_audioSource.clip.length - _audioSource.time < 3)
        {
            foreach (Coroutine c in _coroutines)
            {
                StopCoroutine(c);
            }
            _coroutines.Add(StartCoroutine(FadeOut()));
        }
    }

    public void FadePause()
    {
        _internalPause = true;
        foreach (Coroutine c in _coroutines)
        {
            StopCoroutine(c);
        }
        _coroutines.Add(StartCoroutine(FadeOutPause()));
    }

    public void FadeResume()
    {
        _audioSource.volume = 0;
        _internalPause = false;
        float value = PlayerPrefs.HasKey(ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.MUSIC_VOLUME)) ? PlayerPrefs.GetFloat(ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.MUSIC_VOLUME)) : 0.5f;
        foreach (Coroutine c in _coroutines)
        {
            StopCoroutine(c);
        }
        _coroutines.Add(StartCoroutine(FadeIn(value)));
    }

    IEnumerator FadeIn(float value)
    {
        _audioSource.Play();
        while (_audioSource.volume < value-0.1f)
        {
            _audioSource.volume = _audioSource.volume + 0.1f * Time.deltaTime;
            yield return null;
        }
        _audioSource.volume = value;
    }

    IEnumerator FadeOutPause()
    {
        while(_audioSource.volume != 0)
        {
            _audioSource.volume = _audioSource.volume - 0.1f * Time.deltaTime;
            yield return null;
        }
        _audioSource.Pause();
    }

    IEnumerator FadeOut()
    {
        float time = 0;
        float step = _audioSource.volume / Time.deltaTime;
        while (time < 3)
        {
            time += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(_audioSource.volume, 0, time / 3);
            yield return null;
        }
    }
}

[Serializable]
public class AudioMixerItem
{
    public AudioClip Source;
    [Range(0.1f,1)] public float Probability = 1;
    [HideInInspector] public float CountedProbability;
}
