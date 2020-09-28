using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance { get; private set; }

    public bool HasFullHealth
    {
        get
        {
            return Health == Vitality;
        }
    }

    public int Health
    {
        get
        {
            return playerStats.healthPoints;
        }
        private set
        {
            playerStats.healthPoints = value;
            onHealthUpdated?.Invoke(value);
        }
    }

    public int Vitality
    {
        get
        {
            return playerStats.vitality + equipmentBonuses.vitality;
        }
        private set
        {
            playerStats.vitality = value;
            onVitalityUpdated?.Invoke(value + equipmentBonuses.vitality);
        }
    }

    public int Strength
    {
        get
        {
            int strength = playerStats.strength + equipmentBonuses.strength;
            strength = Mathf.Clamp(strength, 1, int.MaxValue);
            return strength;
        }
        private set
        {
            playerStats.strength = value;
        }
    }

    public int Endurance
    {
        get
        {
            int endurance = playerStats.endurance + equipmentBonuses.endurance;
            endurance = Mathf.Clamp(endurance, 1, int.MaxValue);
            return endurance;
        }
        private set
        {
            playerStats.endurance = value;
        }
    }

    public int HitRolls
    {
        get
        {
            int hitRolls = playerStats.hitRolls + equipmentBonuses.hitRolls + SpellsManager.Instance.getTotalSpellBonusOfType(SpellBonusType.ExtraHit);
            hitRolls = Mathf.Clamp(hitRolls, 1, int.MaxValue);
            return hitRolls;
        }
        private set
        {
            playerStats.hitRolls = value;
        }
    }

    public int Armor
    {
        get
        {
            return equipmentBonuses.armor + SpellsManager.Instance.getTotalSpellBonusOfType(SpellBonusType.Armor);
        }
    }

    public int ScoreToHit
    {
        get
        {
            int scoreToHit = playerStats.scoreToHit + equipmentBonuses.toHit + SpellsManager.Instance.getTotalSpellBonusOfType(SpellBonusType.ToHit);
            scoreToHit = Mathf.Clamp(scoreToHit, 1, 6);
            return scoreToHit;
        }
        private set
        {
            playerStats.scoreToHit = value;
        }
    }

    public int Damages
    {
        get
        {
            return playerStats.damages + equipmentBonuses.damages + SpellsManager.Instance.getTotalSpellBonusOfType(SpellBonusType.Damages);
        }
        private set
        {
            playerStats.damages = value;
        }
    }

    public int BonusToWound
    {
        get
        {
            return equipmentBonuses.toWound + SpellsManager.Instance.getTotalSpellBonusOfType(SpellBonusType.ToWound);
        }
    }

    //Events
    public delegate void statsUpdated();
    public event statsUpdated onStatsUpdated;

    public delegate void healthUpdated(int currentHealth);
    public event healthUpdated onHealthUpdated;

    public delegate void vitalityUpdated(int currentVitality);
    public event healthUpdated onVitalityUpdated;

    public delegate void playerDead();
    public event playerDead onPlayerDeath;

    private PlayerStats playerStats = new PlayerStats();
    private EquipmentBonuses equipmentBonuses = new EquipmentBonuses();

    void Awake()
    {
        Instance = this;
    }

    public void addHealthPointsModifier(int modifier)
    {
        int newHealthPoints = playerStats.healthPoints + modifier;
        newHealthPoints = Mathf.Clamp(newHealthPoints, 0, Vitality);
        Health = newHealthPoints;
        if (Health == 0)
        {
            onPlayerDeath?.Invoke();
        }
    }

    public void applyBaseStatModifier(BaseStat which, uint value)
    {
        onStatsUpdated();
        switch (which)
        {
            case (BaseStat.Vitality):
                Vitality = playerStats.vitality + (int)value;
                break;
            case (BaseStat.Strength):
                playerStats.vitality += (int)value;
                break;
            case (BaseStat.Endurance):
                playerStats.vitality += (int)value;
                break;
            case (BaseStat.HitRolls):
                playerStats.hitRolls += (int)value;
                break;
            case (BaseStat.ScoreToHit):
                playerStats.scoreToHit += (int)value;
                break;
            case (BaseStat.Damages):
                playerStats.damages += (int)value;
                break;
        }
    }

    public void applyEquipmentBonus(GearStats stats)
    {
        switch (stats.type)
        {
            case (EquipmentBonus.Armor):
                equipmentBonuses.armor += stats.value;
                break;
            case (EquipmentBonus.Endurance):
                equipmentBonuses.endurance += stats.value;
                break;
            case (EquipmentBonus.HitRolls):
                equipmentBonuses.hitRolls += stats.value;
                break;
            case (EquipmentBonus.Strength):
                equipmentBonuses.strength += stats.value;
                break;
            case (EquipmentBonus.ToHit):
                equipmentBonuses.toHit += stats.value;
                break;
            case (EquipmentBonus.ToWound):
                equipmentBonuses.toWound += stats.value;
                break;
            case (EquipmentBonus.Damages):
                equipmentBonuses.damages += stats.value;
                break;
            case (EquipmentBonus.Vitality):
                equipmentBonuses.vitality += stats.value;
                int newHealthPoints = playerStats.healthPoints;
                Mathf.Clamp(newHealthPoints, 0, Vitality);
                Health = newHealthPoints;
                onVitalityUpdated?.Invoke(Vitality);
                break;
        }
        onStatsUpdated?.Invoke();
    }

    public void unapplyEquipmentBonus(GearStats stats)
    {
        switch (stats.type)
        {
            case (EquipmentBonus.Armor):
                equipmentBonuses.armor -= stats.value;
                break;
            case (EquipmentBonus.Endurance):
                equipmentBonuses.endurance -= stats.value;
                break;
            case (EquipmentBonus.HitRolls):
                equipmentBonuses.hitRolls -= stats.value;
                break;
            case (EquipmentBonus.Strength):
                equipmentBonuses.strength -= stats.value;
                break;
            case (EquipmentBonus.ToHit):
                equipmentBonuses.toHit -= stats.value;
                break;
            case (EquipmentBonus.ToWound):
                equipmentBonuses.toWound -= stats.value;
                break;
            case (EquipmentBonus.Vitality):
                equipmentBonuses.vitality -= stats.value;
                int newHealthPoints = playerStats.healthPoints;
                Mathf.Clamp(newHealthPoints, 0, Vitality);
                Health = newHealthPoints;
                onVitalityUpdated?.Invoke(Vitality);
                break;
            case (EquipmentBonus.Damages):
                equipmentBonuses.damages -= stats.value;
                break;
        }
        onStatsUpdated?.Invoke();
    }

    public void loadStats(PlayerStats stats)
    {
        playerStats.healthPoints = stats.healthPoints;
        playerStats.vitality = stats.vitality;
        playerStats.strength = stats.strength;
        playerStats.endurance = stats.endurance;
        playerStats.hitRolls = stats.hitRolls;
        playerStats.scoreToHit = stats.scoreToHit;
        playerStats.damages = stats.damages;
    }

    public PlayerStats getPlayerStats()
    {
        PlayerStats copiedPlayerStats = new PlayerStats();
        copiedPlayerStats.healthPoints = playerStats.healthPoints;
        copiedPlayerStats.vitality = playerStats.vitality;
        copiedPlayerStats.strength = playerStats.strength;
        copiedPlayerStats.endurance = playerStats.endurance;
        copiedPlayerStats.hitRolls = playerStats.hitRolls;
        copiedPlayerStats.scoreToHit = playerStats.scoreToHit;
        copiedPlayerStats.damages = playerStats.damages;
        return copiedPlayerStats;
    }

    public int getEquipmentBonus(EquipmentBonus which)
    {
        int bonus = 0;
        switch(which)
        {
            case (EquipmentBonus.Vitality):
                bonus = equipmentBonuses.vitality;
                break;
            case (EquipmentBonus.Strength):
                bonus = equipmentBonuses.strength;
                break;
            case (EquipmentBonus.Endurance):
                bonus = equipmentBonuses.endurance;
                break;
            case (EquipmentBonus.Armor):
                bonus = equipmentBonuses.armor;
                break;
            case (EquipmentBonus.HitRolls):
                bonus = equipmentBonuses.hitRolls;
                break;
            case (EquipmentBonus.ToHit):
                bonus = equipmentBonuses.toHit;
                break;
            case (EquipmentBonus.ToWound):
                bonus = equipmentBonuses.toWound;
                break;
            case (EquipmentBonus.Damages):
                bonus = equipmentBonuses.damages;
                break;
        }
        return bonus;
    }

    public int getScoreToWoundAgainst(int endurance)
    {
        int scoreNeeded = 1;

        if (endurance >= 2 * Strength)
        {
            scoreNeeded = 6;
        }
        else if (endurance > Strength)
        {
            scoreNeeded = 5;
        }
        else if (Strength == endurance)
        {
            scoreNeeded = 4;
        }
        else if (Strength >= 2 * endurance)
        {
            scoreNeeded = 2;
        }
        else if (Strength > endurance)
        {
            scoreNeeded = 3;
        }

        return scoreNeeded - BonusToWound;
    }
}

public class PlayerStats
{
    public int healthPoints = 1;
    public int vitality = 10;
    public int strength = 3;
    public int endurance = 3;
    public int hitRolls = 1;
    public int scoreToHit = 5;
    public int damages = 1;
}

public class EquipmentBonuses
{
    public int armor = 0;
    public int hitRolls = 0;
    public int toHit = 0;
    public int toWound = 0;
    public int vitality = 0;
    public int strength = 0;
    public int endurance = 0;
    public int damages = 0;
}

