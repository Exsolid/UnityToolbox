using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(AudioSource))]
public class AudioMixer : MonoBehaviour
{
    [SerializeField] private bool _isPassive;

    [SerializeField] private float _minDelayBetweenSounds;
    [SerializeField] private float _maxDelayBetweenSounds;

    [SerializeField] private List<AudioMixerItem> _items;
    private float _totalProbability;

    private float _timer;

    private AudioSource _audioSource;


    // Start is called before the first frame update
    void Start()
    {
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
        if (_isPassive)
        {
            return;
        }

        if(_timer <= 0 && !_audioSource.isPlaying)
        {
            float randomSelected = UnityEngine.Random.Range(0, _totalProbability);
            AudioMixerItem item = _items.Where(a => a.CountedProbability > randomSelected).FirstOrDefault();
            if (item.Source != null)
            {
                _audioSource.clip = item.Source;
                _audioSource.Play();
            }
            else
            {
                return;
            }

            _timer = UnityEngine.Random.Range(_minDelayBetweenSounds, _maxDelayBetweenSounds);
        }
        else if(!_audioSource.isPlaying)
        {
            _timer -= Time.deltaTime;
        }
    }

    public void PlayRandomSource()
    {
        float randomSelected = UnityEngine.Random.Range(0, _totalProbability);
        AudioMixerItem item = _items.Where(a => a.CountedProbability > randomSelected).FirstOrDefault();
        if (item.Source != null)
        {
            _audioSource.clip = item.Source;
            _audioSource.Play();
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
