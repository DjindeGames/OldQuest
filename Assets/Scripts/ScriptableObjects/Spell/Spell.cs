using UnityEngine;

namespace Djinde.Quest
{
    [CreateAssetMenu]
    public class Spell : ScriptableObject
    {
        public new string name;
        public string description;
        public ESpellType type;
        public PassiveBonus[] _passiveBonuses;
        public ActiveBonus[] _activeBonuses;
        public int cost;
    }
}