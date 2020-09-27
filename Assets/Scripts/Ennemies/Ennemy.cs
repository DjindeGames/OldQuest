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
    private EnnemyEquipmentSlot[] equipmentSlots;
    [SerializeField]
    private GameObject corpse;

    public delegate void ennemyDeath();
    public event ennemyDeath onEnnemyDeath;

    private int health;

    private void Start()
    {
        health = stats.vitality;
        equipItems();
    }

    public bool resolveWounds(int amount)
    {
        health -= amount;
        return (health <= 0);
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
            Lootable lootable = item.prefab.GetComponent<Lootable>();
            if (lootable)
            {
                if (lootable.item.Type == ItemType.Equipment)
                {
                    Equipment equipment = (Equipment)lootable.item;
                    equipItem(equipment);
                }
            }
        }
    }

    private void equipItem(Equipment equipment)
    {
        EnnemyEquipmentSlot slot = getSlotByType(equipment.slots[0]);
        Instantiate(equipment.skin, slot.parent);
    }

    private EnnemyEquipmentSlot getSlotByType(GearSlot type)
    {
        EnnemyEquipmentSlot slot = null;
        foreach (EnnemyEquipmentSlot equipmentSlot in equipmentSlots)
        {
            if (equipmentSlot.type == type)
            {
                slot = equipmentSlot;
                break;
            }
        }
        return slot;
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
                GameObject prefab = inventoryItem.prefab;
                Lootable lootable = prefab.GetComponent<Lootable>();
                if (lootable)
                {
                    loots.Add(lootable.item);
                    GameObject instantiated = Instantiate(prefab, transform.position, Quaternion.identity);
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
    public GameObject prefab;
    [RangeAttribute(0, 100)]
    public int dropRate;
}

[System.Serializable]
public class EnnemyEquipmentSlot
{
    public GearSlot type;
    public Transform parent;
}