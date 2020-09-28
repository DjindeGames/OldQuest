using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DiceBoardManager : MonoBehaviour
{
    public Collider TableCollider { get; private set; }

    public ThrowActionPerformer currentPerformer
    {
        get
        {
            return getCurrentThrowAction().actionPerformer;
        }
    }

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

    //Selection
    private Vector2 selectionStart;
    private float currentSelectionWidth;
    private float currentSelectionHeight;
    private Vector2 currentSelectionCenter;
    private bool selecting = false;
    private bool isGrabbingMultipleDices = false;
    private Dice currentGrabbedDice;

    //Misc
    private bool deactivated = true;
    float remainingThrowCooldown = 1f;

    void Awake()
    {
        Instance = this;
        TableCollider = (Collider)diceBoard.GetComponents(typeof(Collider))[0];
        selectionBox.enabled = false;
        Random.InitState(System.DateTime.Now.Millisecond);
    }

    void Update()
    {
        ThrowAction currentThrowAction = getCurrentThrowAction();
        if (currentThrowAction != null && !deactivated)
        {
            if (currentThrowAction.isActionComplete())
            {
                StartCoroutine(resolveThrow());
            }
            else
            {
                if (currentThrowAction.actionPerformer == ThrowActionPerformer.Player)
                {
                    performPlayerAction();
                }
                else
                {
                    performEnnemyAction();
                }
            }
        }
    }

    private void performPlayerAction()
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

    private void performEnnemyAction()
    {
        if (!getCurrentThrowAction().areDicesStopped())
        {
            resetThrowCooldown();
        }
        remainingThrowCooldown -= Time.deltaTime;
        if (remainingThrowCooldown <= 0)
        {
            resetThrowCooldown();
            throwAll();
        }
    }

    private IEnumerator resolveThrow()
    {
        deactivated = true;
        yield return new WaitForSeconds(1);
        onThrowComplete();
    }

    private void resetThrowCooldown()
    {
        remainingThrowCooldown = GameConstants.Instance.checkForStabilizationPeriod;
    }

    private void onThrowComplete()
    {
        ThrowAction completedAction = popThrowAction();
        computeAndClearThrowAction(completedAction);
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
        resetThrowCooldown();
        deactivated = false;
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
            Dice diceComponent = instantiatedDice.GetComponent<Dice>();
            diceComponent.setParentAction(action);
            action.dices.Add(diceComponent);
            cumulatedInstantiationOffset += instantiationIterativeOffset;
        }
    }

    private void computeAndClearThrowAction(ThrowAction action)
    {
        action.computeResult();
        for(int i = 0; i < action.dices.Count; i++)
        {
            Destroy(action.dices[i].gameObject);
        }
        action.dices.Clear();
    }

    public void throwAll()
    {
        if (getCurrentThrowAction() != null && getCurrentThrowAction().areDicesStopped())
        {
            foreach (Dice dice in getCurrentThrowAction().dices)
            {
                if (!dice.IsLocked)
                {
                    dice.randomThrow();
                }
            }
        }
    }

    public void recomputeNeededScore()
    {
        getCurrentThrowAction().recomputeNeededScore();
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
    public int result;
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

    public bool areDicesStopped()
    {
        bool dicesStabilized = true;
        foreach (Dice dice in dices)
        {
            if (dice.isMoving())
            {
                dicesStabilized = false;
                break;
            }
        }
        return dicesStabilized;
    }

    public void computeResult()
    {
        switch (actionType)
        {
            case (ThrowActionType.HealingPotion):
            case (ThrowActionType.LootChest):
                result = getValueSum();
                break;
            default:
                result = getValidSum();
                break;
        }
    }

    public void recomputeNeededScore()
    {
        switch(actionType)
        {
            case (ThrowActionType.PlayerHit):
                minimumValueNeeded = PlayerStatsManager.Instance.ScoreToHit;
                break;
            case (ThrowActionType.PlayerWound):
                minimumValueNeeded = CombatManager.Instance.getScoreToWoundCurrentEnnemy();
                break;
        }
    }

    private int getValidSum()
    {
        int nbValidDices = 0;
        foreach (Dice dice in dices)
        {
            if ((int)dice.Value >= minimumValueNeeded)
            {
                nbValidDices++;
            }
        }
        return nbValidDices;
    }

    private int getValueSum()
    {
        int totalScore = 0;
        foreach (Dice dice in dices)
        {
            totalScore += (int)dice.Value;
        }
        return totalScore;
    }
}

