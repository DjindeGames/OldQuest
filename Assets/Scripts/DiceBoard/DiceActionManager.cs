using UnityEngine;

public class DiceActionManager : MonoBehaviour
{
    public static DiceActionManager Instance { get; private set; }

    public delegate void actionPerformed(EThrowActionType actionType, int result);
    public event actionPerformed onActionPerformed;

    private ThrowAction bufferedCompletedAction;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DiceBoardManager.Instance.throwIsComplete += onThrowComplete;
        DiceBoardUI.Instance.onActionAknowledged += onActionAcknowledged;
    }

    private void OnDestroy()
    {
        DiceBoardManager.Instance.throwIsComplete -= onThrowComplete;
        DiceBoardUI.Instance.onActionAknowledged -= onActionAcknowledged;
    }

    public void performThrowAction(EThrowActionType type, int numberOfDices, int minimumValueNeeded)
    {
        ThrowAction action = new ThrowAction();
        action.actionType = type;
        switch(type)
        {
            case (EThrowActionType.HealingPotion):
            case (EThrowActionType.PlayerHit):
            case (EThrowActionType.PlayerWound):
                action.actionPerformer = EThrowActionPerformer.Player;
                action.color = EDiceColor.Green;
                break;
            case (EThrowActionType.EnnemyHit):
            case (EThrowActionType.EnnemyWound):
                action.actionPerformer = EThrowActionPerformer.Ennemy;
                action.color = EDiceColor.Red;
                break;
            default:
                action.actionPerformer = EThrowActionPerformer.Player;
                action.color = EDiceColor.Black;
                break;
        }
        action.numberOfDices = numberOfDices;
        action.minimumValueNeeded = minimumValueNeeded;
        DiceBoardManager.Instance.StartThrowAction(action);
    }

    public void onThrowComplete(ThrowAction action)
    {
        bufferedCompletedAction = action;
        switch (bufferedCompletedAction.actionType)
        {
            case (EThrowActionType.PlayerHit):
            case (EThrowActionType.PlayerWound):
            case (EThrowActionType.EnnemyHit):
            case (EThrowActionType.EnnemyWound):
                DiceBoardUI.Instance.showActionResult(bufferedCompletedAction.actionType, bufferedCompletedAction.result);
                break;
            default:
                onActionAcknowledged();
                break;
        }
    }

    public void onActionAcknowledged()
    {
        switch (bufferedCompletedAction.actionType)
        {
            case (EThrowActionType.HealingPotion):
                ScreenManager.Instance.switchToPreviousScreen();
                PlayerFastAccess._CharacterStats.AddHealthPointsModifier(bufferedCompletedAction.result);
                SoundManager.Instance.playSFX(ESFXType.DrinkPotion);
                break;
            default:
                if (onActionPerformed != null)
                {
                    onActionPerformed(bufferedCompletedAction.actionType, bufferedCompletedAction.result);
                }
                else
                {
                    ScreenManager.Instance.switchToPreviousScreen();
                }
                break;
        }
    }
}
