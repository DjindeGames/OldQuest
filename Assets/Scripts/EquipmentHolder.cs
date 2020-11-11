using UnityEngine;
using Djinde.Utils;
using Djinde.Quest;

public class EquipmentHolder : MonoBehaviour
{
    #region Events



    #endregion

    #region Exposed Attributes
    [Header("Parameters")]
    [SerializeField]
    private bool _isSilent = true;
    [Header("References")]
    [SerializeField]
    private EquipmentSlot[] _equipmentSlots;
    [SerializeField]
    private PuppetSlot[] _puppetSlots;

    #endregion

    #region Attributes

    private CharacterStats _characterStats;

    #endregion

    #region MonoBehaviour Methods



    #endregion

    #region Private Methods

    private void Awake()
    {
        _characterStats = GetComponent<CharacterStats>();
        if (_characterStats == null)
        {
            Tools.LogError(this, "No CharacterStats Component on GameObject " + name);
        }
    }

    private EquipmentSlot GetEquipmentSlot(PuppetSlot associatedSlot)
    {
        return GetEquipmentSlot(associatedSlot);
    }

    private EquipmentSlot GetEquipmentSlot(EGearSlotType which)
    {
        EquipmentSlot slot = null;
        for (int i = 0; i < _equipmentSlots.Length; i++)
        {
            if (_equipmentSlots[i]._type == which)
            {
                slot = _equipmentSlots[i];
                break;
            }
        }
        return slot;
    }

    private PuppetSlot GetPuppetSlot(EquipmentSlot associatedSlot)
    {
        return GetPuppetSlot(associatedSlot._type);
    }

    private PuppetSlot GetPuppetSlot(EGearSlotType which)
    {
        PuppetSlot slot = null;
        for (int i = 0; i < _puppetSlots.Length; i++)
        {
            if (_equipmentSlots[i]._type == which)
            {
                slot = _puppetSlots[i];
                break;
            }
        }
        return slot;
    }

    private bool IsAlreadyEquipped(Lootable item, out EquipmentSlot associatedSlot)
    {
        bool alreadyEquipped = false;
        associatedSlot = null;
        if (Tools.TryCast(item.item, out Equipment equipment))
        {
            foreach (EGearSlotType slotType in equipment.slots)
            {
                EquipmentSlot equipmentSlot = GetEquipmentSlot(slotType);
                if (equipmentSlot._isUsed && equipmentSlot._currentlyEquipped == item)
                {
                    alreadyEquipped = true;
                    associatedSlot = equipmentSlot;
                    break;
                }
            }
        }
        return alreadyEquipped;
    }

    private void RegisterEquipmentBonuses(Equipment equipment)
    {
        foreach (PassiveBonus passiveBonus in equipment._passiveBonuses)
        {
            _characterStats.RegisterPassiveBonus(passiveBonus);
        }
        foreach (ActiveBonus activeBonus in equipment._activeBonuses)
        {
            _characterStats.RegisterActiveBonus(activeBonus);
        }
    }

    private void UnregisterEquipmentBonuses(Equipment equipment)
    {
        foreach (PassiveBonus passiveBonus in equipment._passiveBonuses)
        {
            _characterStats.UnregisterPassiveBonus(passiveBonus);
        }
        foreach (ActiveBonus activeBonus in equipment._activeBonuses)
        {
            _characterStats.UnregisterActiveBonus(activeBonus);
        }
    }

    private void Equip(Lootable item, EquipmentSlot slot)
    {
        if (Tools.TryCast(item.item, out Equipment equipment))
        {
            PuppetSlot puppetSlot = GetPuppetSlot(slot);
            slot.Equip(item);
            RegisterEquipmentBonuses(equipment);

            Instantiate(equipment.skin, slot._parent);
            if (puppetSlot != null)
            {
                EquipPuppetSlot(equipment, puppetSlot);
            }
        }
    }

    private void Unequip(Lootable item, EquipmentSlot slot)
    {
        if (Tools.TryCast(item.item, out Equipment equipment))
        {
            PuppetSlot puppetSlot = GetPuppetSlot(slot);
            slot.Unequip();
            UnregisterEquipmentBonuses(equipment);

            for (int i = 0; i < slot._parent.childCount; i++)
            {
                Destroy(slot._parent.GetChild(0).gameObject);
            }
            if (puppetSlot != null)
            {
                UnequipPuppetSlot(puppetSlot);
            }
        }
    }

    private void EquipPuppetSlot(Equipment equipment, PuppetSlot slot)
    {
        Instantiate(equipment.skin, slot._parent);
    }

    private void UnequipPuppetSlot(PuppetSlot slot)
    {
        for (int i = 0; i < slot._parent.childCount; i++)
        {
            Destroy(slot._parent.GetChild(0).gameObject);
        }
    }

    #endregion

    #region Protected Methods



    #endregion

    #region Public Methods

    public bool IsItemEquipped(Lootable item)
    {
        return IsAlreadyEquipped(item, out EquipmentSlot slot);
    }

    public EEquipResult TryToEquip(Lootable item, bool restoreFromSaveFile = false)
    {
        if (Tools.TryCast(item.item, out Equipment equipment))
        {
            foreach (EGearSlotType slotType in equipment.slots)
            {
                if (IsAlreadyEquipped(item, out EquipmentSlot usedSlot))
                {
                    Unequip(item, usedSlot);
                    if (!restoreFromSaveFile && !_isSilent)
                    {
                        //writeEquipmentLog(0, usedSlot.type, Lootable.LinkedItem.label);
                        SoundManager.Instance.playSFX(ESFXType.ItemEquipped);
                    }
                    return EEquipResult.Unequipped;
                }
                else
                {
                    EquipmentSlot slot = GetEquipmentSlot(slotType);
                    if (!slot._isUsed)
                    {
                        Equip(item, slot);
                        if (!restoreFromSaveFile && !_isSilent)
                        {
                            //writeEquipmentLog(1, slot, Lootable.LinkedItem.label);
                            SoundManager.Instance.playSFX(ESFXType.ItemEquipped);
                        }
                        return EEquipResult.Equipped;
                    }
                }
            }
        }
        return EEquipResult.Failed;
    }

    #endregion
}

[System.Serializable]
public class EquipmentSlot
{
    public EGearSlotType _type;
    public Transform _parent;
    public bool _isUsed = false;
    public Lootable _currentlyEquipped;

    public void Equip(Lootable equipped)
    {
        _isUsed = true;
        _currentlyEquipped = equipped;
    }

    public void Unequip()
    {
        _isUsed = false;
        _currentlyEquipped = null;
    }
}

[System.Serializable]
public class PuppetSlot
{
    public EGearSlotType _type;
    public Transform _parent;
}
