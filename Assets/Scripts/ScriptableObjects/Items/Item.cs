using System;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string description;
    public ItemType Type { get; private set; }
    public LootableOutlineType rarity;

    public void Awake()
    {
        fetchType();
    }

    public void fetchType()
    {
        Type =  (ItemType)Enum.Parse(typeof(ItemType), GetType().ToString());
    }
}
