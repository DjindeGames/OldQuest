using System.Collections;
using UnityEngine;

public class Dice : ConstrainedDraggableItem
{
    [SerializeField]
    private DiceValue faceUp;

    public bool IsThrown
    {
        get
        {
            return currentThrowType == DiceThrowType.Thrown;
        }
    }

    public DiceValue Value
    {
        get
        {
            return faceUp;
        }
        set
        {
            faceUp = value;
        }
    }

    public bool IsLocked
    {
        get
        {
            return locked;
        }
    }

    private ThrowAction parentAction;

    private AudioSource source;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Material[] baseMaterials;
    private Material[] outlinedMaterials;
    private Renderer renderer;

    private Coroutine currentLockingCoroutine = null;
    private DiceThrowType currentThrowType = DiceThrowType.None;
    private DiceOutlineType currentOutline;
    private bool selected = false;
    
    protected override void Awake()
    {
        base.Awake();
        interactable = DiceBoardManager.Instance.currentPerformer == ThrowActionPerformer.Player;
        source = GetComponent<AudioSource>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        renderer = GetComponent<Renderer>();
        baseMaterials = renderer.materials;
    }

    protected override void Start()
    {
        errosionDistance = GameConstants.Instance.dicesErrosionDistance;
        mouseDistance = GameConstants.Instance.dicesMouseDistance;
        containerBounds = DiceBoardManager.Instance.TableCollider.bounds.size;
        containerCenter = DiceBoardManager.Instance.TableCollider.bounds.center;
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (selected)
        {
            toggleOutline(true, DiceOutlineType.Selected);
            constrainBounds();
        }
        else if (Value == DiceValue.Broken)
        {
            toggleOutline(true, DiceOutlineType.Broken);
        }
        else if (IsThrown)
        {
            if (parentAction!= null && (int)Value > parentAction.minimumValueNeeded)
            {
                toggleOutline(true, DiceOutlineType.Success);
            }
            else
            {
                toggleOutline(true, DiceOutlineType.Failure);
            }
        }
        else
        {
            toggleOutline(false);
        }
    }

    protected override void FixedUpdate()
    {
        Vector3 throwDirection;
        switch (currentThrowType)
        {
            case (DiceThrowType.Manual):
                currentThrowType = DiceThrowType.Thrown;
                throwDirection = new Vector3(Input.GetAxis("Mouse X"), -0.5f, Input.GetAxis("Mouse Y"));
                rb.AddForce(throwDirection * GameConstants.Instance.dicesThrowForce, ForceMode.Force);
                rb.AddTorque(Vector3.one * GameConstants.Instance.dicesThrowTorque, ForceMode.Force);
                break;
            case (DiceThrowType.Automatic):
                currentThrowType = DiceThrowType.Thrown;
                throwDirection = new Vector3(Random.Range(-1, 1), 1, Random.Range(-1, 1));
                rb.AddForce(throwDirection * GameConstants.Instance.dicesThrowForce * 2, ForceMode.Force);
                rb.AddTorque(Vector3.one * GameConstants.Instance.dicesThrowTorque, ForceMode.Force);
                break;
            default:
                break;
        }
    }

    public void setParentAction(ThrowAction action)
    {
        parentAction = action;
    }

    public void forceRelease()
    {
        OnItemReleased();
    }

    public bool isMoving()
    {
        return (Vector3.Magnitude(rb.velocity) >= GameConstants.Instance.diceMovingThreshold);
    }

    public void notifyFaceDown(DiceValue face)
    {
        Value = 7 - face;
        if (currentLockingCoroutine != null)
        {
            StopCoroutine(currentLockingCoroutine);
        }
        if (IsThrown && face != DiceValue.Broken)
        {
            currentLockingCoroutine = StartCoroutine(lockCoroutine());
        }
    }

    public void notifyFaceUp(DiceValue face)
    {
        if (Value == 7 - face)
        {
            Value = DiceValue.Broken;
        }
    }

    public void setIsGrabbed()
    {
        rb.isKinematic = true;
        Grabbed = true;
    }

    public void setSelected(bool selected)
    {
        this.selected = selected;
    }

    private void constrainBounds()
    {
        Vector3 currentPosition = transform.position;
        if (((currentPosition.z > upperZ) || (currentPosition.z < lowerZ)) && ((currentPosition.x > upperX) || (currentPosition.x < lowerX)))
        {
            transform.position = lastPosition;
        }
        else if ((currentPosition.z > upperZ) || (currentPosition.z < lowerZ))
        {
            transform.position = new Vector3(currentPosition.x, currentPosition.y, lastPosition.z);
        }
        else if ((currentPosition.x > upperX) || (currentPosition.x < lowerX))
        {
            transform.position = new Vector3(lastPosition.x, currentPosition.y, currentPosition.z);
        }
        lastPosition = transform.position;
    }

    private IEnumerator lockCoroutine()
    {
        yield return new WaitForSeconds(GameConstants.Instance.timeBeforeLock);
        locked = true;
        rb.isKinematic = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        SoundManager.Instance.playCollisionSound(source, collision.relativeVelocity.magnitude);
    }

    protected override void OnItemGrabbed()
    {
        base.OnItemGrabbed();
        if (!selected)
        {
            DiceBoardManager.Instance.resetSelection();
        }
        else
        {
            DiceBoardManager.Instance.grabSelectedDices(this);
        }
    }

    protected override void OnItemReleased()
    {
        base.OnItemReleased();
        currentThrowType = DiceThrowType.Manual;
    }

    public void randomThrow()
    {
        currentThrowType = DiceThrowType.Automatic;
    }

    public void restorePosition()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        rb.velocity = Vector3.zero;
    }

    private void toggleOutline(bool on, DiceOutlineType which = DiceOutlineType.None)
    {
        if (on && which != currentOutline)
        {
            initializeOutlinedMaterials(which);
            renderer.materials = outlinedMaterials;
            currentOutline = which;
        }
        else if (!on && currentOutline != DiceOutlineType.None) 
        {
            renderer.materials = baseMaterials;
            currentOutline = DiceOutlineType.None;
        }
    }

    private void initializeOutlinedMaterials(DiceOutlineType which)
    {
        outlinedMaterials = new Material[baseMaterials.Length];
        Material baseOutline = GameConstants.Instance.getDiceOutlineMaterialByType(which);
        for (int i = 0; i < outlinedMaterials.Length; i++)
        {
            Material material = new Material(baseOutline);
            material.color = baseMaterials[i].color;
            outlinedMaterials[i] = material;
        }
    }
}
