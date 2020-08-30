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
    private TMP_Text maxHealthPoints;

    public static PlayerStatsUI Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerStatsManager.Instance.onHealthUpdated += onHealthUpdated;
        PlayerStatsManager.Instance.onMaxHealthUpdated += onMaxHealthUpdated;
        onHealthUpdated(PlayerStatsManager.Instance.PlayerHealth);
        onMaxHealthUpdated(PlayerStatsManager.Instance.MaxPlayerHealth);
    }

    private void OnDestroy()
    {
        PlayerStatsManager.Instance.onHealthUpdated -= onHealthUpdated;
        PlayerStatsManager.Instance.onMaxHealthUpdated -= onMaxHealthUpdated;
    }

    private void onHealthUpdated(int currentHealth)
    {
        healthSlider.value = currentHealth;
        currentHealthPoints.text = currentHealth.ToString();
    }

    private void onMaxHealthUpdated(int currentMaxHealth)
    {
        healthSlider.maxValue = currentMaxHealth;
        maxHealthPoints.text = currentMaxHealth.ToString();
    }
}
