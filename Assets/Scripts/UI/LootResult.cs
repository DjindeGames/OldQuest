using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LootResult : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private TMP_Text description;
    [SerializeField]
    private Outline outline;

    private Color outlineColor;
    private string name;
    private bool revealed = false;

    public void setData(Item item)
    {
        Usable usable = (Usable)item;
        name = usable.name;
        outlineColor = GameConstants.Instance.getLootableRarityColorByType(usable.rarity);
    }

    public void reveal()
    {
        if (!revealed)
        {
            revealed = true;
            description.text = name;
            outline.effectColor = outlineColor;
            SoundManager.Instance.playSFX(SFXType.LootRevealed);
            DiceBoardUI.Instance.notifyLootRevealed();
        }
    }
}
