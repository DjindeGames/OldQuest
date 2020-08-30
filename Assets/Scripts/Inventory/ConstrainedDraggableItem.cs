using UnityEngine;

public abstract class ConstrainedDraggableItem : MonoBehaviour
{
    public bool Grabbed { get; protected set; } = false;
    protected Rigidbody rb;
    
    //Override this !!
    protected float errosionDistance = 0.2f;
    protected float mouseDistance = 2f;
    protected Vector3 containerCenter = Vector3.zero;
    protected Vector3 containerBounds = Vector3.zero;

    protected float upperZ, lowerZ, upperX, lowerX;
    protected Vector3 lastPosition;
    protected bool locked = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
    }

    protected virtual void Update()
    {
        if (Grabbed && Input.GetMouseButtonUp(0))
        {
            OnItemReleased();
        }
        //Debug.DrawLine(new Vector3(transform.position.x - lowerX, transform.position.y + 1, transform.position.z), new Vector3(transform.position.x + upperX, transform.position.y + 1, transform.position.z), Color.green, Time.deltaTime, false);
    }

    protected virtual void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && !Grabbed && !locked)
        {
            OnItemGrabbed();
        }
    }

    protected virtual void OnMouseEnter()
    {
    }

    protected virtual void OnMouseExit()
    {
    }

    protected virtual void Start()
    {
        upperZ = containerCenter.z + (containerBounds.z / 2) - errosionDistance;
        lowerZ = containerCenter.z - (containerBounds.z / 2) + errosionDistance;
        upperX = containerCenter.x + (containerBounds.x / 2) - errosionDistance;
        lowerX = containerCenter.x - (containerBounds.x / 2) + errosionDistance;
    }

    protected virtual void FixedUpdate()
    {
    }

    protected virtual void OnMouseDrag()
    {
        if (locked)
        {
            return;
        }
        rb.isKinematic = true;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mouseDistance));
        //If the mouse is out of the bounds
        if (((mousePos.z > upperZ) || (mousePos.z < lowerZ)) && ((mousePos.x > upperX) || (mousePos.x < lowerX)))
        {
            transform.position = lastPosition;
        }
        else if ((mousePos.z > upperZ) || (mousePos.z < lowerZ))
        {
            transform.position = new Vector3(mousePos.x, mousePos.y, lastPosition.z);
        }
        else if ((mousePos.x > upperX) || (mousePos.x < lowerX))
        {
            transform.position = new Vector3(lastPosition.x, mousePos.y, mousePos.z);
        }
        else
        {
            //The item follows the mouse
            transform.position = mousePos;
        }
        lastPosition = transform.position;
    }

    protected virtual void OnItemReleased()
    {
        Grabbed = false;
        rb.isKinematic = false;
    }

    protected virtual void OnItemGrabbed()
    {
        Grabbed = true;
    }
}
