using UnityEngine;

[CreateAssetMenu]
public class Spell : ScriptableObject
{
    public new string name;
    public string description;
    public ESpellType type;
    public SpellBonus[] bonuses;
    public int cost;
}

[System.Serializable]
public class SpellBonus
{
    public ESpellBonusType type;
    public int value;
}
