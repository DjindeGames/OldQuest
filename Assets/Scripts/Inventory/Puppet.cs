using UnityEngine;

public class Puppet : MonoBehaviour
{
    private Quaternion baseRotation;

    void Awake()
    {
        baseRotation = transform.rotation;
    }

    private void Update()
    {
        if (ScreenManager.Instance.ActiveScreen != EScreenType.Puppet)
        {
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Cursor.visible = true;
            transform.rotation = baseRotation;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Cursor.visible = false;
        }

        if (Input.GetMouseButton(0))
        {
            transform.Rotate(new Vector3(0, -Input.GetAxis("Mouse X") * GameConstants.Instance.puppetRotationSpeedMultiplier, 0));
        }
    }

    void OnMouseOver()
    {
        if (ScreenManager.Instance.ActiveScreen != EScreenType.Inventory)
        {
            return;
        }
        if(Input.GetMouseButtonDown(0))
        {
            ScreenManager.Instance.switchScreen(EScreenType.Puppet);
        }
    }
}
