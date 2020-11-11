using UnityEngine;

namespace Djinde.Quest
{
    [CreateAssetMenu]
    public class Potion : Usable
    {
        public EPotionType type;
        public int value;
    }
}