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
