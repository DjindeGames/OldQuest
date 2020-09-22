using System;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string description;
    public ItemType Type { get; private set; }
    public LootableOutlineType rarity;

    public void Awake()
    {
        Type = (ItemType)Enum.Parse(typeof(ItemType), GetType().ToString());
    }

    public void fetchType()
    {
        Type =  (ItemType)Enum.Parse(typeof(ItemType), GetType().ToString());
    }
}
