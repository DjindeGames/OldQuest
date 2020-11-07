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

    public void startCombat(Ennemy ennemy)
    {
        CurrentEnnemy = ennemy;
        CurrentEnnemy.onEnnemyDeath += onEnnemyDeath;
        DiceActionManager.Instance.onActionPerformed += onActionComplete;
        DiceActionManager.Instance.performThrowAction(EThrowActionType.PlayerHit, PlayerFastAccess._CharacterStats.GetPassiveStatOfType(EPassiveStatType.HitRolls), PlayerFastAccess._CharacterStats.GetPassiveStatOfType(EPassiveStatType.ScoreToHit));
    }

    public int getScoreToWoundCurrentEnnemy()
    {
        return PlayerFastAccess._CharacterStats.GetScoreToWoundAgainst(CurrentEnnemy.stats);
    }

    private void onActionComplete(EThrowActionType actionType, int result)
    {
        switch(actionType)
        {
            case (EThrowActionType.PlayerHit):
                if (result > 0)
                {
                    DiceActionManager.Instance.performThrowAction(EThrowActionType.PlayerWound, result, getScoreToWoundCurrentEnnemy());
                }
                else
                {
                    DiceActionManager.Instance.performThrowAction(EThrowActionType.EnnemyHit, CurrentEnnemy.stats.GetPassiveStatOfType(EPassiveStatType.HitRolls), CurrentEnnemy.stats.GetPassiveStatOfType(EPassiveStatType.ScoreToHit));
                }
                break;
            case (EThrowActionType.PlayerWound):
                CurrentEnnemy.stats.AddHealthPointsModifier(-result);
                if (CurrentEnnemy.stats._IsDead)
                {
                    onEnnemyDeath();
                }
                else
                {
                    DiceActionManager.Instance.performThrowAction(EThrowActionType.EnnemyHit, CurrentEnnemy.stats.GetPassiveStatOfType(EPassiveStatType.HitRolls), CurrentEnnemy.stats.GetPassiveStatOfType(EPassiveStatType.ScoreToHit));
                }
                break;
            case (EThrowActionType.EnnemyHit):
                if (result > 0)
                {
                    //TODO: Update this line!!
                    DiceActionManager.Instance.performThrowAction(EThrowActionType.EnnemyWound, result, CurrentEnnemy.stats.GetScoreToWoundAgainst(PlayerFastAccess._CharacterStats));
                }
                else
                {
                    DiceActionManager.Instance.performThrowAction(EThrowActionType.PlayerHit, PlayerFastAccess._CharacterStats.GetPassiveStatOfType(EPassiveStatType.HitRolls), PlayerFastAccess._CharacterStats.GetPassiveStatOfType(EPassiveStatType.ScoreToHit));
                }
                break;
            case (EThrowActionType.EnnemyWound):
                PlayerFastAccess._CharacterStats.AddHealthPointsModifier(-result);
                if (PlayerFastAccess._CharacterStats._IsDead)
                {
                    onPlayerDeath();
                }
                else
                {
                    DiceActionManager.Instance.performThrowAction(EThrowActionType.PlayerHit, PlayerFastAccess._CharacterStats.GetPassiveStatOfType(EPassiveStatType.HitRolls), PlayerFastAccess._CharacterStats.GetPassiveStatOfType(EPassiveStatType.ScoreToHit));
                }
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
