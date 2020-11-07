/**************************** DICE BOARD **************************/

public enum EDiceValue
{
    Broken = 0,
    One,
    Two,
    Three,
    Four,
    Five,
    Six
}

public enum EDiceColor
{
    Red,
    Green,
    Blue,
    Black,
    White,
    Gold
}

public enum EDiceOutlineType
{
    None,
    Broken,
    Success,
    Failure,
    Selected
}

public enum EDiceThrowType
{
    None,
    Automatic,
    Manual,
    Thrown
}

public enum EThrowActionType
{
    HealingPotion,
    StrengthPotion,
    LootChest,
    PlayerHit,
    PlayerWound,
    EnnemyHit,
    EnnemyWound
}

public enum EThrowActionPerformer
{
    Player,
    Ennemy
}

/******************************* ITEMS ****************************/

public enum EItemType
{
    Equipment,
    Readable,
    Valuable,
    Key,
    Potion,
    Oil,
    Spell
}

public enum EEquipResult
{
    Equipped,
    Unequipped,
    Failed
}

public enum ELootableOutlineType
{
    Lightable,
    Readable,
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public enum EPotionType
{
    Strength,
    Health
}

/******************************* STATS ****************************/

public enum EPassiveStatType
{
    Vitality,
    Strength,
    Endurance,
    HitRolls,
    ScoreToHit,
    Damages,
    //Not Vanilla
    Armor,
    BonusToWound
}

public enum EActiveBonusType
{
    RerollOnes,
    RerollTwos,
    RerollThrees,
    RerollFours,
    RerollFives,
    RerollFailed,
}

public enum ESpellType
{
    BaldwynsBlessing,
    PoisonousBlade,
    GolemsForm
}

public enum EGearSlotType
{
    Chest,
    Head,
    RightHand,
    LeftHand,
    Arms,
    Legs,
    Feet,
    Shoulders
}

/******************************* SOUND ****************************/

public enum ESFXType
{
    None,
    ItemEquipped,
    PageChanged,
    OpenReadable,
    CloseReadable,
    OpenedChest,
    DrinkPotion,
    LootRevealed,
    ItemRemoved,
    CastSpell
}

public enum EVolumeType
{
    Music,
    Effects,
    Physics
}

/******************************** UI ******************************/

public enum EScreenType
{
    Main,
    Inventory,
    Menu,
    Archives,
    Puppet,
    DiceBoard
}

public enum EUIType
{
    None,
    Main,
    Inventory,
    Menu,
    Archives,
    DiceBoard,
    Puppet,
    PlayerUtils,
    All
}

public enum EDiceBoardMenuTab
{
    None,
    Consumables,
    Stats,
    SpellBook,
    All
}

/******************************* MISC *****************************/

public enum ECameraType
{
    Player,
    Inventory,
    FirstPerson,
    DiceBoard,
    Puppet
}

public enum EDoorState
{
    Opened,
    Closed,
    Locked
}





