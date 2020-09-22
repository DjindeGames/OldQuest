using UnityEngine;

public class DiceActionManager : MonoBehaviour
{
    public static DiceActionManager Instance { get; private set; }

    public delegate void actionPerformed(ThrowActionType actionType, int result);
    public event actionPerformed onActionPerformed;

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

    public void performThrowAction(ThrowActionType type, int numberOfDices, int minimumValueNeeded)
    {
        ThrowAction action = new ThrowAction();
        action.actionType = type;
        switch(type)
        {
            case (ThrowActionType.HealingPotion):
            case (ThrowActionType.PlayerHit):
            case (ThrowActionType.PlayerWound):
                action.actionPerformer = ThrowActionPerformer.Player;
                action.color = DiceColor.Green;
                break;
            case (ThrowActionType.EnnemyHit):
            case (ThrowActionType.EnnemyWound):
                action.actionPerformer = ThrowActionPerformer.Ennemy;
                action.color = DiceColor.Red;
                break;
            default:
                action.actionPerformer = ThrowActionPerformer.Player;
                action.color = DiceColor.Black;
                break;
        }
        action.numberOfDices = numberOfDices;
        action.minimumValueNeeded = minimumValueNeeded;
        DiceBoardManager.Instance.StartThrowAction(action);
    }

    public void onThrowComplete(ThrowAction action)
    {
        switch(action.actionType)
        {
            case (ThrowActionType.HealingPotion):
                ScreenManager.Instance.switchToPreviousScreen();
                PlayerStatsManager.Instance.addHealthPointsModifier(action.result);
                SoundManager.Instance.playSFX(SFXType.DrinkPotion);
                break;
            default:
                if(onActionPerformed != null)
                {
                    onActionPerformed(action.actionType, action.result);
                }
                else
                {
                    ScreenManager.Instance.switchToPreviousScreen();
                }
                break;
        }
    }
}
