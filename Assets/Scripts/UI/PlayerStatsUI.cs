using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private OrbGauge healthGauge;
    [SerializeField]
    private OrbGauge deathShardsGauge;
    [SerializeField]
    private List<StatValueLabel> _passiveStatLabels = new List<StatValueLabel>();
    [SerializeField]
    private List<StatValueLabel> _passiveStatBonusLabels = new List<StatValueLabel>();

    public static PlayerStatsUI Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerFastAccess._CharacterStats.OnPassiveStatUpdated += OnPassiveStatUpdated;
        PlayerFastAccess._CharacterStats.OnHealthUpdatedEvent += OnHealthUpdated;
        SpellsManager.Instance.onDeathShardsUpdated += onDeathShardsUpdated;
        SpellsManager.Instance.onMaxDeathShardsUpdated += onMaxDeathShardsUpdated;

        onDeathShardsUpdated(SpellsManager.Instance.DeathShards);
        onMaxDeathShardsUpdated(SpellsManager.Instance.MaxDeathShards);
        OnHealthUpdated(PlayerFastAccess._CharacterStats.GetCurrentHealth());
        OnPassiveStatUpdated(EPassiveStatType.Vitality, PlayerFastAccess._CharacterStats.GetPassiveStatOfType(EPassiveStatType.Vitality));
        OnPassiveStatUpdated(EPassiveStatType.Strength, PlayerFastAccess._CharacterStats.GetPassiveStatOfType(EPassiveStatType.Strength));
        OnPassiveStatUpdated(EPassiveStatType.Endurance, PlayerFastAccess._CharacterStats.GetPassiveStatOfType(EPassiveStatType.Endurance));
        OnPassiveStatUpdated(EPassiveStatType.HitRolls, PlayerFastAccess._CharacterStats.GetPassiveStatOfType(EPassiveStatType.HitRolls));
        OnPassiveStatUpdated(EPassiveStatType.ScoreToHit, PlayerFastAccess._CharacterStats.GetPassiveStatOfType(EPassiveStatType.ScoreToHit));
        OnPassiveStatUpdated(EPassiveStatType.Damages, PlayerFastAccess._CharacterStats.GetPassiveStatOfType(EPassiveStatType.Damages));
        OnPassiveStatUpdated(EPassiveStatType.Armor, PlayerFastAccess._CharacterStats.GetPassiveStatOfType(EPassiveStatType.Armor));
        OnPassiveStatUpdated(EPassiveStatType.BonusToWound, PlayerFastAccess._CharacterStats.GetPassiveStatOfType(EPassiveStatType.BonusToWound));
    }

    private void OnDestroy()
    {
        PlayerFastAccess._CharacterStats.OnPassiveStatUpdated -= OnPassiveStatUpdated;
        PlayerFastAccess._CharacterStats.OnHealthUpdatedEvent -= OnHealthUpdated;
        SpellsManager.Instance.onDeathShardsUpdated -= onDeathShardsUpdated;
        SpellsManager.Instance.onMaxDeathShardsUpdated -= onMaxDeathShardsUpdated;
    }

    private void OnHealthUpdated(int newValue)
    {
        healthGauge.setValue(newValue);
    }

    private void onVitalityUpdated(int vitality)
    {
        healthGauge.setMaxValue(vitality);
    }

    private void onDeathShardsUpdated(int deathShards)
    {
        deathShardsGauge.setValue(deathShards);
    }

    private void onMaxDeathShardsUpdated(int maxDeathShards)
    {
        deathShardsGauge.setMaxValue(maxDeathShards);
    }

    private void OnPassiveStatUpdated(EPassiveStatType type, int newValue)
    {
        PlayerCharacterStats playerStats = PlayerFastAccess._CharacterStats;

        StatValueLabel label = GetStatValueLabelByType(type);
        if (label != null)
        {
            foreach(TMP_Text text in label._labels)
            {
                text.text = newValue.ToString();
            }
        }
        StatValueLabel bonusLabel = GetStatBonusValueLabelByType(type);
        if (bonusLabel != null)
        {
            int bonusValue = PlayerFastAccess._CharacterStats.GetPassiveBonusOfType(type);
            foreach (TMP_Text text in bonusLabel._labels)
            {
                text.text = bonusValue.ToString();
                text.text = "[" + ((bonusValue >= 0) ? "+" : "") + bonusValue + "]";
                text.color = GetAppropriateBonusColorByTypeAndValue(type, bonusValue);
            }
        }
        if (type == EPassiveStatType.Vitality)
        {
            healthGauge.setMaxValue(newValue);
        }
    }

    private StatValueLabel GetStatValueLabelByType(EPassiveStatType which)
    {
        StatValueLabel foundLabel = null;
        foreach(StatValueLabel label in _passiveStatLabels)
        {
            if (label._type == which)
            {
                foundLabel = label;
                break;
            }
        }
        return foundLabel;
    }

    private StatValueLabel GetStatBonusValueLabelByType(EPassiveStatType which)
    {
        StatValueLabel foundLabel = null;
        foreach (StatValueLabel label in _passiveStatBonusLabels)
        {
            if (label._type == which)
            {
                foundLabel = label;
                break;
            }
        }
        return foundLabel;
    }

    private Color GetAppropriateBonusColorByTypeAndValue(EPassiveStatType type, int value)
    {
        Color bonusColor = Color.black;
        switch(type)
        {
            case (EPassiveStatType.Vitality):
            case (EPassiveStatType.Endurance):
            case (EPassiveStatType.Strength):
            case (EPassiveStatType.HitRolls):
            case (EPassiveStatType.Damages):
            case (EPassiveStatType.Armor):
                if (value > 0)
                {
                    bonusColor = Color.green;
                }
                else if (value < 0)
                {
                    bonusColor = Color.red;
                }
                break;
            case (EPassiveStatType.ScoreToHit):
                if (value > 0)
                {
                    bonusColor = Color.red;
                }
                else if (value < 0)
                {
                    bonusColor = Color.green;
                }
                break;
        }
        return bonusColor;
    }
}

[System.Serializable]
public class StatValueLabel
{
    public EPassiveStatType _type;
    public TMP_Text[] _labels;
}
