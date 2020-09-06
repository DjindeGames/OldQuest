using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]
    [Range(1,20)]
    private int inventorySlots;
    [Header("References")]
    [SerializeField]
    private GameObject inventoryTable;
    [SerializeField]
    private InventoryKey[] keys;
    [SerializeField]
    private EquipmentSlot[] equipmentSlots;
    [SerializeField]
    private PuppetSlot[] puppetSlots;

    private int goldAmount = 0;
    private int oilAmount = 0;

    public int GoldAmount
    {
        get
        {
            return goldAmount;
        }
        set
        {
            goldAmount = value;
            InventoryUI.Instance.setGoldAmount(goldAmount);
        }
    }
    public int OilAmount
    {
        get
        {
            return oilAmount;
        }
        set
        {
            oilAmount = value;
            InventoryUI.Instance.setOilAmount(oilAmount);
        }
    }

    public static InventoryManager Instance { get; private set; }

    public List<Tuple<GameObject,bool>> inventoryContent = new List<Tuple<GameObject, bool>>();
    public List<ReadableKey> archives = new List<ReadableKey>();

    public Collider TableCollider { get; private set; }
    
    void Awake()
    {
        Instance = this;
        hideKeys();
        TableCollider = (Collider)inventoryTable.GetComponents(typeof(Collider))[0];
    }

    public bool hasKey(KeyMaterial material)
    {
        return getKeyByMaterial(material).isInInventory;
    }

    public void loot(Lootable looted)
    {
        bool requiresSlot = false;
        bool success = true;
        Item item = looted.item;
        switch (item.Type)
        {
            case ItemType.Valuable:
                SoundPlayer.Instance.playLootSound(item.Type);
                retrieveSurroundingValuables(looted);
                return;
            case ItemType.Oil:
                OilAmount++;
                break;
            case ItemType.Key:
                addKey(((Key)item).material);
                break;
            case ItemType.Readable:
                //ReadableUI.Instance.open(((Readable)item).contentKey);
                addFileToArchives(((Readable)item).contentKey);
                ScreenManager.Instance.switchScreen(ScreenType.Archives);
                break;
            default:
                success = (inventoryContent.Count < inventorySlots);
                requiresSlot = true;
                break;
        }
        if (!requiresSlot)
        {
            Destroy(looted.gameObject);
        }
        else if (success)
        {
            addItem(looted.gameObject);
        }
        if (success)
        {
            MainUI.Instance.hideDescription();
            writeLootLog(item.description.ToLower());
            SoundPlayer.Instance.playLootSound(looted.item.Type);
            StateSave stateSave = looted.GetComponent<StateSave>();
            if (stateSave)
            {
                SaveManager.Instance.addDestroyedItem(stateSave.id);
            }
        }
        else
        {
            MainUI.Instance.writeLog("Inventory is full!");
        }
    }

    public void removeItem(GameObject item, bool destroy = false)
    {
        Tuple<GameObject, bool> itemToRemove = retrieveItem(item);
        inventoryContent.Remove(itemToRemove);
        Destroy(item);
    }

    public void addFileToArchives(ReadableKey key, bool withSelection = true)
    {
        archives.Add(key);
        ArchivesUI.Instance.addFile(key, withSelection);
    }

    public bool hasOil()
    {
        bool has = true;
        if (OilAmount > 0)
        {
            OilAmount--;
        }
        else
        {
            has = false;
        }
        return has;
    }

    private void retrieveSurroundingValuables(Lootable pickedValuable)
    {
        int goldFound = 0;
        RaycastHit[] hits = Physics.SphereCastAll(pickedValuable.transform.position, 0.5f, Vector3.up, 0.1f, LayerMask.GetMask("Valuable"));
        
        for (int i = 0; i < hits.Length; i++)
        {
            Lootable looted = hits[i].collider.GetComponent<Lootable>();
            if (looted && looted.item.Type == ItemType.Valuable)
            {
                goldFound += ((Valuable)looted.item).value;
                StateSave stateSave = looted.GetComponent<StateSave>();
                if (stateSave)
                {
                    SaveManager.Instance.addDestroyedItem(stateSave.id);
                }
                Destroy(looted.gameObject);
            }
        }
        GoldAmount += goldFound;
        writeLootLog(goldFound.ToString() + " gold coins.");
    }

    private void writeLootLog(string log)
    {
        MainUI.Instance.writeLog("Picked up " + log);
    }

    public void addItem(GameObject item, bool equipped = false, bool restored = false)
    {
        inventoryContent.Add(new Tuple<GameObject, bool>(item.gameObject, equipped));
        Vector3 newPosition = new Vector3(inventoryTable.transform.position.x, inventoryTable.transform.position.y + 2, inventoryTable.transform.position.z);
        item.transform.position = newPosition;
        item.GetComponent<ItemPhysics>().resetStartPosition(newPosition);
        item.AddComponent<InventoryItem>();
        if (equipped)
        {
            useItem(item.GetComponent<InventoryItem>(), restored);
        }
    }

    public void addKey(KeyMaterial material)
    {
        InventoryKey key = getKeyByMaterial(material);
        key.addToInventory();
    }

    public void useItem(InventoryItem inventoryItem, bool restored = false)
    {
        Item item = inventoryItem.GetComponent<Lootable>().item;
        switch (item.Type)
        {
            case (ItemType.Equipment):
                Equipment equipment = ((Equipment)item);
                StateSave stateSave = inventoryItem.GetComponent<StateSave>();
                for (int i = 0; i < equipment.slots.Length; i++)
                {
                    bool slotsRemaining = (i < equipment.slots.Length - 1) ? true : false;
                    
                    if (isAlreadyEquipped(inventoryItem, out EquipmentSlot equippedSlot))
                    {
                        equippedSlot.unequip();
                        PuppetSlot pupperSlot = getPuppetSlot(equippedSlot.type);
                        Destroy(equippedSlot.parent.GetChild(0).gameObject);
                        Destroy(pupperSlot.parent.GetChild(0).gameObject);
                        updateEquipmentState(inventoryItem.gameObject, false);
                        updateEquipmentBonuses(equipment, false);
                        if (!restored)
                        {
                            writeEquipmentLog(0, equippedSlot.type, inventoryItem.LinkedItem.name);
                            SoundPlayer.Instance.playSFX(SFXType.ItemEquipped);
                        }
                        break;
                    }
                    else
                    {
                        EquipmentSlot associatedSlot = getEquipmentSlot(((Equipment)item).slots[i]);
                        if (!associatedSlot.isUsed)
                        {
                            associatedSlot.equip(inventoryItem);
                            GameObject skin = Resources.Load<GameObject>(Constants.skinsPath + inventoryItem.name.Split('$')[0]);
                            PuppetSlot pupperSlot = getPuppetSlot(associatedSlot.type);
                            Instantiate(skin, associatedSlot.parent);
                            Instantiate(skin, pupperSlot.parent);
                            updateEquipmentState(inventoryItem.gameObject, true);
                            updateEquipmentBonuses(equipment, true);
                            if (!restored)
                            {
                                writeEquipmentLog(1, equipment.slots[i], inventoryItem.LinkedItem.name);
                                SoundPlayer.Instance.playSFX(SFXType.ItemEquipped);
                            }
                            break;
                        }
                        else if (!slotsRemaining)
                        {
                            writeEquipmentLog(-1, equipment.slots[i], inventoryItem.LinkedItem.name);
                        }
                    }
                }
                break;
            case (ItemType.Potion):
                usePotion((Potion)item);
                removeItem(inventoryItem.gameObject);
                break;
        }
    }

    private void updateEquipmentBonuses(Equipment equipment, bool apply)
    {
        for (int i = 0; i < equipment.stats.Length; i++)
        {
            if (apply)
            {
                PlayerStatsManager.Instance.applyEquipmentBonus(equipment.stats[i]);
            }
            else
            {
                PlayerStatsManager.Instance.unapplyEquipmentBonus(equipment.stats[i]);
            }
        }
    }

    private void usePotion(Potion potion)
    {
        DiceActionManager.Instance.performThrowAction(ThrowActionType.HealingPotion, potion.value, 0);
    }

    private void writeEquipmentLog(int equipped, GearSlot slot, string equipmentName)
    {
        string formattedSlot = slot.ToString().ToLower();
        switch(slot)
        {
            case (GearSlot.LeftHand):
                formattedSlot = "left hand";
                break;
            case (GearSlot.RightHand):
                formattedSlot = "right hand";
                break;
        }
        switch(equipped)
        {
            case (-1):
                MainUI.Instance.writeLog("Cannot equip the " + equipmentName.ToLower() + ": " + formattedSlot + " slot already used.");
                break;
            case (0):
                MainUI.Instance.writeLog("Unequipped " + formattedSlot + " of the " + equipmentName.ToLower() + ".");
                break;
            case (1):
                MainUI.Instance.writeLog("Equipped " + formattedSlot + " with the " + equipmentName.ToLower() + ".");
                break;
        }
    }

    private void updateEquipmentState(GameObject reference, bool value)
    {
        Tuple<GameObject, bool> tuple = retrieveItem(reference);
        inventoryContent.Remove(tuple);
        inventoryContent.Add(new Tuple<GameObject, bool>(reference, value));
    }

    private Tuple<GameObject, bool> retrieveItem(GameObject reference)
    {
        Tuple<GameObject, bool> tuple = new Tuple<GameObject, bool>(null, false);
        foreach(Tuple<GameObject,bool> t in inventoryContent)
        {
            if (t.Item1 == reference)
            {
                tuple = t;
                break;
            }
        }
        return tuple;
    }

    private EquipmentSlot getEquipmentSlot(GearSlot which)
    {
        EquipmentSlot slot = null;
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].type == which)
            {
                slot = equipmentSlots[i];
                break;
            }
        }
        return slot;
    }

    private PuppetSlot getPuppetSlot(GearSlot which)
    {
        PuppetSlot slot = null;
        for (int i = 0; i < puppetSlots.Length; i++)
        {
            if (equipmentSlots[i].type == which)
            {
                slot = puppetSlots[i];
                break;
            }
        }
        return slot;
    }

    private bool isAlreadyEquipped(InventoryItem item, out EquipmentSlot associatedSlot)
    {
        bool alreadyEquipped = false;
        associatedSlot = null;
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].isUsed && equipmentSlots[i].currentlyEquipped == item)
            {
                alreadyEquipped = true;
                associatedSlot = equipmentSlots[i];
                break;
            }
        }
        return alreadyEquipped;
    }

    public InventoryKey getKeyByMaterial(KeyMaterial material)
    {
        InventoryKey foundKey = null;
        for (int i = 0; i < keys.Length; i++)
        {
            if (keys[i].material == material)
            {
                foundKey = keys[i];
                break;
            }
        }
        return foundKey;
    }

    private void hideKeys()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].hide();
        }
    }
}

[System.Serializable]
public class EquipmentSlot
{
    public GearSlot type;
    public Transform parent;
    public bool isUsed = false;
    public InventoryItem currentlyEquipped;

    public void equip(InventoryItem equipped)
    {
        isUsed = true;
        currentlyEquipped = equipped;
    }

    public void unequip()
    {
        isUsed = false;
        currentlyEquipped = null;
    }
}

[System.Serializable]
public class PuppetSlot
{
    public GearSlot type;
    public Transform parent;
}

[System.Serializable]
public class InventoryKey
{
    public GameObject key;
    public KeyMaterial material;
    public bool isInInventory = false;

    public void show()
    {
        key.SetActive(true);
    }

    public void hide()
    {
        key.SetActive(false);
    }

    public void addToInventory()
    {
        isInInventory = true;
        show();
    }
}
