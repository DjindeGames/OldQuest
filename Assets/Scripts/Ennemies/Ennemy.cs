using UnityEngine;
using System.Collections.Generic;
public class Ennemy : MonoBehaviour
{
    [Header("Stats")]
    public CharacterStats stats;
    [SerializeField]
    private EnnemyInventoryItem[] inventoryContent;
    [Header("References")]
    [SerializeField]
    private GameObject corpse;

    private EquipmentHolder _equipmentHolder;

    public delegate void ennemyDeath();
    public event ennemyDeath onEnnemyDeath;

    private void Start()
    {
        _equipmentHolder = GetComponent<EquipmentHolder>();
        if (_equipmentHolder != null)
        {
            equipItems();
        }
        else
        {
            Utils.LogWarning(this, "No EquipmentHolder on " + name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            engageCombat();
        }
    }

    private void engageCombat()
    {
        CombatManager.Instance.startCombat(this);
    }

    private void equipItems()
    {
        foreach (EnnemyInventoryItem item in inventoryContent)
        {
            if (item._equipped && item._applyBonuses && Utils.TryCast(item.item.item, out Equipment equipment))
            {
                _equipmentHolder.TryToEquip(item.item);
            }
        }
    }

    public void onDeath()
    {
        StateSave stateSave = GetComponent<StateSave>();
        if (stateSave != null)
        {
            SaveManager.Instance.addKilledEnnemy(stateSave.id);
        }
        dispatchLoot();
    }

    public void forceUnspawn()
    {
        spawnCorpse();
        Destroy(gameObject);
    }

    private void dispatchLoot()
    {
        List<Item> loots = new List<Item>();
        foreach (EnnemyInventoryItem inventoryItem in inventoryContent)
        {
            if (Random.Range(1, 101) <= inventoryItem.dropRate)
            {
                Lootable lootable = inventoryItem.item;
                if (lootable)
                {
                    loots.Add(lootable.item);
                    GameObject instantiated = Instantiate(lootable.gameObject, transform.position, Quaternion.identity);
                    Destroy(instantiated.GetComponent<StateSave>());
                    SaveManager.Instance.addSpawnedItem(instantiated);
                }
            }
        }
        DiceBoardUI.Instance.showLootResults(loots);
        spawnCorpse();
        Destroy(gameObject);
    }

    private void spawnCorpse()
    {
        Instantiate(corpse, transform.position, transform.rotation);
    }
}

[System.Serializable]
public class EnnemyInventoryItem
{
    public Lootable item;
    public bool _equipped = true;
    public bool _applyBonuses = true;
    [RangeAttribute(0, 100)]
    public int dropRate;
}