using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadManager : MonoBehaviour
{
    public static LoadManager Instance { get; private set; }
    private SaveManager saveManager;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        saveManager = SaveManager.Instance;
        loadUserPrefs();
        if (saveManager.SaveLoaded)
        {
            saveManager.SaveLoaded = false;
            loadSavedData();
        }
    }

    private void loadUserPrefs()
    {
        string[] files = Directory.GetFiles(Constants.PreferencesFilePath, Constants.PreferencesFilename);
        if (files.Length > 0)
        {
            JSONObject preferences = new JSONObject(System.IO.File.ReadAllText(files[0]));
            SettingsManager.Instance.setVolume(VolumeType.Music, float.Parse(preferences.GetField("MusicVolume").str));
            SettingsManager.Instance.setVolume(VolumeType.Effects, float.Parse(preferences.GetField("EffectsVolume").str));
            SettingsManager.Instance.setVolume(VolumeType.Physics, float.Parse(preferences.GetField("PhysicsVolume").str));
            SettingsManager.Instance.setFirstPersonMouseSensitivity(float.Parse(preferences.GetField("FirstPersonMouseSensitivity").str));
        }
        else
        {
            //If the file does not exist, SaveOptions will create it !
            saveManager.SaveSettings();
        }
        MenuUI.Instance.refreshSettings();
    }

    private void loadSavedData()
    {
        MenuUI.Instance.setFileName(saveManager.SaveFileName);
        //Restoring inventory
        loadInventoryContent();
        //Restoring playerStats
        PlayerStatsManager.Instance.loadStats(SaveManager.Instance.PlayerStats);
        //Restoring scene state
        PlayerController.Instance.gameObject.transform.position = saveManager.PlayerPosition;
        removeDestroyedItems();
        restoreLights();
        restoreDoorsStates();
        openOpenedChests();
        unspawnKilledEnnemies();
        loadSpawnedItems();
        loadSpells();
    }

    private void restoreDoorsStates()
    {
        //Unlock unlocked doors
        foreach (int id in saveManager.UnlockedDoorsIds)
        {
            GameObject door = retrieveSavedGameObject(id);
            if (door)
            {
                Door doorComponent = door.GetComponent<Door>();
                if (doorComponent)
                {
                    doorComponent.unlock();
                }
            }
        }
        //Restore doorsStates
        foreach (KeyValuePair<int, bool> door in saveManager.DoorsStates)
        {
            GameObject doorObject = retrieveSavedGameObject(door.Key);
            if (doorObject)
            {
                Door doorComponent = doorObject.GetComponent<Door>();
                if (doorComponent)
                {
                    doorComponent.restoreState(door.Value);
                }
            }
        }
    }

    private void removeDestroyedItems()
    {
        foreach(int id in saveManager.DestroyedItemsIds)
        {
            if (id >= 0)
            {
                GameObject toBeDestroyed = retrieveSavedGameObject(id);
                if (toBeDestroyed)
                {
                    Destroy(toBeDestroyed);
                }
            }
        }
    }

    private void restoreLights()
    {
        foreach (int id in saveManager.LitLightsIds)
        {
            GameObject toBeLit = retrieveSavedGameObject(id);
            if (toBeLit)
            {
                Lightable lightable = toBeLit.GetComponent<Lightable>();
                if (lightable)
                {
                    lightable.lightUp(true);
                }
            }
        }
    }

    private void openOpenedChests()
    {
        foreach (int id in saveManager.OpenedChestsIds)
        {
            GameObject chestToOpen = retrieveSavedGameObject(id);
            if (chestToOpen)
            {
                Chest chest = chestToOpen.GetComponent<Chest>();
                if (chest)
                {
                    chest.forceOpen();
                }
            }
        }
    }

    private void unspawnKilledEnnemies()
    {
        foreach (int id in saveManager.KilledEnnemiesIds)
        {
            GameObject ennemyToKill = retrieveSavedGameObject(id);
            if (ennemyToKill)
            {
                Ennemy ennemy = ennemyToKill.GetComponent<Ennemy>();
                if (ennemy)
                {
                    ennemy.forceUnspawn();
                }
            }
        }
    }

    private void loadSpawnedItems()
    {
        //References can't be kept as we would compare an instantiated gameobject to it's prefab 
        //when later picking up any removed item (see the implementation of SaveManager::tryDeleteRemovedItem)
        //That's why the list needs to be rebuilt
        List<GameObject> newListContent = new List<GameObject>();
        foreach (Tuple<GameObject, Tuple<Vector3, Vector3>> spawnedItem in saveManager.SpawnedItems)
        {
            GameObject instantiated = Instantiate(spawnedItem.Item1);
            Destroy(instantiated.GetComponent<StateSave>());
            instantiated.transform.position = spawnedItem.Item2.Item1;
            instantiated.transform.rotation = Quaternion.Euler(spawnedItem.Item2.Item2);
            newListContent.Add(instantiated);
        }
        saveManager.SpawnedItems.Clear();
        foreach (GameObject item in newListContent)
        {
            SaveManager.Instance.addSpawnedItem(item);
        }
    }

    private void loadInventoryContent()
    {
        InventoryManager.Instance.OilAmount = saveManager.OilAmount;
        InventoryManager.Instance.GoldAmount = saveManager.GoldAmount;
        foreach (Tuple<GameObject, bool> item in saveManager.InventoryContent)
        {
            InventoryManager.Instance.addItem(Instantiate(item.Item1), item.Item2, true);
        }
        foreach (KeyMaterial material in saveManager.KeysInInventory)
        {
            InventoryManager.Instance.addKey(material);
        }
        foreach (ReadableKey key in saveManager.Archives)
        {
            InventoryManager.Instance.addFileToArchives(key, false);
        }
    }

    private void loadSpells()
    {
        SpellsManager.Instance.DeathShards = SaveManager.Instance.DeathShards;
        SpellsManager.Instance.MaxDeathShards = SaveManager.Instance.MaxDeathShards;
        foreach (SpellType spell in saveManager.LearntSpells)
        {
            SpellsManager.Instance.addSpell(spell);
        }
    }

    private GameObject retrieveSavedGameObject(int id)
    {
        GameObject retrieved = null;
        foreach (GameObject item in GameObject.FindGameObjectsWithTag(Constants.SaveTag))
        {
            StateSave stateSave = item.GetComponent<StateSave>();
            if (stateSave)
            {
                if (stateSave.id == id)
                {
                    retrieved = item;
                    break;
                }
            }
        }
        return retrieved;
    }
}
