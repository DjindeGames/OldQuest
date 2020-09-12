using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DiceBoardUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject lootsResultWindow;
    [SerializeField]
    private Transform lootsParent;
    [SerializeField]
    private GameObject lootPrefab;
    [SerializeField]
    private Button lootsResultWindowConfirm;

    public static DiceBoardUI Instance { get; private set; }
    private int totalLoots;
    private int lootsRevealed;


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        lootsResultWindow.SetActive(false);
    }

    public void close()
    {
        lootsResultWindow.SetActive(false);
        ScreenManager.Instance.switchToPreviousScreen();
    }

    public void showLootResults(List<Item> loots)
    {
        resetLootList();
        totalLoots = loots.Count;
        lootsResultWindow.SetActive(true);
        foreach (Item item in loots)
        {
            GameObject instantiated = Instantiate(lootPrefab, lootsParent);
            LootResult lootResult = instantiated.GetComponent<LootResult>();
            lootResult.setData(item);
        }
    }

    private void resetLootList()
    {
        totalLoots = 0;
        lootsRevealed = 0;
        lootsResultWindowConfirm.interactable = false;
        for (int i = 0; i < lootsParent.childCount; i++)
        {
            Destroy(lootsParent.GetChild(0).gameObject);
        }
    }

    public void notifyLootRevealed()
    {
        lootsRevealed++;
        if (lootsRevealed == totalLoots)
        {
            lootsResultWindowConfirm.interactable = true;
        }
    }
}
