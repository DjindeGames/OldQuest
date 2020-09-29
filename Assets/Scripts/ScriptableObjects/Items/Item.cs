using System;
using UnityEngine;

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
        Type =  (EItemType)Enum.Parse(typeof(EItemType), GetType().ToString());
    }
}
