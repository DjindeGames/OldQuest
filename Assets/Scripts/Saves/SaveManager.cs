using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    public bool SaveLoaded { get; set; }


    /*****************************************************/
    /********************* SAVED DATA ********************/
    /*****************************************************/

    //METADATA
    public string SaveFileName { get; private set; }

    //SCENE STATE
    public Vector3 PlayerPosition { get; private set; }
    public List<int> DestroyedItemsIds { get; private set; }
    public List<int> LitLightsIds { get; private set; }
    public Dictionary<int, bool> DoorsStates { get; private set; }
    public List<int> UnlockedDoorsIds { get; private set; }
    public List<int> OpenedChestsIds { get; private set; }

    //INVENTORY
    public int OilAmount { get; private set; }
    public int GoldAmount { get; private set; }
    public List<Tuple<GameObject, bool>> InventoryContent { get; private set; } = new List<Tuple<GameObject, bool>>();
    public List<KeyMaterial> KeysInInventory { get; private set; } = new List<KeyMaterial>();
    public List<ReadableKey> Archives { get; private set; } = new List<ReadableKey>();

    //PLAYER STATS
    public PlayerStats PlayerStats { get; private set; }

    private string savesPath;
    private int totalPlayTime = 0;

    public delegate void saveComplete();
    public event saveComplete saveIsComplete;

    private void Awake()
    {
        if (!Instance) {
            Instance = this;
        }
        savesPath = Constants.SaveFilesPath + "/";
        DontDestroyOnLoad(this.gameObject);
        DestroyedItemsIds = new List<int>();
        LitLightsIds = new List<int>();
        OpenedChestsIds = new List<int>();
        DoorsStates = new Dictionary<int,bool>();
        UnlockedDoorsIds = new List<int>();
    }

    public void save(string fileName)
    {
        StartCoroutine(saveCoroutine(fileName));
    }

    public void addDestroyedItem(int id)
    {
        DestroyedItemsIds.Add(id);
    }

    public void addLitLight(int id)
    {
        LitLightsIds.Add(id);
    }

    public void addUnlockedDoor(int id)
    {
        UnlockedDoorsIds.Add(id);
    }

    public void addOpenedChest(int id)
    {
        OpenedChestsIds.Add(id);
    }

    public void updateDoorState(int id, bool opened)
    {
        DoorsStates[id] = opened;
    }

    public void SaveSettings()
    {
        JSONObject prefs = new JSONObject();

        prefs.AddField("MusicVolume", SettingsManager.Instance.MusicVolume.ToString());
        prefs.AddField("PhysicsVolume", SettingsManager.Instance.PhysicsVolume.ToString());
        prefs.AddField("EffectsVolume", SettingsManager.Instance.EffectsVolume.ToString());
        prefs.AddField("FirstPersonMouseSensitivity", SettingsManager.Instance.FirstPersonMouseSensitivity.ToString());

        Directory.CreateDirectory(Constants.PreferencesFilePath);
        File.WriteAllText(Constants.PreferencesFilePath + "/" + Constants.PreferencesFilename, prefs.Print(true));
    }

    private IEnumerator saveCoroutine(string fileName)
    {
        JSONObject serializedSave = new JSONObject();
        JSONObject playerStats = new JSONObject(JSONObject.Type.ARRAY);
        JSONObject inventoryContent = new JSONObject(JSONObject.Type.ARRAY);
        JSONObject archives = new JSONObject(JSONObject.Type.ARRAY);
        JSONObject litLights = new JSONObject(JSONObject.Type.ARRAY);
        JSONObject openedChests = new JSONObject(JSONObject.Type.ARRAY);
        JSONObject unlockedDoors = new JSONObject(JSONObject.Type.ARRAY);
        JSONObject doorsStates = new JSONObject(JSONObject.Type.ARRAY);
        JSONObject destroyedItems = new JSONObject(JSONObject.Type.ARRAY);
        JSONObject keys = new JSONObject(JSONObject.Type.ARRAY);

        //GETTING DATE AND PLAYTIME
        System.Globalization.CultureInfo.CurrentCulture.ClearCachedData();
        string date = DateTime.Now.ToString("MM-dd-yyyy_HH-mm-ss");
        string dateInFile = DateTime.Now.ToString("MM/dd/yyyy - HH:mm:ss");
        totalPlayTime += (int)TimeManager.Instance.ElapsedTime;

        //SERIALIZING SAVE METADATA
        serializedSave.AddField(Constants.SFSerializedNameField, fileName);
        serializedSave.AddField(Constants.SFSerializedDateField, dateInFile);
        serializedSave.AddField(Constants.SFSerializedChapterField, "Chapter " + ChapterData.Instance.chapterNumber + ": " + ChapterData.Instance.chapterName);
        serializedSave.AddField(Constants.SFSerializedPlayTimeField, totalPlayTime);
        serializedSave.AddField(Constants.SFSerializedSceneField, SceneManager.GetActiveScene().name);

        //SERIALIZING PLAYER STATS
        PlayerStats copiedPlayerStats = PlayerStatsManager.Instance.getPlayerStats();
        playerStats.AddField(Constants.SFSerializedPlayerStatsHealthPointsField, copiedPlayerStats.healthPoints);
        playerStats.AddField(Constants.SFSerializedPlayerStatsVitalityField, copiedPlayerStats.vitality);
        playerStats.AddField(Constants.SFSerializedPlayerStatsStrengthField, copiedPlayerStats.strength);
        playerStats.AddField(Constants.SFSerializedPlayerStatsEnduranceField, copiedPlayerStats.endurance);
        playerStats.AddField(Constants.SFSerializedPlayerStatsHitRollsField, copiedPlayerStats.hitRolls);
        playerStats.AddField(Constants.SFSerializedPlayerStatsScoreToHitField, copiedPlayerStats.scoreToHit);
        serializedSave.AddField(Constants.SFSerializedPlayerStatsField, playerStats);

        //SERIALIZING SCENE STATE
        JSONObject position = new JSONObject();
        Vector3 playerPosition = PlayerController.Instance.gameObject.transform.position;
        position.AddField("x", playerPosition.x.ToString());
        position.AddField("y", playerPosition.y.ToString());
        position.AddField("z", playerPosition.z.ToString());

        serializedSave.AddField(Constants.SFSerializedPlayerPositionField, position);

        //Destroyed items
        foreach (int id in DestroyedItemsIds)
        {
            destroyedItems.Add(id);
        }
        serializedSave.AddField(Constants.SFSerializedDestroyedItemsField, destroyedItems);

        //Lit lights
        foreach (int id in LitLightsIds)
        {
            litLights.Add(id);
        }
        serializedSave.AddField(Constants.SFSerializedLitLightsField, litLights);

        //Opened chests
        foreach (int id in OpenedChestsIds)
        {
            openedChests.Add(id);
        }
        serializedSave.AddField(Constants.SFSerializedOpenedChestsField, openedChests);

        //Doors state
        foreach (KeyValuePair<int, bool> door in DoorsStates)
        {
            JSONObject serializedDoorState = new JSONObject(JSONObject.Type.ARRAY);
            serializedDoorState.Add(door.Key);
            serializedDoorState.Add(door.Value);
            doorsStates.Add(serializedDoorState);
        }
        serializedSave.AddField(Constants.SFSerializedDoorsStatesField, doorsStates);

        //Unlocked doors
        foreach (int unlockedDoor in UnlockedDoorsIds)
        {
            unlockedDoors.Add(unlockedDoor);
        }
        serializedSave.AddField(Constants.SFSerializedUnlockedDoorsField, unlockedDoors);

        //SERIALIZING INVENTORY
        serializedSave.AddField(Constants.SFSerializedOilAmountField, InventoryManager.Instance.OilAmount);
        serializedSave.AddField(Constants.SFSerializedGoldAmountField, InventoryManager.Instance.GoldAmount);

        //Keys
        for(int i = 0; i < 3; i++)
        {
            InventoryKey key = InventoryManager.Instance.getKeyByMaterial((KeyMaterial)i);
            if (key.isInInventory)
            {
                keys.Add(((KeyMaterial)i).ToString());
            }
        }
        serializedSave.AddField(Constants.SFSerializedKeysField, keys);

        //Items
        foreach (Tuple<GameObject,bool> item in InventoryManager.Instance.inventoryContent)
        {
            JSONObject inventoryItem = new JSONObject(JSONObject.Type.ARRAY);
            inventoryItem.Add(Constants.prefabsPath + item.Item1.GetComponent<Lootable>().item.Type.ToString() + "/" + item.Item1.name.Split('$')[0]);
            inventoryItem.Add(item.Item2);
            inventoryContent.Add(inventoryItem);
        }
        serializedSave.AddField(Constants.SFSerializedInventoryContentField, inventoryContent);

        //Archives
        foreach (ReadableKey key in InventoryManager.Instance.archives)
        {
            archives.Add(key.ToString());
        }
        serializedSave.AddField(Constants.SFSerializedArchivesField, archives);

        //WRITING SAVE ON DISK
        Directory.CreateDirectory(savesPath + date);
        System.IO.File.WriteAllText(savesPath + date + "/" + date + Constants.SaveFilesExtension, serializedSave.Print(true));

        //TAKING SCREENSHOT
        UIManager.Instance.toggleCurrentUI(false);
        yield return new WaitForEndOfFrame();
        ScreenCapture.CaptureScreenshot(savesPath + date + "/" + date + Constants.ScreenshotsExtension);
        UIManager.Instance.toggleCurrentUI(true);

        //WAITING FOR SCREEN TO BE WRITTEN ON DISK
        while (!File.Exists(savesPath + date + "/" + date + Constants.ScreenshotsExtension))
        {
            yield return null;
        }

        //ZIP THE SAVE FILE
        ZipFile.CreateFromDirectory(savesPath + date, savesPath + date + Constants.ZippedSavesExtension);
        Directory.Delete(savesPath + date, true);
        if (saveIsComplete != null)
        {
            saveIsComplete();
        }

        //REFRESH SAVES LIST
        LoadScreenManager.Instance.refreshList();
    }

    public void load(string file)
    {
        resetAll();
        ZipFile.ExtractToDirectory(file, Constants.TemporaryExtractedSavesPath);
        JSONObject saveFile = new JSONObject(System.IO.File.ReadAllText(Directory.GetFiles(Constants.TemporaryExtractedSavesPath, "*" + Constants.SaveFilesExtension)[0]));

        totalPlayTime = Int32.Parse(saveFile.GetField(Constants.SFSerializedPlayTimeField).ToString());

        //Listing destroyedItems
        foreach (JSONObject destroyed in saveFile.GetField(Constants.SFSerializedDestroyedItemsField).list)
        {
            DestroyedItemsIds.Add(Int32.Parse(destroyed.ToString()));
        }

        //Listing litLights
        foreach (JSONObject lit in saveFile.GetField(Constants.SFSerializedLitLightsField).list)
        {
            LitLightsIds.Add(Int32.Parse(lit.ToString()));
        }

        //Listing openedChests
        foreach (JSONObject chest in saveFile.GetField(Constants.SFSerializedOpenedChestsField).list)
        {
            OpenedChestsIds.Add(Int32.Parse(chest.ToString()));
        }

        //Listing UnlockedDoors
        foreach (JSONObject unlocked in saveFile.GetField(Constants.SFSerializedUnlockedDoorsField).list)
        {
            UnlockedDoorsIds.Add(Int32.Parse(unlocked.ToString()));
        }

        //Listing DoorsStates
        foreach (JSONObject door in saveFile.GetField(Constants.SFSerializedDoorsStatesField).list)
        {
            if (bool.TryParse(door[1].ToString(), out bool state))
            {
                DoorsStates[Int32.Parse(door[0].ToString())] = state;
            }
        }

        //Restoring inventory
        OilAmount = Int32.Parse(saveFile.GetField(Constants.SFSerializedOilAmountField).ToString());
        GoldAmount = Int32.Parse(saveFile.GetField(Constants.SFSerializedGoldAmountField).ToString());
        foreach (JSONObject item in saveFile.GetField(Constants.SFSerializedInventoryContentField).list)
        {
            if (bool.TryParse(item[1].ToString(), out bool state))
            {
                InventoryContent.Add(new Tuple<GameObject, bool>(Resources.Load<GameObject>(item[0].str + "$"), state));
            }
        }

        //Restoring keys
        foreach (JSONObject item in saveFile.GetField(Constants.SFSerializedKeysField).list)
        {
            if (Enum.TryParse<KeyMaterial>(item.str, out KeyMaterial material))
            {
                KeysInInventory.Add(material);
            }
        }

        //Restoring archives
        foreach (JSONObject archiveFile in saveFile.GetField(Constants.SFSerializedArchivesField).list)
        {
            if (Enum.TryParse<ReadableKey>(archiveFile.str, out ReadableKey key))
            {
                Archives.Add(key);
            }
        }

        //Restoring Player stats
        JSONObject playerStats = saveFile.GetField(Constants.SFSerializedPlayerStatsField);
        PlayerStats = new PlayerStats();
        PlayerStats.healthPoints = Int32.Parse(playerStats.GetField(Constants.SFSerializedPlayerStatsHealthPointsField).ToString());
        PlayerStats.vitality = Int32.Parse(playerStats.GetField(Constants.SFSerializedPlayerStatsVitalityField).ToString());
        PlayerStats.strength = Int32.Parse(playerStats.GetField(Constants.SFSerializedPlayerStatsStrengthField).ToString());
        PlayerStats.endurance = Int32.Parse(playerStats.GetField(Constants.SFSerializedPlayerStatsEnduranceField).ToString());
        PlayerStats.hitRolls = Int32.Parse(playerStats.GetField(Constants.SFSerializedPlayerStatsHitRollsField).ToString());
        PlayerStats.scoreToHit = Int32.Parse(playerStats.GetField(Constants.SFSerializedPlayerStatsScoreToHitField).ToString());

        //Misc
        JSONObject playerPosition = saveFile.GetField(Constants.SFSerializedPlayerPositionField);
        PlayerPosition = new Vector3(float.Parse(playerPosition.GetField("x").str), float.Parse(playerPosition.GetField("y").str), float.Parse(playerPosition.GetField("z").str));
        SaveFileName = saveFile.GetField(Constants.SFSerializedInventoryContentField).str;

        Directory.Delete(Constants.TemporaryExtractedSavesPath, true);
        SaveLoaded = true;
        if (ScreenManager.Instance != null)
        {
            ScreenManager.Instance.switchScreen(ScreenType.Main);
        }
        SceneManager.LoadScene(saveFile.GetField(Constants.SFSerializedSceneField).str);
    }

    private void resetAll()
    {
        InventoryContent.Clear();
        LitLightsIds.Clear();
        DestroyedItemsIds.Clear();
        OpenedChestsIds.Clear();
        KeysInInventory.Clear();
        DoorsStates.Clear();
        UnlockedDoorsIds.Clear();
        Archives.Clear();
    }
}
