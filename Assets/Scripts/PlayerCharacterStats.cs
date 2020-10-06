using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterStats : CharacterStats
{
    #region Public Methods

    public void LoadStats(BaseCharacterStats stats)
    {
        _baseCharacterStats = stats;
    }

    public BaseCharacterStats GetStats()
    {
        BaseCharacterStats stats = new BaseCharacterStats();
        stats.SetStat(EPassiveStatType.Vitality, _baseCharacterStats.GetStat(EPassiveStatType.Vitality));
        stats.SetStat(EPassiveStatType.Strength, _baseCharacterStats.GetStat(EPassiveStatType.Strength));
        stats.SetStat(EPassiveStatType.Endurance, _baseCharacterStats.GetStat(EPassiveStatType.Endurance));
        stats.SetStat(EPassiveStatType.HitRolls, _baseCharacterStats.GetStat(EPassiveStatType.HitRolls));
        stats.SetStat(EPassiveStatType.ScoreToHit, _baseCharacterStats.GetStat(EPassiveStatType.ScoreToHit));
        stats.SetStat(EPassiveStatType.Damages, _baseCharacterStats.GetStat(EPassiveStatType.Damages));
        stats.SetCurrentHealth(_baseCharacterStats.GetCurrentHealth());
        return stats;
    }

    #endregion
}
