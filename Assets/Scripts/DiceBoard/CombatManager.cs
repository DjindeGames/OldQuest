using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    private Ennemy currentEnnemy;

    private void Awake()
    {
        Instance = this;
    }

    public void startCombat(Ennemy ennemy)
    {
        currentEnnemy = ennemy;
        currentEnnemy.onEnnemyDeath += onEnnemyDeath;
        DiceActionManager.Instance.onActionPerformed += onActionComplete;
        DiceActionManager.Instance.performThrowAction(ThrowActionType.PlayerHit, PlayerStatsManager.Instance.HitRolls, PlayerStatsManager.Instance.ScoreToHit);
    }

    private void onActionComplete(ThrowActionType actionType, int result)
    {
        switch(actionType)
        {
            case (ThrowActionType.PlayerHit):
                if (result > 0)
                {
                    DiceActionManager.Instance.performThrowAction(ThrowActionType.PlayerWound, result, PlayerStatsManager.Instance.getScoreToWoundAgainst(currentEnnemy.stats.endurance));
                }
                else
                {
                    DiceActionManager.Instance.performThrowAction(ThrowActionType.EnnemyHit, currentEnnemy.stats.hitRolls, currentEnnemy.stats.scoreToHit);
                }
                break;
            case (ThrowActionType.PlayerWound):
                if (currentEnnemy.resolveWounds(result))
                {
                    onEnnemyDeath();
                }
                else
                {
                    DiceActionManager.Instance.performThrowAction(ThrowActionType.EnnemyHit, currentEnnemy.stats.hitRolls, currentEnnemy.stats.scoreToHit);
                }
                break;
            case (ThrowActionType.EnnemyHit):
                if (result > 0)
                {
                    DiceActionManager.Instance.performThrowAction(ThrowActionType.EnnemyWound, result, PlayerStatsManager.Instance.getScoreToWoundAgainst(currentEnnemy.stats.endurance));
                }
                else
                {
                    DiceActionManager.Instance.performThrowAction(ThrowActionType.PlayerHit, PlayerStatsManager.Instance.HitRolls, PlayerStatsManager.Instance.ScoreToHit);
                }
                break;
            case (ThrowActionType.EnnemyWound):
                PlayerStatsManager.Instance.addHealthPointsModifier(-result);
                DiceActionManager.Instance.performThrowAction(ThrowActionType.PlayerHit, PlayerStatsManager.Instance.HitRolls, PlayerStatsManager.Instance.ScoreToHit);
                break;
        }
    }

    private void onEnnemyDeath()
    {
        ScreenManager.Instance.switchScreen(ScreenType.Main);
        DiceActionManager.Instance.onActionPerformed -= onActionComplete;
        currentEnnemy.onEnnemyDeath -= onEnnemyDeath;
    }

    private void onPlayerDeath()
    {

    }
}
