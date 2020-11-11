using UnityEngine;
using Djinde.Quest;

public class SoundManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject audioSourcePrefab;
    [Header("AudioClips")]
    [SerializeField]
    private LootSound[] lootSounds;
    [SerializeField]
    private SoundEffect[] SFXsounds;

    public static SoundManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void playSFX(ESFXType type)
    {
        if (type != ESFXType.None)
        {
            GameObject oneShotSource = Instantiate(audioSourcePrefab);
            AudioSource source = oneShotSource.GetComponent<AudioSource>();
            source.clip = getSFX(type);
            source.Play();
        }
    }

    public void playLootSound(EItemType type)
    {
        GameObject oneShotSource = Instantiate(audioSourcePrefab);
        AudioSource source = oneShotSource.GetComponent<AudioSource>();
        source.clip = getLootSound(type);
        source.Play();
    }

    public void playCollisionSound(AudioSource source, float collisionForce)
    {
        source.volume = SettingsManager.Instance.PhysicsVolume * collisionForce * GameConstants.Instance.physicsSoundMultiplier;
        source.pitch = Mathf.Clamp(1 * collisionForce, GameConstants.Instance.minCollisionPitch, GameConstants.Instance.maxCollisionPitch);
        source.Play();
    }

    private AudioClip getLootSound(EItemType which)
    {
        AudioClip clip = null;
        for (int i = 0; i < lootSounds.Length; i++)
        {
            if (lootSounds[i].type == which)
            {
                clip = lootSounds[i].clip;
                break;
            }
        }
        return clip;
    }

    private AudioClip getSFX(ESFXType which)
    {
        AudioClip[] clips = null;
        for (int i = 0; i < SFXsounds.Length; i++)
        {
            if (SFXsounds[i].type == which)
            {
                clips = SFXsounds[i].clips;
                break;
            }
        }
        if (clips.Length > 0)
        {
            return clips[Random.Range(0, clips.Length)];
        }
        return null;
    }
}

[System.Serializable]
public class LootSound
{
    public EItemType type;
    public AudioClip clip;
}

[System.Serializable]
public class SoundEffect
{
    public ESFXType type;
    public AudioClip[] clips;
}
