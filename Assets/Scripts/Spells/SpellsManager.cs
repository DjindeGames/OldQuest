using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellsManager : MonoBehaviour
{
    [Header("Spells")]
    [SerializeField]
    private Spell[] spellsDB;
    public List<ESpellType> learntSpells = new List<ESpellType>();

    public static SpellsManager Instance { get; private set; }

    private List<Spell> castedSpells = new List<Spell>();

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

    public void clearActiveBonuses()
    {
        foreach(Spell spell in castedSpells)
        {
            UnregisterSpellBonuses(spell);
        }
        castedSpells.Clear();
        SpellBookUI.Instance.resetActiveSpells();
    }

    public Spell getSpellFromDB(ESpellType which)
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

    public void addSpell(ESpellType which)
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

    public bool canSpellBeCasted(ESpellType which)
    {
        Spell spell = getSpellFromDB(which);
        return spell.cost <= DeathShards && !castedSpells.Contains(spell);
    }

    public void castSpell(ESpellType which)
    {
        SoundManager.Instance.playSFX(ESFXType.CastSpell);
        Spell spell = getSpellFromDB(which);
        DeathShards -= spell.cost;
        castedSpells.Add(spell);
        RegisterSpellBonuses(spell);
        DiceBoardManager.Instance.recomputeNeededScore();
    }

    private void RegisterSpellBonuses(Spell spell)
    {
        foreach(ActiveBonus bonus in spell._activeBonuses)
        {
            PlayerFastAccess._CharacterStats.RegisterActiveBonus(bonus);
        }
        foreach (PassiveBonus bonus in spell._passiveBonuses)
        {
            PlayerFastAccess._CharacterStats.RegisterPassiveBonus(bonus);
        }
    }

    private void UnregisterSpellBonuses(Spell spell)
    {
        foreach (ActiveBonus bonus in spell._activeBonuses)
        {
            PlayerFastAccess._CharacterStats.UnregisterActiveBonus(bonus);
        }
        foreach (PassiveBonus bonus in spell._passiveBonuses)
        {
            PlayerFastAccess._CharacterStats.UnregisterPassiveBonus(bonus);
        }
    }
}
