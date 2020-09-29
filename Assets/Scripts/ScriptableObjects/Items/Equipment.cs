using UnityEngine;

[CreateAssetMenu]
public class Equipment : Usable
{
    public GameObject skin;
    public EGearSlotType[] slots;
    public GearStats[] stats;
}

[System.Serializable]
public class GearStats
{
    public EEquipmentBonus type;
    public int value;
}
