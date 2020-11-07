using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    #region Events

    public delegate void ActiveBonusUpdatedEvent(EActiveBonusType which, bool newValue);
    public event ActiveBonusUpdatedEvent OnActiveBonusUpdatedEvent;

    public delegate void PassiveStatUpdatedEvent(EPassiveStatType which, int newValue);
    public event PassiveStatUpdatedEvent OnPassiveStatUpdated;

    public delegate void HealthUpdatedEvent(int currentHealth);
    public event HealthUpdatedEvent OnHealthUpdatedEvent;

    #endregion

    #region Exposed Attributes

    [Header("Parameters")]
    [SerializeField]
    protected BaseCharacterStats _baseCharacterStats = new BaseCharacterStats();

    #endregion

    #region Attributes

    public bool _HasFullHealth
    {
        get
        {
            return (_baseCharacterStats.GetCurrentHealth() == _baseCharacterStats.GetStat(EPassiveStatType.Vitality));
        }
    }

    public bool _IsDead
    {
        get
        {
            return (_baseCharacterStats.GetCurrentHealth() == 0);
        }
    }

    private List<PassiveBonus> _registeredPassiveBonuses = new List<PassiveBonus>();
    private List<ActiveBonus> _registeredActiveBonuses = new List<ActiveBonus>();

    #endregion

    #region Public Methods

    public void AddHealthPointsModifier(int modifier)
    {
        int newHealthPoints = _baseCharacterStats.GetCurrentHealth() + modifier;
        newHealthPoints = Mathf.Clamp(newHealthPoints, 0, _baseCharacterStats.GetStat(EPassiveStatType.Vitality));
        _baseCharacterStats.SetCurrentHealth(newHealthPoints);
        OnHealthUpdatedEvent?.Invoke(newHealthPoints);
    }

    public void IncreaseBaseStat(EPassiveStatType type, int amount)
    {
        int newValue = _baseCharacterStats.GetStat(type) + amount;
        _baseCharacterStats.SetStat(type, newValue);
        OnPassiveStatUpdated?.Invoke(type, GetPassiveStatOfType(type));
    }

    public int GetCurrentHealth()
    {
        return _baseCharacterStats.GetCurrentHealth();
    }

    public int GetPassiveStatOfType(EPassiveStatType passiveStatType, bool includeBonuses = true)
    {
        int stat = _baseCharacterStats.GetStat(passiveStatType);
        if (includeBonuses)
        {
            stat += GetPassiveBonusOfType(passiveStatType);
        }
        switch(passiveStatType)
        {
            
            case (EPassiveStatType.HitRolls):
            case (EPassiveStatType.Endurance):
            case (EPassiveStatType.Strength):
                stat = Mathf.Clamp(stat, 1, int.MaxValue);
                break;
            case (EPassiveStatType.ScoreToHit):
                stat = Mathf.Clamp(stat, 1, 6);
                break;
            case (EPassiveStatType.Armor):
            case (EPassiveStatType.Damages):
            case (EPassiveStatType.BonusToWound):
            case (EPassiveStatType.Vitality):
                break;
        }
        return stat;
    }

    public int GetPassiveBonusOfType(EPassiveStatType bonusType)
    {
        int totalBonus = 0;
        foreach (PassiveBonus bonus in _registeredPassiveBonuses)
        {
            if (bonus.type == bonusType)
            {
                totalBonus += bonus.value;
            }
        }
        return totalBonus;
    }

    public bool HasActiveBonusOfType(EActiveBonusType bonusType)
    {
        bool hasBonus = false;
        foreach (ActiveBonus bonus in _registeredActiveBonuses)
        {
            if (bonus.type == bonusType)
            {
                hasBonus = true;
                break;
            }
        }
        return hasBonus;
    }

    public int GetScoreToWoundAgainst(CharacterStats oppenentStats)
    {
        int scoreNeeded = 1;
        int strength = _baseCharacterStats.GetStat(EPassiveStatType.Strength);
        int endurance = oppenentStats.GetPassiveStatOfType(EPassiveStatType.Endurance);
        if (endurance >= 2 * strength)
        {
            scoreNeeded = 6;
        }
        else if (endurance > strength)
        {
            scoreNeeded = 5;
        }
        else if (strength == endurance)
        {
            scoreNeeded = 4;
        }
        else if (strength >= 2 * endurance)
        {
            scoreNeeded = 2;
        }
        else if (strength > endurance)
        {
            scoreNeeded = 3;
        }
        return scoreNeeded - GetPassiveBonusOfType(EPassiveStatType.BonusToWound);
    }

    public void RegisterPassiveBonus(PassiveBonus bonus)
    {
        _registeredPassiveBonuses.Add(bonus);
        OnPassiveStatUpdated?.Invoke(bonus.type, GetPassiveStatOfType(bonus.type));
    }

    public void UnregisterPassiveBonus(PassiveBonus bonus)
    {
        _registeredPassiveBonuses.Remove(bonus);
        OnPassiveStatUpdated?.Invoke(bonus.type, GetPassiveStatOfType(bonus.type));
    }

    public void RegisterActiveBonus(ActiveBonus bonus)
    {
        bool hadBonus = HasActiveBonusOfType(bonus.type);
        _registeredActiveBonuses.Add(bonus);
        if (!hadBonus)
        {
            OnActiveBonusUpdatedEvent?.Invoke(bonus.type, true);
        }
    }

    public void UnregisterActiveBonus(ActiveBonus bonus)
    {
        bool hadBonus = HasActiveBonusOfType(bonus.type);
        _registeredActiveBonuses.Remove(bonus);
        if (HasActiveBonusOfType(bonus.type) == false)
        {
            OnActiveBonusUpdatedEvent?.Invoke(bonus.type, false);
        }
    }

    #endregion
}

