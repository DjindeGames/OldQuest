using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private CameraInstance[] cameras;
    [SerializeField]
    private Image firstPersonCursor;
    [SerializeField]
    private ECameraType activeCamera = ECameraType.Player;

    public static CameraManager Instance { get; private set; }

    public bool FirstPersonViewActive { get; private set; }
    public Camera FirstPersonCamera { get; private set; }

    private void Awake()
    {
        FirstPersonViewActive = false;
        FirstPersonCamera = getCamera(ECameraType.FirstPerson);
        Instance = this;
        toggleCamera(activeCamera);
        firstPersonCursor.enabled = false;
    }

    public void switchCamera(ECameraType which)
    {
        ECameraType actuallySwitched = which;
        switch (which)
        {
            case ECameraType.Player:
                if (FirstPersonViewActive)
                {
                    actuallySwitched = ECameraType.FirstPerson;
                    toggleFPCursor(true);
                }
                else
                {
                    actuallySwitched = ECameraType.Player;
                    toggleFPCursor(false);
                }
                break;
            default:
                toggleFPCursor(false);
                break;
        }
        toggleCamera(actuallySwitched);
        //Debug.Log("Switched Camera to " + activeCamera.name + " " + activeCamera.enabled);
    }

    private void toggleCamera(ECameraType which)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i].type == which)
            {
                cameras[i].cam.enabled = true;
                cameras[i].listener.enabled = true;
            }
            else
            {
                cameras[i].cam.enabled = false;
                cameras[i].listener.enabled = false;
            }
        }
    }

    private Camera getCamera(ECameraType which)
    {
        Camera cam = null;
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i].type == which)
            {
                cam = cameras[i].cam;
                break;
            }
        }
        return cam;
    }

    public void toggleFPCursor(bool on)
    {
        firstPersonCursor.enabled = on;
        Cursor.visible = !on;
        if (on)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void toggleFirstPersonView(bool on)
    {
        FirstPersonViewActive = on;
        toggleFPCursor(FirstPersonViewActive);
        if (FirstPersonViewActive)
        {
            activeCamera = ECameraType.FirstPerson;
        }
        else
        {
            activeCamera = ECameraType.Player;
        }
        toggleCamera(activeCamera);
    }
}

[System.Serializable]
public class CameraInstance
{
    public Camera cam;
    public ECameraType type;
    public AudioListener listener;
}
