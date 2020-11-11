using UnityEngine;

namespace Djinde.Quest
{
    public enum KeyMaterial { Iron, Bronze, Gold }

    [CreateAssetMenu]
    public class Key : Item
    {
        public KeyMaterial material;
    }
}