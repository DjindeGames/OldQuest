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
        PlayerStatsManager._Instance._PlayerStats.OnPassiveStatUpdated += OnPassiveStatUpdated;
        PlayerStatsManager._Instance._PlayerStats.OnHealthUpdatedEvent += OnHealthUpdated;
        SpellsManager.Instance.onDeathShardsUpdated += onDeathShardsUpdated;
        SpellsManager.Instance.onMaxDeathShardsUpdated += onMaxDeathShardsUpdated;

        onDeathShardsUpdated(SpellsManager.Instance.DeathShards);
        onMaxDeathShardsUpdated(SpellsManager.Instance.MaxDeathShards);
        OnHealthUpdated(PlayerStatsManager._Instance._PlayerStats.GetCurrentHealth());
        OnPassiveStatUpdated(EPassiveStatType.Vitality, PlayerStatsManager._Instance._PlayerStats.GetPassiveStatOfType(EPassiveStatType.Vitality));
        OnPassiveStatUpdated(EPassiveStatType.Strength, PlayerStatsManager._Instance._PlayerStats.GetPassiveStatOfType(EPassiveStatType.Strength));
        OnPassiveStatUpdated(EPassiveStatType.Endurance, PlayerStatsManager._Instance._PlayerStats.GetPassiveStatOfType(EPassiveStatType.Endurance));
        OnPassiveStatUpdated(EPassiveStatType.HitRolls, PlayerStatsManager._Instance._PlayerStats.GetPassiveStatOfType(EPassiveStatType.HitRolls));
        OnPassiveStatUpdated(EPassiveStatType.ScoreToHit, PlayerStatsManager._Instance._PlayerStats.GetPassiveStatOfType(EPassiveStatType.ScoreToHit));
        OnPassiveStatUpdated(EPassiveStatType.Damages, PlayerStatsManager._Instance._PlayerStats.GetPassiveStatOfType(EPassiveStatType.Damages));
        OnPassiveStatUpdated(EPassiveStatType.Armor, PlayerStatsManager._Instance._PlayerStats.GetPassiveStatOfType(EPassiveStatType.Armor));
        OnPassiveStatUpdated(EPassiveStatType.BonusToWound, PlayerStatsManager._Instance._PlayerStats.GetPassiveStatOfType(EPassiveStatType.BonusToWound));
    }

    private void OnDestroy()
    {
        PlayerStatsManager._Instance._PlayerStats.OnPassiveStatUpdated -= OnPassiveStatUpdated;
        PlayerStatsManager._Instance._PlayerStats.OnHealthUpdatedEvent -= OnHealthUpdated;
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
        PlayerCharacterStats playerStats = PlayerStatsManager._Instance._PlayerStats;

        StatValueLabel label = GetStatValueLabelByType(type);
        if (label != null)
        {
            label._label.text = newValue.ToString();
        }
        StatValueLabel bonusLabel = GetStatBonusValueLabelByType(type);
        if (bonusLabel != null)
        {
            bonusLabel._label.text = "[" + ((newValue >= 0) ? "+" : "") + newValue + "]";
            bonusLabel._label.color = GetAppropriateBonusColorByTypeAndValue(type, newValue);
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
    public TMP_Text _label;
}
