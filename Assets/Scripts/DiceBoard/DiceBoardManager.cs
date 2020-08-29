﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceBoardManager : MonoBehaviour
{
    public Collider TableCollider { get; private set; }

    public delegate void throwComplete(ThrowAction action);
    public event throwComplete throwIsComplete;

    [Header("References")]
    [SerializeField]
    private ColoredDicePrefab[] dicesPrefabs;
    [SerializeField]
    private GameObject diceBoard;
    [SerializeField]
    private Transform dicesContainer;
    [Header("Parameters")]
    [SerializeField]
    private float baseVerticalInstantiationOffset = 1f;
    [SerializeField]
    private float instantiationIterativeOffset = 0.1f;
    [Header("Misc")]
    [SerializeField]
    private Image selectionBox;

    public static DiceBoardManager Instance { get; private set; }

    private List<ThrowAction> throwActions = new List<ThrowAction>();

    private List<Dice> selectedDices = new List<Dice>();

    private Vector2 selectionStart;
    private float currentSelectionWidth;
    private float currentSelectionHeight;
    private Vector2 currentSelectionCenter;
    private bool selecting = false;

    private bool isGrabbingMultipleDices = false;
    private Dice currentGrabbedDice;

    void Awake()
    {
        Instance = this;
        TableCollider = (Collider)diceBoard.GetComponents(typeof(Collider))[0];
        selectionBox.enabled = false;
        Random.InitState(System.DateTime.Now.Millisecond);
    }

    // Update is called once per frame
    void Update()
    {
        ThrowAction currentThrowAction = getCurrentThrowAction();
        if (currentThrowAction != null)
        {
            if (currentThrowAction.isActionComplete())
            {
                onThrowComplete();
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && !anyDiceIsGrabbed())
                {
                    selectionStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    setIsSelecting(true);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (isGrabbingMultipleDices)
                    {
                        throwSelectedDices();
                        resetSelection();
                    }
                    else
                    {
                        setIsSelecting(false);
                    }
                }
                if (selecting)
                {
                    updateSelectionBox();
                    selectDices();
                }
            }
        }
    }

    private void onThrowComplete()
    {
        ThrowAction completedAction = popThrowAction();
        clearThrowAction(completedAction);
        throwIsComplete(completedAction);
    }

    public void grabSelectedDices(Dice handledOne)
    {
        currentGrabbedDice = handledOne;
        isGrabbingMultipleDices = true;
        float minDicesOffset = GameConstants.Instance.minDicesOffset;
        float maxDicesOffset = GameConstants.Instance.maxDicesOffset;
        foreach (Dice dice in selectedDices)
        {
            if (dice != handledOne)
            {
                dice.setIsGrabbed();
                dice.transform.SetParent(handledOne.transform, true);
                dice.transform.localPosition = new Vector3(Random.Range(-maxDicesOffset, maxDicesOffset), Random.Range(-maxDicesOffset, maxDicesOffset), Random.Range(-maxDicesOffset, maxDicesOffset));
            }
        }
    }

    private void setIsSelecting(bool isSelecting)
    {
        selecting = isSelecting;
        selectionBox.enabled = isSelecting;
    }

    private void throwSelectedDices()
    {
        isGrabbingMultipleDices = false;
        foreach (Dice dice in selectedDices)
        {
            if (dice != currentGrabbedDice)
            {
                dice.transform.parent = dicesContainer;
                dice.forceRelease();
            }
        }
        currentGrabbedDice = null;
    }

    public void resetSelection()
    {
        foreach (Dice dice in selectedDices)
        {
            dice.setSelected(false);
        }
        selectedDices.Clear();
    }

    private bool anyDiceIsGrabbed()
    {
        foreach (Dice dice in getCurrentThrowAction().dices)
        {
            if (dice.Grabbed)
            {
                return true;
            }
        }
        return false;
    }

    private void updateSelectionBox()
    {
        Vector2 currentMousePosition = Input.mousePosition;
        currentSelectionCenter = new Vector2((selectionStart.x + currentMousePosition.x)/2, (selectionStart.y + currentMousePosition.y) / 2);
        currentSelectionWidth = Mathf.Abs(currentMousePosition.x - selectionStart.x);
        currentSelectionHeight = Mathf.Abs(currentMousePosition.y - selectionStart.y);
        selectionBox.transform.position = currentSelectionCenter;
        selectionBox.rectTransform.sizeDelta = new Vector2(currentSelectionWidth, currentSelectionHeight);
    }

    private void selectDices()
    {
        resetSelection();
        foreach (Dice dice in getCurrentThrowAction().dices)
        {
            GameObject diceObject = dice.gameObject;
            Vector2 diceScreenPosition = Camera.main.WorldToScreenPoint(diceObject.transform.position);
            if (isInSelection(diceScreenPosition) && !dice.IsLocked)
            {
                dice.setSelected(true);
                selectedDices.Add(dice);
            }
        }
    }

    private bool isInSelection(Vector2 position)
    {
        return (position.x < (currentSelectionCenter.x + (currentSelectionWidth / 2))) &&
               (position.x > (currentSelectionCenter.x - (currentSelectionWidth / 2))) &&
               (position.y < (currentSelectionCenter.y + (currentSelectionHeight / 2))) &&
               (position.y > (currentSelectionCenter.y - (currentSelectionHeight / 2)));
    }

    public void StartThrowAction(ThrowAction action)
    {
        ScreenManager.Instance.switchScreen(ScreenType.DiceBoard);
        pushThrowAction(action);
        addDices(action);
    }

    private void pushThrowAction(ThrowAction action)
    {
        ThrowAction currentAction = getCurrentThrowAction();
        if (currentAction != null)
        {
            currentAction.pauseAction();
        }
        throwActions.Add(action);
    }

    private ThrowAction popThrowAction()
    {
        ThrowAction poppedAction = getCurrentThrowAction();
        if (poppedAction != null)
        {
            throwActions.Remove(poppedAction);
            ThrowAction previousAction = getCurrentThrowAction();
            if (previousAction != null)
            {
                previousAction.resumeAction();
            }
        }
        if (throwActions.Count == 0)
        {
            ScreenManager.Instance.switchToPreviousScreen();
        }
        return poppedAction;
    }

    private ThrowAction getCurrentThrowAction()
    {
        if (throwActions.Count > 0)
        {
            return throwActions[throwActions.Count - 1];
        }
        else
        {
            return null;
        }
    }

    private void addDices(ThrowAction action)
    {
        GameObject dice = GetDicePrefab(action.color);
        Vector3 diceBoardPosition = diceBoard.transform.position;
        float cumulatedInstantiationOffset = 0f;
        for (int i = 0; i < action.numberOfDices; i++)
        {
            GameObject instantiatedDice = Instantiate(dice, dicesContainer);
            instantiatedDice.transform.position = new Vector3(diceBoardPosition.x, diceBoardPosition.y + baseVerticalInstantiationOffset + cumulatedInstantiationOffset, diceBoardPosition.z);
            action.dices.Add(instantiatedDice.GetComponent<Dice>());
            cumulatedInstantiationOffset += instantiationIterativeOffset;
        }
    }

    private void clearThrowAction(ThrowAction action)
    {
        for(int i = 0; i < action.dices.Count; i++)
        {
            Destroy(action.dices[i].gameObject);
        }
        action.dices.Clear();
    }

    public void throwAll()
    {
        foreach(Dice dice in getCurrentThrowAction().dices)
        {
            dice.randomThrow();
        }
    }

    private GameObject GetDicePrefab(DiceColor which)
    {
        GameObject dice = null;
        for (int i = 0; i < dicesPrefabs.Length; i++)
        {
            if (dicesPrefabs[i].color == which)
            {
                dice = dicesPrefabs[i].dice;
                break;
            }
        }
        return dice;
    }
}

