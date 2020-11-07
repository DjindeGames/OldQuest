using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using System.Collections;

public class DiceBoardUI : MonoBehaviour
{
    /**************************************************/
    /************* Action Results Window **************/
    /**************************************************/
    [BoxGroup("References")]
    [Header("Action Results")]
    [SerializeField]
    private GameObject actionResultWindow;
    [BoxGroup("References")]
    [SerializeField]
    private TMP_Text textResult;
    /**************************************************/
    /************** Loot Results Window ***************/
    /**************************************************/
    [BoxGroup("References")]
    [Header("Loot Results")]
    [SerializeField]
    private GameObject lootsResultWindow;
    [SerializeField]
    [BoxGroup("References")]
    private Button lootsResultWindowConfirm;
    [SerializeField]
    [BoxGroup("References")]
    private SelectableList_Loot _lootList;
    /**************************************************/
    /******************* Menu Tabs ********************/
    /**************************************************/
    [BoxGroup("References")]
    [Header("Menu Tabs")]
    [SerializeField]
    private DiceBoardMenuTab[] menuTabs;
    [BoxGroup("References")]
    [SerializeField]
    private GameObject navigationBar;
    [BoxGroup("References")]
    [SerializeField]
    private Color selectedTabColor;
    [BoxGroup("References")]
    [SerializeField]
    private Color idleTabColor;
   
    /**************************************************/
    /****************** Consumables *******************/
    /**************************************************/
    [Header("Consumables")]
    [BoxGroup("References")]
    [SerializeField]
    private Transform consumablesParents;
    [BoxGroup("References")]
    [SerializeField]
    private Transform currentConsumableName;
    [BoxGroup("References")]
    [SerializeField]
    private Transform currentConsumableDescription;
    [BoxGroup("References")]
    [SerializeField]
    private Button useButton;

    public delegate void actionAknowledged();
    public event actionAknowledged onActionAknowledged;

    public static DiceBoardUI Instance { get; private set; }

    public bool IsWaitingForAcknowledgement { get; private set; } = false;

    

    private void Awake()
    {
        Instance = this;
        _lootList.OnLootsFullyRevealedEvent += OnLootsFullyRevealed;
    }

    private void OnDestroy()
    {
        _lootList.OnLootsFullyRevealedEvent -= OnLootsFullyRevealed;
    }

    void Start()
    {
        lootsResultWindow.SetActive(false);
        actionResultWindow.SetActive(false);
        StartCoroutine(loadTabs());
    }

    public void close()
    {
        lootsResultWindow.SetActive(false);
        ScreenManager.Instance.switchToPreviousScreen();
    }

    public void acknowledge()
    {
        IsWaitingForAcknowledgement = false;
        actionResultWindow.SetActive(false);
        onActionAknowledged?.Invoke();
    }

    public void showLootResults(List<Item> loots)
    {
        closeTabs();
        _lootList.ClearList();
        lootsResultWindowConfirm.interactable = false;
        lootsResultWindow.SetActive(true);
        foreach (Item item in loots)
        {
            SelectableListItem_LootData data = new SelectableListItem_LootData();
            data._label = item.description;
            data._outlineColor = GameConstants.Instance.getLootableRarityColorByType(item.rarity);
            _lootList.AddItem(data);
        }
    }

    private void OnLootsFullyRevealed()
    {
        lootsResultWindowConfirm.interactable = true;
    }

    public void showActionResult(EThrowActionType actionType, int result)
    {
        closeTabs();
        switch(actionType)
        {
            case (EThrowActionType.PlayerHit):
                if (result == 0)
                {
                    textResult.text = "You have missed all your attacks.";
                }
                else
                {
                    textResult.text = "You have hit the ennemy " + result + " times.";
                }
                break;
            case (EThrowActionType.PlayerWound):
                if (result == 0)
                {
                    textResult.text = "You have not managed to wound the ennemy.";
                }
                else
                {
                    textResult.text = "You have wounded the ennemy " + result + " times for a total of " + PlayerFastAccess._CharacterStats.GetPassiveStatOfType(EPassiveStatType.Damages) * result + " points of damages.";
                }
                break;
            case (EThrowActionType.EnnemyHit):
                if (result == 0)
                {
                    textResult.text = "The ennemy missed all of his attacks.";
                }
                else
                {
                    textResult.text = "The ennemy has hit you " + result + " times.";
                }
                break;
            case (EThrowActionType.EnnemyWound):
                if (result == 0)
                {
                    textResult.text = "The ennemy has not managed to wound you.";
                }
                else
                {
                    textResult.text = "The ennemy has wounded you " + result + " times for a total of " + CombatManager.Instance.CurrentEnnemy.stats.GetPassiveStatOfType(EPassiveStatType.Damages) * result + " points of damages.";
                }
                break;
        }
        actionResultWindow.SetActive(true);
        IsWaitingForAcknowledgement = true;
    }

    public void displayTab(EDiceBoardMenuTab which, bool playSFX = true)
    {
        foreach(DiceBoardMenuTab menuTab in menuTabs)
        {
            bool selected = (menuTab.tabType == which) || (which == EDiceBoardMenuTab.All);
            menuTab.tabContent.SetActive(selected);
            menuTab.tab.color = selected ? selectedTabColor : idleTabColor;
            if (playSFX)
            {
                if (which != EDiceBoardMenuTab.None)
                {
                    SoundManager.Instance.playSFX(ESFXType.OpenReadable);
                }
                else
                {
                    SoundManager.Instance.playSFX(ESFXType.CloseReadable);
                }
            }
        }
    }

    public void displaySpells()
    {
        displayTab(EDiceBoardMenuTab.SpellBook);
    }

    public void displayConsumables()
    {
        displayTab(EDiceBoardMenuTab.Consumables);
    }

    public void displayStats()
    {
        displayTab(EDiceBoardMenuTab.Stats);
    }

    public void closeTabs()
    {
        displayTab(EDiceBoardMenuTab.None);
    }

    private IEnumerator loadTabs()
    {
        displayTab(EDiceBoardMenuTab.All, false);
        yield return new WaitForEndOfFrame();
        displayTab(EDiceBoardMenuTab.None, false);
    }
}

[System.Serializable]
public class DiceBoardMenuTab
{
    public GameObject tabContent;
    public Image tab;
    public EDiceBoardMenuTab tabType;
}