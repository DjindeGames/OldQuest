using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    public Ennemy CurrentEnnemy { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerStatsManager.Instance.onPlayerDeath += onPlayerDeath;
    }

    private void OnDestroy()
    {
        PlayerStatsManager.Instance.onPlayerDeath -= onPlayerDeath;
    }

    public void startCombat(Ennemy ennemy)
    {
        CurrentEnnemy = ennemy;
        CurrentEnnemy.onEnnemyDeath += onEnnemyDeath;
        DiceActionManager.Instance.onActionPerformed += onActionComplete;
        DiceActionManager.Instance.performThrowAction(ThrowActionType.PlayerHit, PlayerStatsManager.Instance.HitRolls, PlayerStatsManager.Instance.ScoreToHit);
    }

    public int getScoreToWoundCurrentEnnemy()
    {
        return PlayerStatsManager.Instance.getScoreToWoundAgainst(CurrentEnnemy.stats.endurance);
    }

    private void onActionComplete(ThrowActionType actionType, int result)
    {
        switch(actionType)
        {
            case (ThrowActionType.PlayerHit):
                if (result > 0)
                {
                    DiceActionManager.Instance.performThrowAction(ThrowActionType.PlayerWound, result, PlayerStatsManager.Instance.getScoreToWoundAgainst(CurrentEnnemy.stats.endurance));
                }
                else
                {
                    DiceActionManager.Instance.performThrowAction(ThrowActionType.EnnemyHit, CurrentEnnemy.stats.hitRolls, CurrentEnnemy.stats.scoreToHit);
                }
                break;
            case (ThrowActionType.PlayerWound):
                if (CurrentEnnemy.resolveWounds(result))
                {
                    onEnnemyDeath();
                }
                else
                {
                    DiceActionManager.Instance.performThrowAction(ThrowActionType.EnnemyHit, CurrentEnnemy.stats.hitRolls, CurrentEnnemy.stats.scoreToHit);
                }
                break;
            case (ThrowActionType.EnnemyHit):
                if (result > 0)
                {
                    DiceActionManager.Instance.performThrowAction(ThrowActionType.EnnemyWound, result, PlayerStatsManager.Instance.getScoreToWoundAgainst(CurrentEnnemy.stats.endurance));
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
        CurrentEnnemy.onDeath();
        SpellsManager.Instance.clearActiveBonuses();
        DiceActionManager.Instance.onActionPerformed -= onActionComplete;
        CurrentEnnemy.onEnnemyDeath -= onEnnemyDeath;
    }

    private void onPlayerDeath()
    {
        SpellsManager.Instance.clearActiveBonuses();
        SceneManager.LoadScene("Intro");
    }
}
