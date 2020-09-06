using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private TMP_Text itemName;
    [SerializeField]
    private TMP_Text itemDetails;
    [SerializeField]
    private TMP_Text goldAmount;
    [SerializeField]
    private TMP_Text oilAmount;


    public static InventoryUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        itemName.text = "";
        itemDetails.text = "";
        oilAmount.text = InventoryManager.Instance.OilAmount.ToString();
        goldAmount.text = InventoryManager.Instance.GoldAmount.ToString();
    }

    public void exit()
    {
        ScreenManager.Instance.switchScreen(ScreenType.Main);
    }

    public void showItemDetails(Usable which)
    {
        itemName.text = which.name;
        itemDetails.text = which.details;
        if (which.Type == ItemType.Equipment)
        {
            Equipment equipment = (Equipment)which;
            itemDetails.text += "\n\nBonuses:";
            for (int i = 0; i < equipment.stats.Length; i++)
            {
                itemDetails.text += "\n" + equipment.stats[i].type.ToString() + ": " + equipment.stats[i].value.ToString();
            }
        }
    }

    public void hideItemDetails()
    {
        itemName.text = "";
        itemDetails.text = "";
    }

    public void setGoldAmount(int amount)
    {
        goldAmount.text = amount.ToString();
    }

    public void setOilAmount(int amount)
    {
        oilAmount.text = amount.ToString();
    }
}