[System.Serializable]
public class BaseCharacterStats
{
    [SerializeField]
    private int _healthPoints = 3;

    //Vanilla Base Stats
    [SerializeField]
    private int _vitality = 3;
    [SerializeField]
    private int _strength = 3;
    [SerializeField]
    private int _endurance = 3;
    [SerializeField]
    private int _hitRolls = 1;
    [SerializeField]
    private int _scoreToHit = 5;
    [SerializeField]
    private int _damages = 1;

    //Not Vanilla, cannot be set
    private int _armor = 0;
    private int _bonusToWound = 0;

    public int GetCurrentHealth()
    {
        return _healthPoints;
    }

    public void SetCurrentHealth(int healthPoints)
    {
        _healthPoints = healthPoints;
    }

    public int GetStat(EPassiveStatType type)
    {
        int stat = -1;
        switch(type)
        {
            case (EPassiveStatType.Vitality):
                stat = _vitality;
                break;
            case (EPassiveStatType.Strength):
                stat = _strength;
                break;
            case (EPassiveStatType.Endurance):
                stat = _endurance;
                break;
            case (EPassiveStatType.HitRolls):
                stat = _hitRolls;
                break;
            case (EPassiveStatType.ScoreToHit):
                stat = _scoreToHit;
                break;
            case (EPassiveStatType.Damages):
                stat = _damages;
                break;
            //Not Vanilla, always 0
            case (EPassiveStatType.BonusToWound):
                stat = _bonusToWound;
                break;
            case (EPassiveStatType.Armor):
                stat = _armor;
                break;
        }
        return stat;
    }

    public void SetStat(EPassiveStatType type, int value)
    {
        switch (type)
        {
            case (EPassiveStatType.Vitality):
                _vitality = value;
                _healthPoints = Mathf.Clamp(_healthPoints, 0, _vitality);
                break;
            case (EPassiveStatType.Strength):
                _strength = value;
                break;
            case (EPassiveStatType.Endurance):
                _endurance = value;
                break;
            case (EPassiveStatType.HitRolls):
                _hitRolls = value;
                break;
            case (EPassiveStatType.ScoreToHit):
                _scoreToHit = value;
                break;
            case (EPassiveStatType.Damages):
                _damages = value;
                break;
        }
    }
}