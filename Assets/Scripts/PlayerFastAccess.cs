namespace Djinde.Quest
{
    public class PlayerFastAccess
    {
        public static EquipmentHolder _EquipmentHolder
        {
            get
            {
                return PlayerComponent._Instance._EquipmentHolder;
            }
        }

        public static PlayerCharacterStats _CharacterStats
        {
            get
            {
                return PlayerComponent._Instance._CharacterStats;
            }
        }
    }
}