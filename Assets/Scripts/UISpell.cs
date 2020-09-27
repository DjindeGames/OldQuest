using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISpell : MonoBehaviour
{
    [Header("References")]
    public TMP_Text name;
    [Header("Parameters")]
    [SerializeField]
    private Color idleSpellColor;
    [SerializeField]
    private Color selectedSpellColor;

    private Image backgroundImage;
    public SpellType type;
    private bool selected = false;

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
        unselect();
    }

    public void setData(Spell spell)
    {
        name.text = spell.name;
        type = spell.type;
    }

    public void onSelect(bool playSFX = true)
    {
        if (!selected)
        {
            selected = true;
            SpellBookUI.Instance.onSpellSelected(type, playSFX);
            backgroundImage.color = selectedSpellColor;
        }
    }

    public void unselect()
    {
        selected = false;
        backgroundImage.color = idleSpellColor;
    }
}