[System.Serializable]
public class ColoredDicePrefab
{
    public DiceColor color;
    public GameObject dice;
}

public class ThrowAction
{
    public ThrowActionType actionType;
    public ThrowActionPerformer actionPerformer;
    public DiceColor color;
    public int numberOfDices;
    public int minimumValueNeeded;
    public bool isAutomaticThrow = false;
    public List<Dice> dices = new List<Dice>();

    public void pauseAction()
    {
        foreach(Dice dice in dices)
        {
            dice.gameObject.SetActive(false);
        }
    }

    public void resumeAction()
    {
        foreach (Dice dice in dices)
        {
            dice.gameObject.SetActive(true);
        }
    }

    public bool isActionComplete()
    {
        bool actionComplete = true;
        foreach (Dice dice in dices)
        {
            if (!dice.IsLocked)
            {
                actionComplete = false;
                break;
            }
        }
        return actionComplete;
    }

    public int getResult()
    {
        int result = 0;
        switch(actionType)
        {
            case (ThrowActionType.HealingPotion):
                result = getTotalScore();
                break;
            default:
                result = getNBValid();
                break;
        }
        return result;
    }

    private int getNBValid()
    {
        int nbValidDices = 0;
        foreach (Dice dice in dices)
        {
            if ((int)dice.Value > minimumValueNeeded)
            {
                nbValidDices++;
            }
        }
        return nbValidDices;
    }

    private int getTotalScore()
    {
        int totalScore = 0;
        foreach (Dice dice in dices)
        {
            totalScore += (int)dice.Value;
        }
        return totalScore;
    }
}

