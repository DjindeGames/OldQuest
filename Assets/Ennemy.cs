using UnityEngine;

public class Ennemy : MonoBehaviour
{
    [Header("Stats")]
    public CharacterStats stats;
    [SerializeField]
    private EnnemyInventoryItem[] inventoryContent;
    [Header("References")]
    [SerializeField]
    private EnnemyEquipmentSlot[] equipmentSlots;

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
            if (item.item.Type == ItemType.Equipment)
            {
                Equipment equipment = (Equipment)item.item;
                equipItem(equipment);
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
}

[System.Serializable]
public class EnnemyInventoryItem
{
    public Item item;
    [RangeAttribute(0, 100)]
    public int dropRate;
}

[System.Serializable]
public class EnnemyEquipmentSlot
{
    public GearSlot type;
    public Transform parent;
}