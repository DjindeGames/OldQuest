using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    [SerializeField]
    private GameObject linkedUI;


    private void OnEnable()
    {
        foreach(Button button in linkedUI.GetComponentsInChildren<Button>())
        {
            if (!button.transform.IsChildOf(transform))
            {
                button.interactable = false;
            }
        }
    }

    private void OnDisable()
    {
        foreach (Button button in linkedUI.GetComponentsInChildren<Button>())
        {
            if (!button.transform.IsChildOf(transform))
            {
                button.interactable = true;
            }
        }
    }
}
