using System;
using System.Collections.Generic;
using UnityEngine;

namespace Djinde.Quest
{
    public class InventoryManager : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField]
        [Range(1, 20)]
        private int inventorySlots;
        [Header("References")]
        [SerializeField]
        private GameObject inventoryTable;
        [SerializeField]
        private InventoryKey[] keys;

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

        public List<Tuple<GameObject, bool>> inventoryContent = new List<Tuple<GameObject, bool>>();
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
                case EItemType.Valuable:
                    SoundManager.Instance.playLootSound(item.Type);
                    retrieveSurroundingValuables(looted);
                    return;
                case EItemType.Oil:
                    OilAmount++;
                    break;
                case EItemType.Key:
                    addKey(((Key)item).material);
                    break;
                case EItemType.Readable:
                    //ReadableUI.Instance.open(((Readable)item).contentKey);
                    addFileToArchives(((Readable)item).contentKey);
                    ScreenManager.Instance.switchScreen(EScreenType.Archives);
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
                SoundManager.Instance.playLootSound(looted.item.Type);
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

        public void removeItem(InventoryItem inventoryItem, bool destroy = false)
        {

            if (/*isAlreadyEquipped(inventoryItem, out EquipmentSlot slot)*/true)
            {
                //writeEquipmentLog(-2, slot.type, inventoryItem.LinkedItem.name);
                MainUI.Instance.writeLog("Cannot remove the " + inventoryItem.LinkedItem.label + " from inventory: unequip it first.");
            }
            else
            {
                GameObject item = inventoryItem.gameObject;
                Tuple<GameObject, bool> itemToRemove = retrieveItem(item);

                inventoryContent.Remove(itemToRemove);
                //Item is manually removed by player
                if (!destroy)
                {
                    Destroy(inventoryItem);

                    Vector3 playerPosition = PlayerController.Instance.PlayerPosition;
                    item.transform.position = new Vector3(playerPosition.x, playerPosition.y + 1.0f, playerPosition.z);

                    SaveManager.Instance.addSpawnedItem(item);

                    SoundManager.Instance.playSFX(ESFXType.ItemRemoved);
                    MainUI.Instance.writeLog(inventoryItem.LinkedItem.label + " removed from inventory.");
                }
                //Item is consumed
                else
                {
                    Destroy(item);
                }
            }
        }

        private void updateEquipmentState(GameObject reference, bool value)
        {
            Tuple<GameObject, bool> tuple = retrieveItem(reference);
            inventoryContent.Remove(tuple);
            inventoryContent.Add(new Tuple<GameObject, bool>(reference, value));
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
                if (looted && looted.item.Type == EItemType.Valuable)
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
            Destroy(item.GetComponent<StateSave>());
            SaveManager.Instance.tryRemovedSpawnedItem(item);
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
                case (EItemType.Equipment):
                    UseEquipment(inventoryItem);
                    break;
                case (EItemType.Potion):
                    Potion potion = (Potion)item;
                    if (potion.type == EPotionType.Health && PlayerFastAccess._CharacterStats._HasFullHealth)
                    {
                        MainUI.Instance.writeLog("It would be a waste!");
                    }
                    else
                    {
                        usePotion(potion);
                        removeItem(inventoryItem, true);
                    }
                    break;
            }
        }

        private void UseEquipment(InventoryItem item)
        {
            switch (PlayerFastAccess._EquipmentHolder.TryToEquip(item.GetComponent<Lootable>()))
            {
                case (EEquipResult.Equipped):
                    updateEquipmentState(item.gameObject, true);
                    break;
                case (EEquipResult.Unequipped):
                    updateEquipmentState(item.gameObject, false);
                    break;
                case (EEquipResult.Failed):
                    break;
            }
        }

        private void usePotion(Potion potion)
        {
            DiceActionManager.Instance.performThrowAction(EThrowActionType.HealingPotion, potion.value, 0);
        }

        private void writeEquipmentLog(int equipped, EGearSlotType slot, string equipmentName)
        {
            string formattedSlot = slot.ToString().ToLower();
            switch (slot)
            {
                case (EGearSlotType.LeftHand):
                    formattedSlot = "left hand";
                    break;
                case (EGearSlotType.RightHand):
                    formattedSlot = "right hand";
                    break;
            }
            switch (equipped)
            {
                case (-2):
                    MainUI.Instance.writeLog("Cannot remove the " + equipmentName.ToLower() + ": " + formattedSlot + " is equipped with it.");
                    break;
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



        private Tuple<GameObject, bool> retrieveItem(GameObject reference)
        {
            Tuple<GameObject, bool> tuple = new Tuple<GameObject, bool>(null, false);
            foreach (Tuple<GameObject, bool> t in inventoryContent)
            {
                if (t.Item1 == reference)
                {
                    tuple = t;
                    break;
                }
            }
            return tuple;
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
}