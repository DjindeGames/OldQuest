﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceActionManager : MonoBehaviour
{
    public static DiceActionManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DiceBoardManager.Instance.throwIsComplete += onThrowComplete;
    }

    private void OnDestroy()
    {
        DiceBoardManager.Instance.throwIsComplete += onThrowComplete;
    }

    void Update()
    {
        
    }

    public void performThrowAction(ThrowActionType type, int numberOfDices, int minimumValueNeeded)
    {
        ThrowAction action = new ThrowAction();
        action.actionType = type;
        action.actionPerformer = ThrowActionPerformer.Player;
        switch(type)
        {
            case (ThrowActionType.HealingPotion):
                action.color = DiceColor.Green;
                break;
            default:
                action.color = DiceColor.Black;
                break;
        }
        action.numberOfDices = numberOfDices;
        action.minimumValueNeeded = minimumValueNeeded;
        action.isAutomaticThrow = false;
        DiceBoardManager.Instance.StartThrowAction(action);
    }

    public void onThrowComplete(ThrowAction action)
    {

    }
}
