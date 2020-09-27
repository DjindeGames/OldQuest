using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellsManager : MonoBehaviour
{
    [Header("Spells")]
    [SerializeField]
    private Spell[] spellsDB;
    public List<SpellType> learntSpells = new List<SpellType>();

    public static SpellsManager Instance { get; private set; }

    private List<SpellType> castedSpells = new List<SpellType>();

    public int DeathShards
    {
        get
        {
            return deathShards;
        }
        set
        {
            deathShards = value;
            onDeathShardsUpdated?.Invoke(deathShards);
        }
    }

    public int MaxDeathShards
    {
        get
        {
            return maxDeathShards;
        }
        set
        {
            maxDeathShards = value;
            onMaxDeathShardsUpdated?.Invoke(maxDeathShards);
        }
    }

    private int deathShards = 3;
    private int maxDeathShards = 5;

    //Events
    public delegate void deathShardsUpdated(int currentAmount);
    public event deathShardsUpdated onDeathShardsUpdated;

    public delegate void maxDeathShardsUpdated(int currentAmount);
    public event maxDeathShardsUpdated onMaxDeathShardsUpdated;

    private void Awake()
    {
        Instance = this;
    }

    public Spell getSpellFromDB(SpellType which)
    {
        Spell spell = null;
        foreach (Spell DBSpell in spellsDB)
        {
            if (DBSpell.type == which)
            {
                spell = DBSpell;
                break;
            }
        }
        return spell;
    }

    public void addSpell(SpellType which)
    {
        if (!learntSpells.Contains(which))
        {
            learntSpells.Add(which);
        }
    }

    public void addDeathShards(int amount)
    {
        DeathShards += amount;
    }

    public void addMaxDeathShards(int amount)
    {
        MaxDeathShards += amount;
    }

    public bool canSpellBeCasted(SpellType which)
    {
        Spell spell = getSpellFromDB(which);
        return spell.cost <= DeathShards && !castedSpells.Contains(which);
    }

    public void castSpell(SpellType which)
    {
        Spell spell = getSpellFromDB(which);
        DeathShards -= spell.cost;
        castedSpells.Add(which);
    }
}
