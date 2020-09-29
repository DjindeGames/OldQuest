using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public float MusicVolume { get; private set; } = 0.5f;
    public float PhysicsVolume { get; private set; } = 0.5f;
    public float EffectsVolume { get; private set; } = 0.5f;
    public float FirstPersonMouseSensitivity { get; private set; } = 10f;

    public delegate void volumeChanged(EVolumeType which, float value);
    public event volumeChanged volumeHasChanged;

    public static SettingsManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void setVolume(EVolumeType which, float value)
    {
        switch (which)
        {
            case (EVolumeType.Music):
                MusicVolume = value;
                break;
            case (EVolumeType.Physics):
                PhysicsVolume = value;
                break;
            case (EVolumeType.Effects):
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
