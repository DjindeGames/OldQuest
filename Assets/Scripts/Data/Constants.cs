using UnityEngine;

public class Constants
{
    //FILES
    //paths
    public static string SaveFilesPath { get; private set; } = Application.persistentDataPath + "/Saves";
    public static string TemporaryExtractedSavesPath { get; private set; } = SaveFilesPath + "/Temp";
    public static string PreferencesFilePath { get; private set; } = Application.persistentDataPath;
    public static string PrefabsPath { get; private set; } = "Prefabs/Lootables/";
    public static char LootableEndCharacter { get; private set; } = '$';
    public static string PreferencesFilename { get; private set; } = "userprefs.json";
    //extensions
    public static string SaveFilesExtension { get; private set; } = ".json";
    public static string ZippedSavesExtension { get; private set; } = ".qsf";
    public static string ScreenshotsExtension { get; private set; } = ".png";

    //TAGS
    public static string SaveTag { get; private set; } = "Savable";
    public static string DiceBoardTag { get; private set; } = "DiceBoard";

    //SAVEFILES SERIALIZED FIELDS
    //Metadata
    public static string SFSerializedNameField { get; private set; } = "Name";
    public static string SFSerializedDateField { get; private set; } = "Date";
    public static string SFSerializedChapterField { get; private set; } = "Chapter";
    public static string SFSerializedPlayTimeField { get; private set; } = "PlayTime";
    public static string SFSerializedSceneField { get; private set; } = "Scene";
    //PlayerStats
    public static string SFSerializedPlayerStatsField { get; private set; } = "PlayerStats";
    public static string SFSerializedPlayerStatsHealthPointsField { get; private set; } = "HealthPoints";
    public static string SFSerializedPlayerStatsVitalityField { get; private set; } = "Vitality";
    public static string SFSerializedPlayerStatsEnduranceField { get; private set; } = "Endurance";
    public static string SFSerializedPlayerStatsStrengthField { get; private set; } = "Strength";
    public static string SFSerializedPlayerStatsHitRollsField { get; private set; } = "HitRolls";
    public static string SFSerializedPlayerStatsScoreToHitField { get; private set; } = "ScoreToHit";
    public static string SFSerializedPlayerStatsDamagesField { get; private set; } = "Damages";
    //Spells
    public static string SFSerializedSpellsField { get; private set; } = "Spells";
    public static string SFSerializedDeathShardsField { get; private set; } = "DeathShardsAmount";
    public static string SFSerializedMaxDeathShardsField { get; private set; } = "MaxDeathShards";
    //Inventory
    public static string SFSerializedOilAmountField { get; private set; } = "OilAmount";
    public static string SFSerializedGoldAmountField { get; private set; } = "GoldAmount";
    public static string SFSerializedKeysField { get; private set; } = "Keys";
    public static string SFSerializedInventoryContentField { get; private set; } = "InventoryContent";
    public static string SFSerializedArchivesField { get; private set; } = "Archives";
    //SceneState
    public static string SFSerializedPlayerPositionField { get; private set; } = "PlayerPosition";
    public static string SFSerializedDestroyedItemsField { get; private set; } = "DestroyedItems";
    public static string SFSerializedLitLightsField { get; private set; } = "LitLights";
    public static string SFSerializedOpenedChestsField { get; private set; } = "OpenedChests";
    public static string SFSerializedKilledEnnemiesField { get; private set; } = "KilledEnnemies";
    public static string SFSerializedDoorsStatesField { get; private set; } = "DoorsStates";
    public static string SFSerializedUnlockedDoorsField { get; private set; } = "UnlockedDoors";
    public static string SFSerializedSpawnedItemsField { get; private set; } = "SpawnedItems";
    public static string SFSerializedSpawnedItemPositionField { get; private set; } = "Position";
    public static string SFSerializedSpawnedItemRotationField { get; private set; } = "Rotation";
}
