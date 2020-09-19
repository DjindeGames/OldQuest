using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSync : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]
    private VolumeType type;

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
            case (VolumeType.Effects):
                source.volume = SettingsManager.Instance.EffectsVolume;
                break;
            case (VolumeType.Music):
                source.volume = SettingsManager.Instance.MusicVolume;
                break;
            case (VolumeType.Physics):
                source.volume = SettingsManager.Instance.PhysicsVolume;
                break;
        }
    }

    private void onVolumeChanged(VolumeType which, float value)
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
