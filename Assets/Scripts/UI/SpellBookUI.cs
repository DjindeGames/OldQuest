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
    [SerializeField]
    private SelectableList _spellsList;

    public static SpellBookUI Instance { get; private set; }
    private List<SelectableListItem_Spell> spells = new List<SelectableListItem_Spell>();

    private void Awake()
    {
        Instance = this;
        _spellsList.OnItemSelectedEvent += OnSpellSelected;
    }

    private void OnDestroy()
    {
        _spellsList.OnItemSelectedEvent -= OnSpellSelected;
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
        foreach (ESpellType spellType in SpellsManager.Instance.learntSpells)
        {
            Spell spell = SpellsManager.Instance.getSpellFromDB(spellType);
            SelectableListItem_SpellData data = new SelectableListItem_SpellData();
            data._label = spell.name;
            data._spellType = spell.type;
            _spellsList.AddItem(data);
        }
    }

    public void OnSpellSelected(SelectableListItem listItem)
    {
        SelectableListItem_SpellData data = (SelectableListItem_SpellData)listItem._Data;
        Spell spell = SpellsManager.Instance.getSpellFromDB(data._spellType);
        currentSpellName.text = spell.name;
        currentSpellDescription.text = spell.description;
        foreach(ActiveBonus activeBonus in spell._activeBonuses)
        {
            currentSpellDescription.text += "\n" + activeBonus.type.ToString();
        }
        foreach(PassiveBonus passiveBonus in spell._passiveBonuses)
        {
            currentSpellDescription.text += "\n" + passiveBonus.type.ToString() + ": " + passiveBonus.value.ToString();
        }
        currentSpellCost.text = spell.cost.ToString();
        castButton.onClick.RemoveAllListeners();
        castButton.onClick.AddListener(delegate () { castSpell(spell.type); });
        castButton.interactable = SpellsManager.Instance.canSpellBeCasted(spell.type);
    }

    public void castSpell(ESpellType which)
    {
        Spell spell = SpellsManager.Instance.getSpellFromDB(which);
        SpellsManager.Instance.castSpell(which);
        castButton.interactable = false;
    }

    public void resetActiveSpells()
    {
    }
}
