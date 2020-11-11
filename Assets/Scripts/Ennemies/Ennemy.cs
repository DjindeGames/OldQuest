using UnityEngine;
using System.Collections.Generic;
using Djinde.Utils;

namespace Djinde.Quest
{
    public class Ennemy : MonoBehaviour
    {
        #region Events



        #endregion

        #region Exposed Attributes

        [Header("Stats")]
        [SerializeField]
        private EnnemyInventoryItem[] inventoryContent;
        [Header("References")]
        [SerializeField]
        private GameObject corpse;

        #endregion

        #region Attributes

        public CharacterStats _Stats
        {
            get
            {
                return _characterStats;
            }
        }

        private EquipmentHolder _equipmentHolder;
        private CharacterStats _characterStats;

        #endregion

        #region MonoBehaviour Methods

        private void Start()
        {
            _equipmentHolder = GetComponent<EquipmentHolder>();
            _characterStats = GetComponent<CharacterStats>();
            if (_characterStats == null)
            {
                Tools.LogWarning(this, "No CharacterStats on " + name);
            }
            if (_equipmentHolder != null)
            {
                equipItems();
            }
            else
            {
                Tools.LogWarning(this, "No EquipmentHolder on " + name);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                engageCombat();
            }
        }

        #endregion

        #region Private Methods

        private void engageCombat()
        {
            CombatManager.Instance.startCombat(this);
        }

        private void equipItems()
        {
            foreach (EnnemyInventoryItem item in inventoryContent)
            {
                if (item._equipped && item._applyBonuses && Tools.TryCast(item.item.item, out Equipment equipment))
                {
                    _equipmentHolder.TryToEquip(item.item);
                }
            }
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

        #endregion

        #region Protected Methods



        #endregion

        #region Public Methods

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

        #endregion

        [System.Serializable]
        private class EnnemyInventoryItem
        {
            public Lootable item;
            public bool _equipped = true;
            public bool _applyBonuses = true;
            [RangeAttribute(0, 100)]
            public int dropRate;
        }
    }
}
