using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateSave))]
public class Lightable : Highlightable
{
    [Header("References")]
    [SerializeField]
    private GameObject flame;
    
    [Header("Parameters")]
    [SerializeField]
    private bool lit = false;
    [SerializeField]
    private bool requiresOil = true;

    private StateSave stateSave;

    override protected void Awake()
    {
        base.Awake();
        stateSave = GetComponent<StateSave>();
        if (lit)
        {
            OutlineEnabled = false;
            Destroy(this);
        } else
        {
            flame.SetActive(false);
        }
    }

    protected override void activate()
    {
        lightUp(false);
    }

    public void lightUp(bool force)
    {
        bool possible = true;
        if (!force)
        {
            if (requiresOil)
            {
                possible = InventoryManager.Instance.hasOil();
            }
            if (possible)
            {
                //Save light state
                if (stateSave)
                {
                    SaveManager.Instance.addLitLight(GetComponent<StateSave>().id);
                }
                if (requiresOil)
                {
                    MainUI.Instance.writeLog("Used a oil flask.");
                }
            }
            else
            {
                MainUI.Instance.writeLog("With some oil you could light this up...");
            }
        }
        if (possible)
        {
            flame.SetActive(true);
            OutlineEnabled = false;
            Destroy(this);
        }
    }
}
