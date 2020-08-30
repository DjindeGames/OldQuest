using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance { get; private set; }

    public int PlayerHealth
    {
        get
        {
            return playerStats.healthPoints;
        }
        private set
        {
            playerStats.healthPoints = value;
            onHealthUpdated(value);
        }
    }

    public int MaxPlayerHealth
    {
        get
        {
            return playerStats.maxHealthPoints;
        }
        private set
        {
            playerStats.maxHealthPoints = value;
            onMaxHealthUpdated(value);
        }
    }

    //Events
    public delegate void healthUpdated(int currentHealth);
    public event healthUpdated onHealthUpdated;

    public delegate void maxHealthUpdated(int currentHealth);
    public event healthUpdated onMaxHealthUpdated;

    public delegate void playerDead();
    public event playerDead onPlayerDeath;

    private PlayerStats playerStats = new PlayerStats();

    void Awake()
    {
        Instance = this;
    }

    public void addHealthPointsModifier(int modifier)
    {
        int newHealthPoints = playerStats.healthPoints + modifier;
        Mathf.Clamp(newHealthPoints, 0, playerStats.maxHealthPoints);
        PlayerHealth = newHealthPoints;
        if (PlayerHealth == 0)
        {
            onPlayerDeath();
        }
    }

    public void addMaxHealthModifier(int modifier)
    {
        int newMaxHealth = playerStats.maxHealthPoints + modifier;
        int newHealth = playerStats.healthPoints;
        Mathf.Clamp(newHealth, 0, newMaxHealth);
        PlayerHealth = newHealth;
        MaxPlayerHealth = newMaxHealth;
    }

    public void resetStats()
    {
        playerStats.healthPoints = playerStats.maxHealthPoints;
    }

    public void loadStats(PlayerStats stats)
    {
        playerStats = stats;
    }
}

public class PlayerStats
{
    public int healthPoints = 1;
    public int maxHealthPoints = 10;
}