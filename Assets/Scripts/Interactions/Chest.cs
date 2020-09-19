using UnityEngine;
using System.Collections.Generic;

public class Chest : Highlightable
{
    [Header("References")]
    [SerializeField]
    GameObject openedChestPrefab;
    [SerializeField]
    LootByScore[] loots;
    [Header("Parameters")]
    [SerializeField]
    float baseLootVerticalOffset = 0.3f;
    [SerializeField]
    float iterativeLootVerticalOffset = 0.3f;


    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
    }

    protected override void activate()
    {
        DiceActionManager.Instance.performThrowAction(ThrowActionType.LootChest, 6, 0);
        DiceActionManager.Instance.onActionPerformed += onThrowComplete;
    }

    private void onThrowComplete(ThrowActionType actionType, int score)
    {
        if (actionType == ThrowActionType.LootChest)
        {
            DiceActionManager.Instance.onActionPerformed -= onThrowComplete;
            StateSave stateSave = GetComponent<StateSave>();
            if (stateSave)
            {
                SaveManager.Instance.addOpenedChest(stateSave.id);
            }
            openChest(score);
        }
    }

    private void dispatchLoot(int score)
    {
        List<Item> lootedItems = new List<Item>();
        for (int i = 0; i < loots.Length; i++)
        {
            if (loots[i].requiredScore <= score)
            {
                Vector3 instancePosition = new Vector3(transform.position.x, transform.position.y + baseLootVerticalOffset, transform.position.z);
                GameObject instantiated = Instantiate(loots[i].loot, instancePosition, Quaternion.identity);
                Destroy(instantiated.GetComponent<StateSave>());
                SaveManager.Instance.addSpawnedItem(instantiated);
                baseLootVerticalOffset += iterativeLootVerticalOffset;
                lootedItems.Add(instantiated.GetComponent<Lootable>().item);
            }
        }
        DiceBoardUI.Instance.showLootResults(lootedItems);
        SoundManager.Instance.playSFX(SFXType.OpenedChest);
    }

    private void openChest(int score, bool forced = false)
    {
        if (!forced)
        {
            dispatchLoot(score);
        }
        Instantiate(openedChestPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public void forceOpen()
    {
        openChest(-1, true);
    }
}

[System.Serializable]
public class LootByScore
{
    [RangeAttribute(6,36)]
    public int requiredScore;
    public GameObject loot;
}
