using System.Collections;
using UnityEngine;

public class Door : Interactable
{
    [Header("Reference")]
    [SerializeField]
    private Transform doorHandle;
    [Header("Parameters")]
    [SerializeField]
    [Range(1,10)]
    private int rotationSpeed = 1;
    [SerializeField]
    private bool revert = true;
    [SerializeField]
    private DoorState state = DoorState.Closed;
    [SerializeField]
    private KeyMaterial material;

    [Header("Sounds")]
    [SerializeField]
    DoorStateSound[] stateSounds;

    private int rotation = 0;
    private AudioSource source;
    private bool animationInProgress = false;
    private StateSave stateSave;

    protected void Awake()
    {
        stateSave = GetComponent<StateSave>();
        source = GetComponent<AudioSource>();
        if (revert)
        {
            rotationSpeed = -rotationSpeed;
        }
        if (state == DoorState.Opened)
        {
            StartCoroutine(rotateDoor());
        }
    }

    override protected float getInteractionDistance()
    {
        return Vector3.Distance(doorHandle.position, PlayerController.Instance.interactionsOrigin.position);
    }

    override protected void activate()
    {
        if (!animationInProgress)
        {
            switch(state)
            {
                case (DoorState.Locked):
                    if (InventoryManager.Instance.hasKey(material))
                    {
                        if (stateSave)
                        {
                            SaveManager.Instance.addUnlockedDoor(stateSave.id);
                        }
                        MainUI.Instance.writeLog("Opened the door using the " + material.ToString().ToLower() + " key.");
                        state = DoorState.Opened;
                    }
                    else
                    {
                        MainUI.Instance.writeLog("The door is locked, the handle seems to be made of " + material.ToString().ToLower() + "...");
                    }
                    break;
                case (DoorState.Opened):
                    state = DoorState.Closed;
                    break;
                case (DoorState.Closed):
                    state = DoorState.Opened;
                    break;
            }
            if (state != DoorState.Locked)
            {
                if (stateSave)
                {
                    if (state == DoorState.Opened)
                    {
                        SaveManager.Instance.updateDoorState(stateSave.id, true);
                    }
                    else
                    {
                        SaveManager.Instance.updateDoorState(stateSave.id, false);
                    }
                }
                StartCoroutine(rotateDoor());
            }
            playSound();
        }
    }

    public void restoreState(bool isOpened)
    {
        if (isOpened && state != DoorState.Opened)
        {
            state = DoorState.Opened;
            StartCoroutine(rotateDoor());
        }
    }

    public void unlock()
    {
        state = DoorState.Closed;
    }

    //TO BE CHANGED
    private void playSound()
    {
        source.clip = getSoundByState(state);
        source.Play();
    }

    IEnumerator rotateDoor()
    {
        animationInProgress = true;
        rotationSpeed = -rotationSpeed;
        do
        {
            transform.Rotate(0, 0, rotationSpeed);
            rotation += rotationSpeed;
            yield return null;
        } while (Mathf.Abs(rotation) <= 90);
        rotation = 0;
        animationInProgress = false;
    }

    private AudioClip getSoundByState(DoorState state)
    {
        AudioClip clip = null;
        for (int i = 0; i < stateSounds.Length; i++)
        {
            if (stateSounds[i].state == state)
            {
                clip = stateSounds[i].sound;
                break;
            }
        }
        return clip;
    }
}

[System.Serializable]
public class DoorStateSound
{
    public DoorState state;
    public AudioClip sound;
}
