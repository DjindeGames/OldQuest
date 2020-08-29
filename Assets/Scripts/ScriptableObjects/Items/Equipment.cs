using UnityEngine;

[CreateAssetMenu]
public class Equipment : Usable
{
    public GearSlot[] slots;
    public GearStats[] stats;
}

[System.Serializable]
public class GearStats
{
    public EquipmentBonus type;
    public int value;
}
