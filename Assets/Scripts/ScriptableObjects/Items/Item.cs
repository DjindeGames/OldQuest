using System;
using UnityEngine;

namespace Djinde.Quest
{
    public abstract class Item : ScriptableObject
    {
        public string description;
        public EItemType Type { get; private set; }
        public ELootableOutlineType rarity;

        public void Awake()
        {
            fetchType();
        }

        public void fetchType()
        {
            string[] splittedType = GetType().ToString().Split('.');
            Type = (EItemType)Enum.Parse(typeof(EItemType), splittedType[splittedType.Length - 1]);
        }
    }
}