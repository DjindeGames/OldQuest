using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    #region Exposed Attributes

    [Header("References")]
    [SerializeField]
    private PlayerCharacterStats _playerStats;

    #endregion

    #region Attributes

    public static PlayerStatsManager _Instance { get; private set; }
    public PlayerCharacterStats _PlayerStats
    {
        get
        {
            return _playerStats;
        }
    }

    #endregion

    #region MonoBehaviour Methods

    private void Awake()
    {
        _Instance = this;
    }

    #endregion
}