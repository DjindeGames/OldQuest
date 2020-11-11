using UnityEngine;
using Djinde.Utils;

namespace Djinde.Quest
{
    public class PlayerComponent : MonoBehaviour
    {
        #region Events



        #endregion

        #region Exposed Attributes



        #endregion

        #region Attributes

        public static PlayerComponent _Instance { get; private set; }

        public Vector3 _Position
        {
            get
            {
                return transform.position;
            }
        }
        public Quaternion _Rotation
        {
            get
            {
                return transform.rotation;
            }
        }

        public EquipmentHolder _EquipmentHolder { get; private set; }
        public PlayerCharacterStats _CharacterStats { get; private set; }

        #endregion

        #region MonoBehaviour Methods

        private void Awake()
        {
            _Instance = this;

            _EquipmentHolder = GetComponent<EquipmentHolder>();
            if (_EquipmentHolder == null)
            {
                Tools.LogWarning(this, "No EquipmentHolder found!");
            }

            _CharacterStats = GetComponent<PlayerCharacterStats>();
            if (_CharacterStats == null)
            {
                Tools.LogWarning(this, "No CharacterStats found!");
            }
        }

        #endregion

        #region Private Methods



        #endregion

        #region Protected Methods



        #endregion

        #region Public Methods



        #endregion
    }
}