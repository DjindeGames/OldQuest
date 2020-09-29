using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject playerEyes;
    [SerializeField]
    private float minimumY = -60f;
    [SerializeField]
    private float maximumY = 60f;

    private Camera firstPersonCamera;
    private Transform player;
    private Vector2 rotation = Vector2.zero;

    void Awake()
    {
        firstPersonCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        player = PlayerController.Instance.gameObject.transform;
    }

    void Update()
    {
        if (firstPersonCamera.enabled && ScreenManager.Instance.ActiveScreen == EScreenType.Main)
        {
            //Player rotates with camera
            player.eulerAngles = new Vector3(player.rotation.x, firstPersonCamera.transform.eulerAngles.y, player.rotation.z);
            //Camera follows player's head
            transform.position = new Vector3(playerEyes.transform.position.x, playerEyes.transform.position.y, playerEyes.transform.position.z);
            rotation.x -= Input.GetAxis("Mouse Y") * SettingsManager.Instance.FirstPersonMouseSensitivity;
            rotation.y += Input.GetAxis("Mouse X") * SettingsManager.Instance.FirstPersonMouseSensitivity;
            rotation.x = clampAngle(rotation.x, minimumY, maximumY);
            transform.eulerAngles = (Vector2)rotation;
        }
    }

    private float clampAngle(float angle, float min, float max)
    {
        angle = angle % 360;
        if ((angle >= -360F) && (angle <= 360F))
        {
            if (angle < -360F)
            {
                angle += 360F;
            }
            if (angle > 360F)
            {
                angle -= 360F;
            }
        }
        return Mathf.Clamp(angle, min, max);
    }
}