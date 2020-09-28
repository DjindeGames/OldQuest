using UnityEngine;

[CreateAssetMenu]
public class Spell : ScriptableObject
{
    public new string name;
    public string description;
    public SpellType type;
    public SpellBonus[] bonuses;
    public int cost;
}

[System.Serializable]
public class SpellBonus
{
    public SpellBonusType type;
    public int value;
}
