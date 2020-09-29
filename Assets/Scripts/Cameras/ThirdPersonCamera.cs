using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject player;

    [Header("Parameters")]
    [SerializeField]
    [Range(10,20)]
    private int minZoomLevel = 10;
    [SerializeField]
    [Range(0, 10)]
    private int maxZoomLevel = 5;


    private int currentZoomLevel;
    private bool active = true;

    private void Awake()
    {
        currentZoomLevel = minZoomLevel - maxZoomLevel;
    }

    void Start()
    {
        ScreenManager.Instance.screenHasChanged += onScreenChange;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            //Follow the player
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y + currentZoomLevel, player.transform.position.z);
            //Player looks at mouse
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
            player.transform.LookAt(new Vector3(mousePos.x, player.transform.position.y, mousePos.z));
            //unzooming
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && transform.position.y < player.transform.position.y + minZoomLevel)
            {
                //Increases the y component 
                currentZoomLevel++;
                if (CameraManager.Instance.FirstPersonViewActive)
                {
                    CameraManager.Instance.toggleFirstPersonView(false);
                }
            }
            //zooming
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                if (transform.position.y > player.transform.position.y + maxZoomLevel)
                {
                    //Decreases the y component 
                    currentZoomLevel--;
                }
                else
                {
                    if (!CameraManager.Instance.FirstPersonViewActive)
                    {
                        CameraManager.Instance.toggleFirstPersonView(true);
                    }
                }
            }
        }
    }

    private void onScreenChange(EScreenType current)
    {
        if (current == EScreenType.Main)
        {
            active = true;
        }
        else
        {
            active = false;
        }
        //Debug.Log("[PlayerCamera] now active : " + active);
    }
}
