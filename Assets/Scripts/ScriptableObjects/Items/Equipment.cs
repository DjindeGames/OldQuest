using UnityEngine;

[CreateAssetMenu]
public class Equipment : Usable
{
    public GameObject skin;
    public GearSlot[] slots;
    public GearStats[] stats;
}

[System.Serializable]
public class GearStats
{
    public EquipmentBonus type;
    public int value;
}
