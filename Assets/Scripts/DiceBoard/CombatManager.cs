using UnityEngine;
using UnityEngine.SceneManagement;

namespace Djinde.Quest
{
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
            DiceActionManager.Instance.onActionPerformed += onActionComplete;
            DiceActionManager.Instance.performThrowAction(EThrowActionType.PlayerHit, PlayerFastAccess._CharacterStats.GetPassiveStatOfType(EPassiveStatType.HitRolls), PlayerFastAccess._CharacterStats.GetPassiveStatOfType(EPassiveStatType.ScoreToHit));
        }

        public int getScoreToWoundCurrentEnnemy()
        {
            return PlayerFastAccess._CharacterStats.GetScoreToWoundAgainst(CurrentEnnemy._Stats);
        }

        private void onActionComplete(EThrowActionType actionType, int result)
        {
            switch (actionType)
            {
                case (EThrowActionType.PlayerHit):
                    if (result > 0)
                    {
                        DiceActionManager.Instance.performThrowAction(EThrowActionType.PlayerWound, result, getScoreToWoundCurrentEnnemy());
                    }
                    else
                    {
                        DiceActionManager.Instance.performThrowAction(EThrowActionType.EnnemyHit, CurrentEnnemy._Stats.GetPassiveStatOfType(EPassiveStatType.HitRolls), CurrentEnnemy._Stats.GetPassiveStatOfType(EPassiveStatType.ScoreToHit));
                    }
                    break;
                case (EThrowActionType.PlayerWound):
                    CurrentEnnemy._Stats.AddHealthPointsModifier(-result);
                    if (CurrentEnnemy._Stats._IsDead)
                    {
                        onEnnemyDeath();
                    }
                    else
                    {
                        DiceActionManager.Instance.performThrowAction(EThrowActionType.EnnemyHit, CurrentEnnemy._Stats.GetPassiveStatOfType(EPassiveStatType.HitRolls), CurrentEnnemy._Stats.GetPassiveStatOfType(EPassiveStatType.ScoreToHit));
                    }
                    break;
                case (EThrowActionType.EnnemyHit):
                    if (result > 0)
                    {
                        //TODO: Update this line!!
                        DiceActionManager.Instance.performThrowAction(EThrowActionType.EnnemyWound, result, CurrentEnnemy._Stats.GetScoreToWoundAgainst(PlayerFastAccess._CharacterStats));
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
        }

        private void onPlayerDeath()
        {
            SpellsManager.Instance.clearActiveBonuses();
            SceneManager.LoadScene("Intro");
        }
    }
}