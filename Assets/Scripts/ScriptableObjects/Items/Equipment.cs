using UnityEngine;

namespace Djinde.Quest
{
    [CreateAssetMenu]
    public class Equipment : Usable
    {
        public GameObject skin;
        public EGearSlotType[] slots;
        public PassiveBonus[] _passiveBonuses;
        public ActiveBonus[] _activeBonuses;
    }

    [System.Serializable]
    public class PassiveBonus
    {
        public EPassiveStatType type;
        public int value;
    }

    [System.Serializable]
    public class ActiveBonus
    {
        public EActiveBonusType type;
    }
}