using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [Header("AudioClips")]
    [SerializeField]
    private LootSound[] lootSounds;
    [SerializeField]
    private SoundEffect[] SFXsounds;

    public static SoundPlayer Instance { get; private set; }

    private AudioSource source;

    void Awake()
    {
        Instance = this;
        source = GetComponent<AudioSource>();
    }

    public void playSFX(SFXType type)
    {
        source.clip = getSFX(type);
        source.Play();
    }

    public void playLootSound(ItemType type)
    {
        source.clip = getLootSound(type);
        source.Play();
    }

    private AudioClip getLootSound(ItemType which)
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

    private AudioClip getSFX(SFXType which)
    {
        AudioClip clip = null;
        for (int i = 0; i < SFXsounds.Length; i++)
        {
            if (SFXsounds[i].type == which)
            {
                clip = SFXsounds[i].clip;
                break;
            }
        }
        return clip;
    }
}

[System.Serializable]
public class LootSound
{
    public ItemType type;
    public AudioClip clip;
}

[System.Serializable]
public class SoundEffect
{
    public SFXType type;
    public AudioClip clip;
}
