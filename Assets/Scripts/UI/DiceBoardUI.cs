using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;

public class DiceBoardUI : MonoBehaviour
{
    [BoxGroup("References")]
    [Header("Action Results")]
    [SerializeField]
    private GameObject actionResultWindow;
    [BoxGroup("References")]
    [SerializeField]
    private TMP_Text textResult;
    [BoxGroup("References")]
    [Header("Loot Results")]
    [SerializeField]
    private GameObject lootsResultWindow;
    [SerializeField]
    [BoxGroup("References")]
    private Transform lootsParent;
    [SerializeField]
    [BoxGroup("References")]
    private GameObject lootPrefab;
    [SerializeField]
    [BoxGroup("References")]
    private Button lootsResultWindowConfirm;

    public delegate void actionAknowledged();
    public event actionAknowledged onActionAknowledged;

    public static DiceBoardUI Instance { get; private set; }
    private int totalLoots;
    private int lootsRevealed;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        lootsResultWindow.SetActive(false);
        actionResultWindow.SetActive(false);
    }

    public void close()
    {
        lootsResultWindow.SetActive(false);
        ScreenManager.Instance.switchToPreviousScreen();
    }

    public void acknowledge()
    {
        actionResultWindow.SetActive(false);
        onActionAknowledged?.Invoke();
    }

    public void showLootResults(List<Item> loots)
    {
        resetLootList();
        totalLoots = loots.Count;
        lootsResultWindow.SetActive(true);
        foreach (Item item in loots)
        {
            GameObject instantiated = Instantiate(lootPrefab, lootsParent);
            LootResult lootResult = instantiated.GetComponent<LootResult>();
            lootResult.setData(item);
        }
    }

    private void resetLootList()
    {
        totalLoots = 0;
        lootsRevealed = 0;
        lootsResultWindowConfirm.interactable = false;
        for (int i = 0; i < lootsParent.childCount; i++)
        {
            Destroy(lootsParent.GetChild(0).gameObject);
        }
    }

    public void notifyLootRevealed()
    {
        lootsRevealed++;
        if (lootsRevealed == totalLoots)
        {
            lootsResultWindowConfirm.interactable = true;
        }
    }

    public void showActionResult(ThrowActionType actionType, int result)
    {
        switch(actionType)
        {
            case (ThrowActionType.PlayerHit):
                if (result == 0)
                {
                    textResult.text = "You have missed all your attacks.";
                }
                else
                {
                    textResult.text = "You have hit the ennemy " + result + " times.";
                }
                break;
            case (ThrowActionType.PlayerWound):
                if (result == 0)
                {
                    textResult.text = "You have not managed to wound the ennemy.";
                }
                else
                {
                    textResult.text = "You have wounded the ennemy " + result + " times for a total of " + PlayerStatsManager.Instance.Damages * result + " points of damages.";
                }
                break;
            case (ThrowActionType.EnnemyHit):
                if (result == 0)
                {
                    textResult.text = "The ennemy missed all of his attacks.";
                }
                else
                {
                    textResult.text = "The ennemy has hit you " + result + " times.";
                }
                break;
            case (ThrowActionType.EnnemyWound):
                if (result == 0)
                {
                    textResult.text = "The ennemy has not managed to wound you.";
                }
                else
                {
                    textResult.text = "The ennemy has wounded you " + result + " times for a total of " + CombatManager.Instance.CurrentEnnemy.stats.damages * result + " points of damages.";
                }
                break;
        }
        actionResultWindow.SetActive(true);
    }
}
