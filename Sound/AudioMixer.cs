using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AudioMixer : MonoBehaviour
{
    [SerializeField] private bool _isPassive;

    [SerializeField] private float _minDelayBetweenSounds;
    [SerializeField] private float _maxDelayBetweenSounds;

    [SerializeField] private List<AudioMixerItem> _items;
    private float _totalProbability;

    private float _timer;

    private AudioSource _currentPlaying;


    // Start is called before the first frame update
    void Start()
    {
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

        if(_timer <= 0)
        {
            if (_currentPlaying == null)
            {
                float randomSelected = UnityEngine.Random.Range(0, _totalProbability);
                AudioMixerItem item = _items.Where(a => a.CountedProbability > randomSelected).FirstOrDefault();
                if (item.Source != null)
                {
                    _currentPlaying = item.Source;
                    _currentPlaying.Play();
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (_currentPlaying.isPlaying)
                {
                    _currentPlaying = null;
                    _timer = UnityEngine.Random.Range(_minDelayBetweenSounds, _maxDelayBetweenSounds);
                }
            }
        }
        else
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
            item.Source.Play();
        }
    }
}

[Serializable]
public struct AudioMixerItem
{
    public AudioSource Source;
    [Range(0.1f,1)] public float Probability;
    [HideInInspector] public float CountedProbability;
}
