using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GameConstants : MonoBehaviour
{
    [BoxGroup("Interactions")]
    public float throwForce = 8;
    [BoxGroup("Interactions")]
    public float throwTorque = 3000;
    [BoxGroup("Interactions")]
    public float interactionRadius = 3.5f;
    [BoxGroup("Interactions")]
    public float playerControllerFreezingDelay = 0.1f;
    [BoxGroup("Interactions")]
    public float physicsSoundMultiplier = 0.1f;
    [BoxGroup("Interactions")]
    [Min(0)]
    public float minCollisionPitch = 0f;
    [BoxGroup("Interactions")]
    [Min(1)]
    public float maxCollisionPitch = 3f;
    [BoxGroup("Interactions")]
    public LootableOutline[] lootableOutlines;

    [BoxGroup("Inventory")]
    public float puppetRotationSpeedMultiplier = 10f;
    [BoxGroup("Inventory")]
    public float inventoryItemsErrosionDistance = 0.2f;
    [BoxGroup("Inventory")]
    public float inventoryItemsMouseDistance = 2f;
    [BoxGroup("Inventory")]
    public float inventoryItemsVerticalOffset = 2f;

    [BoxGroup("DiceBoard")]
    public float dicesErrosionDistance = 0.2f;
    [BoxGroup("DiceBoard")]
    public float dicesMouseDistance = 2f;
    [BoxGroup("DiceBoard")]
    public float dicesVerticalOffset = 2f;
    [BoxGroup("DiceBoard")]
    public float dicesThrowForce = 3f;
    [BoxGroup("DiceBoard")]
    public float dicesThrowTorque = 300f;
    [BoxGroup("DiceBoard")]
    public float timeBeforeLock = 0.5f;
    [BoxGroup("DiceBoard")]
    public float minDicesOffset = 0.01f;
    [BoxGroup("DiceBoard")]
    public float maxDicesOffset = 0.05f;
    [BoxGroup("DiceBoard")]
    public float checkForStabilizationPeriod = 1f;
    [BoxGroup("DiceBoard")]
    public float diceMovingThreshold = 0.1f;
    [BoxGroup("DiceBoard")]
    public DiceOutline[] dicesOutlines;

    public static GameConstants Instance { get; private set; }

    void Awake()
    {
        Instance = this;   
    }

    public Material getLootableOutlineMaterialByType(LootableOutlineType which)
    {
        Material material = null;
        for (int i = 0; i < lootableOutlines.Length; i++)
        {
            if (lootableOutlines[i].type == which)
            {
                material = lootableOutlines[i].material;
            }
        }
        return material;
    }

    public Color getLootableRarityColorByType(LootableOutlineType which)
    {
        Color outlineColor = Color.black;
        for (int i = 0; i < lootableOutlines.Length; i++)
        {
            if (lootableOutlines[i].type == which)
            {
                outlineColor = lootableOutlines[i].material.GetColor(lootableOutlines[i].material.shader.GetPropertyNameId(3));
                //outlineColor = lootableOutlines[i].material.GetColor(lootableOutlines[i].material.shader.GetPropertyNameId;
            }
        }
        return outlineColor;
    }

    public Material getDiceOutlineMaterialByType(DiceOutlineType which)
    {
        Material material = null;
        for (int i = 0; i < dicesOutlines.Length; i++)
        {
            if (dicesOutlines[i].type == which)
            {
                material = dicesOutlines[i].material;
            }
        }
        return material;
    }
}

[System.Serializable]
public class LootableOutline
{
    public LootableOutlineType type;
    public Material material;
}

[System.Serializable]
public class DiceOutline
{
    public DiceOutlineType type;
    public Material material;
}
