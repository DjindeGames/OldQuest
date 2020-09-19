using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class ItemPhysics : Interactable
{
    [Header("Parameters")]
    [SerializeField]
    private bool safety = false;
    protected AudioSource source;

    protected bool grabbed = false;
    private bool coolDown = false;
    private bool thrown = false;
    private Rigidbody rb;
    private NavMeshObstacle obstacle;
    private Vector3 basePosition;

    private bool preventCollisionSound = true;

    private Vector3 startPosition;
    private Quaternion startRotation;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        obstacle = GetComponent<NavMeshObstacle>();
        source = GetComponent<AudioSource>();
        basePosition = transform.position;
        StartCoroutine(collisionSoundsCooldown());
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    bool OnCollisionEnter(Collision collision)
    {
        if (preventCollisionSound)
        {
            return false;
        }
        SoundManager.Instance.playCollisionSound(source, collision.relativeVelocity.magnitude);
        rb.AddForce(Vector3.Reflect(rb.velocity.normalized, collision.GetContact(0).normal) * collision.relativeVelocity.magnitude);
        if (safety && !grabbed && !coolDown)
        {
            StartCoroutine(resetPosition());
        }
        return true;
    }

    protected override void activate()
    {
        if (CameraManager.Instance.FirstPersonViewActive)
        {
            grabbed = true;
            PlayerHelper.IsGrabbing = true;
            if (obstacle)
            {
                obstacle.enabled = false;
            }
            PlayerController.Instance.togglePlayerController(false);
        }
    }

    protected override void deactivate()
    {
        if (CameraManager.Instance.FirstPersonViewActive)
        {
            grabbed = false;
            PlayerHelper.IsGrabbing = false;
            if (obstacle)
            {
                obstacle.enabled = true;
            }
            PlayerController.Instance.togglePlayerController(true);
        }
    }

    protected override void disable()
    {
        base.disable();
        deactivate();
    }

    private void Update()
    {
        if (grabbed)
        {
            rb.velocity = Vector3.zero;
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
            //Converts the pixels coordinates into world space coordinates
            transform.position = Camera.main.ScreenToWorldPoint(mousePos);
            if (Input.GetMouseButtonDown(1))
            {
                PlayerController.Instance.togglePlayerController(true);
                StartCoroutine(throwItem());
            }
        }
    }

    private IEnumerator throwItem()
    {
        deactivate();
        Vector3 direction = CameraManager.Instance.FirstPersonCamera.transform.forward;
        direction = new Vector3(direction.x, direction.y, direction.z);
        rb.AddForce(direction * GameConstants.Instance.throwForce, ForceMode.Impulse);
        rb.AddTorque(direction * GameConstants.Instance.throwTorque);
        yield return new WaitForSecondsRealtime(5f);
        if (safety)
        {
            resetPosition();
        }
    }

    private IEnumerator resetPosition()
    {
        coolDown = true;
        yield return new WaitForSecondsRealtime(3f);
        if (!PlayerController.Instance.checkPath(transform.position) && !grabbed)
        {
            restorePosition();
        }
        coolDown = false;
    }

    private IEnumerator collisionSoundsCooldown()
    {
        yield return new WaitForSecondsRealtime(1f);
        preventCollisionSound = false;
    }

    public void resetStartPosition(Vector3 newPosition)
    {
        startPosition = newPosition;
    }

    public void restorePosition()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        rb.velocity = Vector3.zero;
    }
}
