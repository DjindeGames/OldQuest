using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private OrbGauge healthGauge;
    [SerializeField]
    private OrbGauge deathShardsGauge;
    [SerializeField]
    private TMP_Text statsFrameVitality;
    [SerializeField]
    private TMP_Text statsFrameVitalityBonus;
    [SerializeField]
    private TMP_Text statsFrameStrength;
    [SerializeField]
    private TMP_Text statsFrameStrengthBonus;
    [SerializeField]
    private TMP_Text statsFrameEndurance;
    [SerializeField]
    private TMP_Text statsFrameEnduranceBonus;
    [SerializeField]
    private TMP_Text statsFrameArmor;
    [SerializeField]
    private TMP_Text statsFrameHitRolls;
    [SerializeField]
    private TMP_Text statsFrameHitRollsBonus;
    [SerializeField]
    private TMP_Text statsFrameScoreToHit;
    [SerializeField]
    private TMP_Text statsFrameScoreToHitBonus;
    [SerializeField]
    private TMP_Text statsFrameBonusToWound;

    public static PlayerStatsUI Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerStatsManager.Instance.onHealthUpdated += onHealthUpdated;
        PlayerStatsManager.Instance.onVitalityUpdated += onVitalityUpdated;
        SpellsManager.Instance.onDeathShardsUpdated += onDeathShardsUpdated;
        SpellsManager.Instance.onMaxDeathShardsUpdated += onMaxDeathShardsUpdated;
        PlayerStatsManager.Instance.onStatsUpdated += onStatsUpdated;
        onHealthUpdated(PlayerStatsManager.Instance.Health);
        onVitalityUpdated(PlayerStatsManager.Instance.Vitality);
        onDeathShardsUpdated(SpellsManager.Instance.DeathShards);
        onMaxDeathShardsUpdated(SpellsManager.Instance.MaxDeathShards);
        onStatsUpdated();
    }

    private void OnDestroy()
    {
        PlayerStatsManager.Instance.onHealthUpdated -= onHealthUpdated;
        PlayerStatsManager.Instance.onVitalityUpdated -= onVitalityUpdated;
        SpellsManager.Instance.onDeathShardsUpdated -= onDeathShardsUpdated;
        SpellsManager.Instance.onMaxDeathShardsUpdated -= onMaxDeathShardsUpdated;
        PlayerStatsManager.Instance.onStatsUpdated -= onStatsUpdated;
    }

    private void onHealthUpdated(int currentHealth)
    {
        healthGauge.setValue(currentHealth);
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

    private void onStatsUpdated()
    {
        PlayerStatsManager playerStatsManager = PlayerStatsManager.Instance;

        //Final Stats
        statsFrameVitality.text = playerStatsManager.Vitality.ToString();
        statsFrameStrength.text = playerStatsManager.Strength.ToString();
        statsFrameEndurance.text = playerStatsManager.Endurance.ToString();
        statsFrameArmor.text = playerStatsManager.Armor.ToString();
        statsFrameHitRolls.text = playerStatsManager.HitRolls.ToString();
        statsFrameScoreToHit.text = playerStatsManager.ScoreToHit.ToString();
        statsFrameBonusToWound.text = playerStatsManager.BonusToWound.ToString();

        //Modifiers
        int vitalityBonus = playerStatsManager.getEquipmentBonus(EquipmentBonus.Vitality);
        statsFrameVitalityBonus.text = "[" + ((vitalityBonus >= 0) ? "+" : "") + vitalityBonus + "]";
        statsFrameVitalityBonus.color = getBonusColor(vitalityBonus);

        int strengthBonus = playerStatsManager.getEquipmentBonus(EquipmentBonus.Strength);
        statsFrameStrengthBonus.text = "[" + ((strengthBonus >= 0) ? "+" : "") + strengthBonus + "]";
        statsFrameStrengthBonus.color = getBonusColor(strengthBonus);

        int enduranceBonus = playerStatsManager.getEquipmentBonus(EquipmentBonus.Endurance);
        statsFrameEnduranceBonus.text = "[" + ((enduranceBonus >= 0) ? "+" : "") + enduranceBonus + "]";
        statsFrameEnduranceBonus.color = getBonusColor(enduranceBonus);

        int hitRollsBonus = playerStatsManager.getEquipmentBonus(EquipmentBonus.HitRolls);
        statsFrameHitRollsBonus.text = "[" + ((hitRollsBonus >= 0) ? "+" : "") + hitRollsBonus + "]";
        statsFrameHitRollsBonus.color = getBonusColor(hitRollsBonus);

        int scoreToHitBonus = playerStatsManager.getEquipmentBonus(EquipmentBonus.ToHit);
        statsFrameScoreToHitBonus.text = "[" + ((scoreToHitBonus >= 0) ? "+" : "") + scoreToHitBonus + "]";
        statsFrameScoreToHitBonus.color = getBonusColor(scoreToHitBonus, true);
    }

    private Color getBonusColor(int value, bool lessIsMore = false)
    {
        Color bonusColor = Color.black;
        if (value > 0)
        {
            bonusColor = lessIsMore ? Color.red : Color.green;
        }
        else if (value < 0)
        {
            bonusColor = lessIsMore ? Color.green : Color.red;
        }
        return bonusColor;
    }
}
