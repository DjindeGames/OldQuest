using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class SpellBookUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform spellsParent;
    [SerializeField]
    private TMP_Text currentSpellName;
    [SerializeField]
    private TMP_Text currentSpellDescription;
    [SerializeField]
    private TMP_Text currentSpellCost;
    [SerializeField]
    private Button castButton;
    [SerializeField]
    private GameObject spellPrefab;
    [SerializeField]
    private TMP_Text activeSpells;

    public static SpellBookUI Instance { get; private set; }
    private List<UISpell> spells = new List<UISpell>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentSpellCost.text = "-";
        currentSpellDescription.text = "";
        currentSpellName.text = "";
        loadSpells();
    }

    private void loadSpells()
    {
        foreach (ESpellType spell in SpellsManager.Instance.learntSpells)
        {
            GameObject spellListItem = Instantiate(spellPrefab, spellsParent);
            UISpell uiSpell = spellListItem.GetComponent<UISpell>();
            uiSpell.setData(SpellsManager.Instance.getSpellFromDB(spell));
            spells.Add(uiSpell);
        }
        if (spells.Count > 0)
        {
            spells[0].onSelect(false);
        }
    }

    public void onSpellSelected(ESpellType which, bool playSFX = true)
    {
        if (playSFX)
        {
            SoundManager.Instance.playSFX(ESFXType.PageChanged);
        }
        Spell spell = SpellsManager.Instance.getSpellFromDB(which);
        currentSpellName.text = spell.name;
        currentSpellDescription.text = spell.description;
        foreach(SpellBonus spellBonus in spell.bonuses)
        {
            currentSpellDescription.text += "\n" + spellBonus.type.ToString() + ": " + spellBonus.value.ToString();
        }
        currentSpellCost.text = spell.cost.ToString();
        castButton.onClick.RemoveAllListeners();
        castButton.onClick.AddListener(delegate () { castSpell(which); });
        castButton.interactable = SpellsManager.Instance.canSpellBeCasted(which);
        unselectAllBut(which);
    }

    private void unselectAllBut(ESpellType which)
    {
        foreach (UISpell spell in spells)
        {
            if (spell.type != which)
            {
                spell.unselect();
            }
        }
    }

    public void castSpell(ESpellType which)
    {
        Spell spell = SpellsManager.Instance.getSpellFromDB(which);
        SpellsManager.Instance.castSpell(which);
        castButton.interactable = false;
        activeSpells.text += "...................................................\n<u>" + spell.name + "</u>";
        foreach(SpellBonus bonus in spell.bonuses)
        {
            activeSpells.text += "\n\t" + bonus.type.ToString() + ": " + bonus.value.ToString();
        }
        activeSpells.text += "\n";
    }

    public void resetActiveSpells()
    {
        activeSpells.text = "";
    }
}
