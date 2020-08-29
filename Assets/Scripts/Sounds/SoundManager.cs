using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void playCollisionSound(AudioSource source, float collisionForce)
    {
        source.volume = SettingsManager.Instance.PhysicsVolume * collisionForce * GameConstants.Instance.physicsSoundMultiplier;
        source.pitch = Mathf.Clamp(1 * collisionForce, GameConstants.Instance.minCollisionPitch, GameConstants.Instance.maxCollisionPitch);
        source.Play();
    }
}
