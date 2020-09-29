using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSync : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]
    private EVolumeType type;

    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SettingsManager.Instance.volumeHasChanged += onVolumeChanged;
        initVolume();
    }

    private void initVolume()
    {
        switch(type)
        {
            case (EVolumeType.Effects):
                source.volume = SettingsManager.Instance.EffectsVolume;
                break;
            case (EVolumeType.Music):
                source.volume = SettingsManager.Instance.MusicVolume;
                break;
            case (EVolumeType.Physics):
                source.volume = SettingsManager.Instance.PhysicsVolume;
                break;
        }
    }

    private void onVolumeChanged(EVolumeType which, float value)
    {
        if (which == type)
        {
            source.volume = value;
        }
    }

    private void OnDestroy()
    {
        SettingsManager.Instance.volumeHasChanged -= onVolumeChanged;
    }
}
