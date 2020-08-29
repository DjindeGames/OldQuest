using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Transform interactionsOrigin;
    [Header("Parameters")]
    [SerializeField]
    [Range(0, 10)]
    //For step sounds
    private float stepLength;
    [Header("Step Sounds")]
    [SerializeField]
    private AudioClip[] leftStepSounds;
    [SerializeField]
    private AudioClip[] rightStepSounds;
    //To know what step sound to play
    private bool rightStep;
    //Position of the transform on last frame
    private Vector3 lastPosition;
    //Increases each time the player moves, when reaches stepLength, plays a step sound
    private float distanceSinceLastStep;
    private AudioSource source;
    //To handle click and move
    private Ray pointer;
    private RaycastHit target;
    private NavMeshAgent agent;
    private NavMeshPath path;

    public static PlayerController Instance { get; private set; }


    private bool active = true;
    private bool frozen = false;

    private void Awake()
    {
        Instance = this;
        source = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        //initialize lookat
        target.point = transform.position;
        lastPosition = transform.position;
    }

    void Start()
    {
        ScreenManager.Instance.screenHasChanged += onScreenChange;
    }

    void Update()
    {
        if (active && !frozen)
        {
            agent.isStopped = false;

            //Increases distance since last step when player is moving
            distanceSinceLastStep += Vector3.Distance(transform.position, lastPosition);
            lastPosition = transform.position;

            //Plays a step sound when reaching stepLength
            if (distanceSinceLastStep >= stepLength)
            {
                distanceSinceLastStep = 0;
                playStepSound();
            }

            //The player is clicking somewhere
            if (Input.GetMouseButton(0))
            {
                //Getting click position
                pointer = Camera.main.ScreenPointToRay(Input.mousePosition);
                //Updates player destination
                if (Physics.Raycast(pointer, out target))
                {
                    if (checkPath(target.point))
                    {
                        //Instant update
                        agent.velocity = Vector3.zero;
                        agent.destination = target.point;
                    }
                }
            }
        }
        else
        {
            agent.isStopped = true;
        }
    }

    IEnumerator freezePlayer()
    {
        float delay = GameConstants.Instance.playerControllerFreezingDelay;
        frozen = true;
        yield return new WaitForSecondsRealtime(delay);
        frozen = false;
    }

    //Called by Interactable to prevent the player from moving when clicking
    public void cancelMove()
    {
        StartCoroutine(freezePlayer());
    }

    //Plays a random step sound
    private void playStepSound()
    {
        AudioClip stepSound = null;
        rightStep = !rightStep;
        if (rightStep)
        {
            stepSound = rightStepSounds[Random.Range(0, rightStepSounds.Length)];
        } else
        {
            stepSound = leftStepSounds[Random.Range(0, leftStepSounds.Length)];
        }
        source.clip = stepSound;
        source.Play();
    }

    public bool checkPath(Vector3 position)
    {
        bool pathExists = false;
        NavMeshPath tryPath = new NavMeshPath();
        agent.CalculatePath(position, tryPath);
        if (tryPath.status == NavMeshPathStatus.PathComplete)
        {
            pathExists = true;
        }
        return pathExists;
    }

    private void onScreenChange(ScreenType current)
    {
        if (current == ScreenType.Main)
        {
            active = true;
        } else
        {
            active = false;
        }
        //Debug.Log("[PlayerController] now active : " + active);
    }

    public void togglePlayerController(bool on)
    {
        active = on;
    }
}
