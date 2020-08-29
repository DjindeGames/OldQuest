using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public float MusicVolume { get; private set; } = 0.5f;
    public float PhysicsVolume { get; private set; } = 0.5f;
    public float EffectsVolume { get; private set; } = 0.5f;
    public float FirstPersonMouseSensitivity { get; private set; } = 10f;

    public delegate void volumeChanged(VolumeType which, float value);
    public event volumeChanged volumeHasChanged;

    public static SettingsManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void setVolume(VolumeType which, float value)
    {
        switch (which)
        {
            case (VolumeType.Music):
                MusicVolume = value;
                break;
            case (VolumeType.Physics):
                PhysicsVolume = value;
                break;
            case (VolumeType.Effects):
                EffectsVolume = value;
                break;
        }
        if (volumeHasChanged != null)
        {
            volumeHasChanged(which, value);
        }
    }

    public void setFirstPersonMouseSensitivity(float sensitivity)
    {
        FirstPersonMouseSensitivity = sensitivity;
    }
}
