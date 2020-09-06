using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private TMP_Text currentHealthPoints;
    [SerializeField]
    private TMP_Text currentVitality;
    [SerializeField]
    private TMP_Text statsFrameHealth;
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
        PlayerStatsManager.Instance.onStatsUpdated += onStatsUpdated;
        onHealthUpdated(PlayerStatsManager.Instance.Health);
        onVitalityUpdated(PlayerStatsManager.Instance.Vitality);
        onStatsUpdated();
    }

    private void OnDestroy()
    {
        PlayerStatsManager.Instance.onHealthUpdated -= onHealthUpdated;
        PlayerStatsManager.Instance.onVitalityUpdated -= onVitalityUpdated;
    }

    private void onHealthUpdated(int currentHealth)
    {
        healthSlider.value = currentHealth;
        currentHealthPoints.text = currentHealth.ToString();
        statsFrameHealth.text = currentHealth.ToString();
    }

    private void onVitalityUpdated(int vitality)
    {
        healthSlider.maxValue = vitality;
        currentVitality.text = vitality.ToString();
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
        int strengthBonus = playerStatsManager.getEquipmentBonus(EquipmentBonus.Strength);
        statsFrameStrengthBonus.text = "[" + ((strengthBonus >= 0) ? "+" : "") + strengthBonus + "]";
        int enduranceBonus = playerStatsManager.getEquipmentBonus(EquipmentBonus.Endurance);
        statsFrameEnduranceBonus.text = "[" + ((enduranceBonus >= 0) ? "+" : "") + enduranceBonus + "]";
        int hitRollsBonus = playerStatsManager.getEquipmentBonus(EquipmentBonus.HitRolls);
        statsFrameHitRollsBonus.text = "[" + ((hitRollsBonus >= 0) ? "+" : "") + hitRollsBonus + "]";
        int scoreToHitBonus = playerStatsManager.getEquipmentBonus(EquipmentBonus.ToHit);
        statsFrameScoreToHitBonus.text = "[" + ((scoreToHitBonus >= 0) ? "+" : "") + scoreToHitBonus + "]";
    }
}
