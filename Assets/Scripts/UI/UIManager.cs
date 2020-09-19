﻿using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    public UIInstance[] switchableUis;
    [SerializeField]
    public UIInstance[] persistentUis;

    private Canvas activeUI;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        activeUI = getCanvas(UIType.Main);
        toggleSwitchableUI(UIType.Main);
        togglePersistentUI(UIType.All);
    }

    public void switchUI(UIType which)
    {
        activeUI.enabled = false;
        activeUI = getCanvas(which);
        activeUI.enabled = true;
        //Debug.Log("Switched UI to " + activatedUI.name + " " + activatedUI.enabled);
    }

    public void toggleCurrentUI(bool on)
    {
        activeUI.enabled = on;
    }

    private Canvas getCanvas(UIType which)
    {
        Canvas canvas = null;
        for (int i = 0; i < switchableUis.Length; i++)
        {
            if (switchableUis[i].type == which)
            {
                canvas = switchableUis[i].canvas;
                break;
            }
        }
        return canvas;
    }

    private void toggleSwitchableUI(UIType which)
    {
        for (int i = 0; i < switchableUis.Length; i++)
        {
            if (switchableUis[i].type == which || which == UIType.All)
            {
                switchableUis[i].canvas.enabled = true;
            }
            else
            {
                switchableUis[i].canvas.enabled = false;
            }
        }
    }

    private void togglePersistentUI(UIType which)
    {
        for (int i = 0; i < persistentUis.Length; i++)
        {
            if (persistentUis[i].type == which || which == UIType.All)
            {
                persistentUis[i].canvas.enabled = true;
            }
            else
            {
                persistentUis[i].canvas.enabled = false;
            }
        }
    }

    public void toggleUI(bool on)
    {
        if (on)
        {
            activeUI.enabled = true;
            togglePersistentUI(UIType.All);
        }
        else
        {
            toggleSwitchableUI(UIType.None);
            togglePersistentUI(UIType.None);
        }
    }
}

[System.Serializable]
public class UIInstance
{
    public UIType type;
    public Canvas canvas;
}