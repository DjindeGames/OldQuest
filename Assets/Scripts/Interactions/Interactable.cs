﻿using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField]
    private ScreenType associatedScreen = ScreenType.Main;
     
    private bool active = true;


    private void Start()
    {
        ScreenManager.Instance.screenHasChanged += onScreenChanged;
    }

    protected bool closeEnough()
    {
        bool closeEnough = getInteractionDistance() < GameConstants.Instance.interactionRadius;
        if (!closeEnough)
        {
            MainUI.Instance.writeLog("That's too far...");
        }
        return closeEnough;
    }

    protected virtual void OnMouseOver()
    {
        if (!active) return;
        if (Input.GetMouseButtonDown(0))
        {
            PlayerController.Instance.cancelMove();
            if (closeEnough() && ScreenManager.Instance.ActiveScreen == associatedScreen)
            {
                activate();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            PlayerController.Instance.cancelMove();
            if (closeEnough() && ScreenManager.Instance.ActiveScreen == associatedScreen)
            {
                deactivate();
            }
        }
    }

    protected virtual float getInteractionDistance()
    {
        return Vector3.Distance(transform.position, PlayerController.Instance.interactionsOrigin.position); ;
    }

    protected virtual void OnMouseEnter()
    {
        if (!active) return;
        if (ScreenManager.Instance.ActiveScreen == associatedScreen)
        {
            mouseEntered();
        }
    }

    protected virtual void OnMouseExit()
    {
        if (!active) return;
        if (ScreenManager.Instance.ActiveScreen == associatedScreen)
        {
            mouseExited();
        }
    }

    private void onScreenChanged(ScreenType which)
    {
        if (which != associatedScreen)
        {
            active = false;
            deactivate();
        }
        else
        {
            active = true;
        }
    }

    protected virtual void activate() { }
    protected virtual void deactivate() { }
    protected virtual void mouseEntered() { }
    protected virtual void mouseExited() { }

    private void OnDestroy()
    {
        ScreenManager.Instance.screenHasChanged -= onScreenChanged;
    }
}
