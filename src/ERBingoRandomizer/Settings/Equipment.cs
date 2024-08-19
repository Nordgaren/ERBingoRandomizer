using System.Collections.Generic;
using Project.Tasks;

namespace Project.Settings;

public class Equipment
{
    public static List<ItemLotWrapper> DlcWeaponItemLots = new List<ItemLotWrapper>()
    {
        new ItemLotWrapper(new ItemLotEntry(6500000, 2), Const.HeavyThrustingType),  // Queelign's
        new ItemLotWrapper(new ItemLotEntry(62510000, 2), Const.HeavyThrustingType), // Carian Thrusting Shield
        new ItemLotWrapper(new ItemLotEntry(62500000, 2), Const.HeavyThrustingType), // Thrusting Shield
        new ItemLotWrapper(new ItemLotEntry(14510000, 2), Const.AxeType), // Death Knight's Twin Axes
        new ItemLotWrapper(new ItemLotEntry(14540000, 2), Const.AxeType), // Forked Tongue Hatchet
        new ItemLotWrapper(new ItemLotEntry(21510000, 2), Const.FistType), // Pata
        new ItemLotWrapper(new ItemLotEntry(21520000, 2), Const.FistType), // Poisoned Hand
        new ItemLotWrapper(new ItemLotEntry(21540000, 2), Const.FistType), // Golem Fist
        new ItemLotWrapper(new ItemLotEntry(22500000, 2), Const.ClawType), // Claws of Night
        new ItemLotWrapper(new ItemLotEntry(4500000, 2), Const.ColossalSwordType), // Ancient Meteoric Ore
        new ItemLotWrapper(new ItemLotEntry(4520000, 2), Const.ColossalSwordType), // Fire Knight's Greatsword
        new ItemLotWrapper(new ItemLotEntry(12530000, 2), Const.ColossalWeaponType), // Bloodfiend's Arm
        new ItemLotWrapper(new ItemLotEntry(23500000, 2), Const.ColossalWeaponType), // Devonia's Hammer
        new ItemLotWrapper(new ItemLotEntry(66510000, 2), Const.KatanaType),  // Dragon-Hunters
        new ItemLotWrapper(new ItemLotEntry(66500000, 2), Const.KatanaType),  // Great Katana
        new ItemLotWrapper(new ItemLotEntry(66520000, 2), Const.KatanaType),  // Rakshasa's
        new ItemLotWrapper(new ItemLotEntry(9500000, 2), Const.KatanaType),   // Sword of Night
        new ItemLotWrapper(new ItemLotEntry(2520000, 2), Const.KatanaType),   // Star Lined Sword
        new ItemLotWrapper(new ItemLotEntry(67500000, 2), Const.GreatswordType), // Milady
        new ItemLotWrapper(new ItemLotEntry(67510000, 2), Const.GreatswordType), // Leda's Sword
        new ItemLotWrapper(new ItemLotEntry(3520000, 2), Const.GreatswordType),  // Lizard Greatsword
        new ItemLotWrapper(new ItemLotEntry(3550000, 2), Const.GreatswordType),  // Greatsword of Solitude
        new ItemLotWrapper(new ItemLotEntry(7500000, 2), Const.CurvedSwordType), // Spirit Sword
        new ItemLotWrapper(new ItemLotEntry(7510000, 2), Const.CurvedSwordType), // Falx
        new ItemLotWrapper(new ItemLotEntry(7520000, 2), Const.CurvedSwordType), // Dancing Blade of Ranah
        new ItemLotWrapper(new ItemLotEntry(7530000, 2), Const.CurvedSwordType), // Horned Warrior's Sword
        new ItemLotWrapper(new ItemLotEntry(41510000, 2), Const.BowType),        // Ansbach's Bow
        new ItemLotWrapper(new ItemLotEntry(40500000, 2), Const.LightBowType),   // Bone Bow
        new ItemLotWrapper(new ItemLotEntry(43500000, 2), Const.BowType),        // repeating crossbow
        new ItemLotWrapper(new ItemLotEntry(43510000, 2), Const.BowType),        // spread crossbow
        new ItemLotWrapper(new ItemLotEntry(2550000, 2), Const.StraightSwordType), // Sword of Light
        new ItemLotWrapper(new ItemLotEntry(2560000, 2), Const.StraightSwordType), // Sword of Darkness
        new ItemLotWrapper(new ItemLotEntry(2540000, 2), Const.StraightSwordType), // stone sheathed
        new ItemLotWrapper(new ItemLotEntry(64520000, 2), Const.CurvedGreatswordType), // Curseblade's Cirque
        new ItemLotWrapper(new ItemLotEntry(64500000, 2), Const.CurvedGreatswordType), // backhand blade
        new ItemLotWrapper(new ItemLotEntry(8510000, 2), Const.CurvedGreatswordType), // Freyja's Greatsword
        new ItemLotWrapper(new ItemLotEntry(8520000, 2), Const.CurvedGreatswordType), // Horned Warrior's Greatsword
        new ItemLotWrapper(new ItemLotEntry(18500000, 2), Const.HalberdType), // Spirit Glaive
        new ItemLotWrapper(new ItemLotEntry(11500000, 2), Const.HammerType),  // Flowerstone Gavel
        new ItemLotWrapper(new ItemLotEntry(19500000, 2), Const.ReaperType), // Obsidian Lamina
        new ItemLotWrapper(new ItemLotEntry(12520000, 2), Const.GreatHammerType), // Blacksteel Greathammer
        new ItemLotWrapper(new ItemLotEntry(15500000, 2), Const.GreataxeType), //  Death Knight's Longhaft Axe
        new ItemLotWrapper(new ItemLotEntry(17520000, 2), Const.GreatSpearType), // Barbed Staff-Spear
        new ItemLotWrapper(new ItemLotEntry(10510000, 2), Const.TwinbladeType), //  Blacksteel Twinblade
        new ItemLotWrapper(new ItemLotEntry(13500000, 2), Const.FlailType), //  Serpent Flail
    };

    public static List<int> CurvedSwordIDs = new List<int>()
    { // includes backhand blades
        2080000, 7050000, 7060000, 7070000, 7080000, 7120000, 7140000,
        7000000, 7010000, 7020000, 7030000, 7040000, 7100000, 7110000,
        7150000, 7500000, 7510000, 7520000, 7530000, // DLC
        64500000,   //    Backhand Blade
    };
    public static List<int> DaggerClawFistIDs = new List<int>() {
        1020000, 1000000, 1010000, 1030000, 1040000, 1050000,
        1070000, 1080000, 1090000, 1100000, 1110000, 1140000, 1150000,
        22000000, 22010000, 22020000, 22030000,
        21000000, 21010000, 21070000, 21080000, 21100000, 21110000, 21120000, 21130000,
        22500000, // Claws of Night
        21540000, 21520000, 21510000 // DLC
    };
    public static List<int> ClawIDs = new List<int>()
    { // includes Beast Claws
        22000000, 22010000, 22020000, 22030000,
        // 68500000, 68510000, 22500000, (DLC not desired at this time)
    };
    public static List<int> DaggerIDs = new List<int>()
    {
        1020000, 1000000, 1010000, 1030000, 1040000, 1050000,
        1070000, 1080000, 1090000, 1100000, 1110000, 1140000, 1150000,
        // 1500000, 1510000 (DLC not wanted at this time) 
    };
    public static List<int> FistIDs = new List<int>()
    { // includes hand arts
        21000000, 21010000, 21070000, 21080000, 21100000, 21110000, 21120000, 21130000, 
        // 21500000, 21510000, 21520000, 21530000, 21540000, 60500000, 60510000, (DLC not wanted at this time)
    };
    public static List<int> HeavyThrustingIDs = new List<int>()
    {   // includes Light Greatswords
        6000000, 6010000, 6020000,
        6500000, 67500000, 67510000, // (DLC not wanted at this time)
        62500000, 62510000, // dueling shields
    };
    public static List<int> KatanaIDs = new List<int>()
    {   // includes Great Katanas
        9000000, 9010000, 9030000, 9040000, 9060000, 9070000, 9080000,
        66510000, // Dragon-Hunter's
        66520000, // Rakshasa's
        66500000, // Great Katana
        9500000, // Sword of Night
        2520000, // star-lined sword
    };

    public static List<int> AxeIDs = new List<int>()
    {
        14000000, 14010000, 14020000, 14030000, 14040000, 14050000,
        14060000, 14080000, 14100000, 14110000, 14120000, 14140000,
        14540000, // (DLC not wanted at this time) 14520000, 14500000
        14510000, // Death Knight's Twin Axes
    };
    public static List<int> CurvedGreatSwordIDs = new List<int>()
    {
        8010000, 8020000, 8030000, 8040000, 8050000,
        8060000, 8070000, 8080000,
        8510000, 8520000, // DLC
        64520000,   // curseblade's cirque
    };
    public static List<int> ColossalSwordIDs = new List<int>()
    {
        4000000, 4010000, 4030000, 4040000, 4060000, 4070000,
        4080000, 4100000, 4110000,
        4500000, 4520000, 4540000, // DLC
    };
    public static List<int> ColossalWeaponIDs = new List<int>()
    {
        23000000, 23010000, 23020000, 23030000, 23040000, 23140000, 23150000,
        23060000, 23070000, 23080000, 23100000, 23110000, 23120000, 23130000,
        12510000, 12530000, 23500000, // (DLC not wanted at this time)
    };
    public static List<int> FlailIDs = new List<int>()
    {
        13000000, 13010000, 13020000, 13040000,
        // 13500000, // DLC
    };
    public static List<int> MerchantSpearIDs = new List<int>
    {
        17520000, 16550000, 17060000, 17050000, 16140000,
        16000000, 16520000, 16540000, 16050000, 16110000, 16030000,
    };
    public static List<int> GreatSpearIDs = new List<int>
    {   // serpent hunter is deliberately excluded 17030000
        17020000, 17050000, 17060000, 17070000,
        16550000, 17510000, 17520000 // (DLC)
    };
    public static List<int> GreataxeIDs = new List<int>
    {
        15000000, 15010000, 15020000, 15030000, 15050000, 15060000,
        15080000, 15120000, 15130000, 15140000,
        15500000, 15510000, // DLC
    };
    public static List<int> GreatHammerIDs = new List<int>
    {
        12000000, 12010000, 12020000, 12060000, 12080000, 12130000, 12140000,
        12150000, 12160000, 12170000, 12180000, 12190000, 12200000, 12210000,
        12500000, 12520000, // DLC
    };
    public static List<int> GreatswordIDs = new List<int>
    {
        2090000, 3000000, 3010000, 3020000, 3030000, 3040000, 3050000,
        3060000, 3070000, 3080000, 3090000, 3130000, 3150000, 3160000,
        3170000, 3180000, 3190000, 3200000, 3210000,
        3520000, 3550000, // (DLC)
        67500000, 67510000
    };
    public static List<int> HalberdIDs = new List<int>
    {
        18000000, 18010000, 18020000, 18030000, 18040000, 18050000, 18060000,
        18070000, 18080000, 18090000, 18100000, 18110000, 18130000, 18140000,
        18150000, 18160000,
        18500000, // (DLC)
    };
    public static List<int> HammerIDs = new List<int>
    {
        11000000, 11010000, 11030000, 11040000, 11050000, 11060000, 11070000,
        11080000, 11090000, 11100000, 11110000, 11120000, 11130000, 11140000,
        11500000, // (DLC)
    };
    public static List<int> ReaperIDs = new List<int>
    {
        19000000, 19060000, 19010000, 19020000,
        19500000, // (DLC)
    };
    public static List<int> SpearIDs = new List<int>
    {
        16000000, 16010000, 16020000, 16030000, 16040000, 16050000, 16060000, 16070000,
        16080000, 16090000, 16110000, 16120000, 16130000, 16140000, 16150000, 16160000,
        16500000, 16520000, 16540000, // (DLC)
    };
    public static List<int> StraightSwordIDs = new List<int>
    {
        2000000, 2010000, 2020000, 2040000, 2050000, 2060000, 2070000, 2110000, 2140000,
        2150000, 2180000, 2190000, 2200000, 2210000, 2220000, 2230000, 2240000, 2250000,
        2260000, 
//        2510000, 2540000, 2550000, 2560000, (DLC)
    };
    public static List<int> ThrustingSwordIDs = new List<int>
    {
        5000000, 5010000, 5020000, 5030000, 5040000, 5050000, 5060000,
        // 2530000, (DLC) 
    };
    public static List<int> TwinbladeIDs = new List<int>
    {
        10000000, 10010000, 10030000, 10050000, 10080000, 10090000,
        10500000, 10510000, // (DLC)
    };
    public static List<int> WhipIDs = new List<int>
    {
        20000000, 20020000, 20030000, 20050000, 20070000, 
        // 20500000, (DLC)
    };
    public static List<int> PerfumeBottleIDs = new List<int>
    {
        61500000, 61510000, 61520000, 61540000,
    };
    public static List<int> GreatShieldIDs = new List<int>
    {
        32000000, 32020000, 32030000, 32040000, 32050000, 32080000, 32090000, 32120000,
        32130000, 32140000, 32150000, 32160000, 32170000, 32190000, 32200000, 32210000,
        32220000, 32230000, 32240000, 32250000, 32260000, 32270000, 32280000, 32290000,
        // 32300000, 32500000, 32520000, (DLC)
    };
    public static List<int> MediumShieldIDs = new List<int>
    {
        31000000, 31010000, 31020000, 31030000, 31040000, 31050000, 31060000, 31070000,
        31080000, 31090000, 31100000, 31130000, 31140000, 31170000, 31190000, 31230000,
        31240000, 31250000, 31260000, 31270000, 31280000, 31290000, 31300000, 31310000,
        31320000, 31330000, 31340000, 
        // 31500000, 31510000, 31520000, 31530000, (DLC)
    };
    public static List<int> SmallShieldIDs = new List<int>
    {
        30000000, 30010000, 30020000, 30030000, 30040000, 30060000, 30070000,
        30080000, 30090000, 30100000, 30110000, 30120000, 30130000, 30140000, 30150000,
        30190000, 30200000, 
        // 30510000, 21550000, 
    };
    public static List<int> TorchIDs = new List<int>
    {
        24000000, 24040000, 24050000, 24060000, 24070000, 
        // 24500000, 24510000, 24020000, // (DLC preferable to not have at this time)
    };

    // not sure what to do with dueling shields yet: 62500000, 62510000,

    public static List<int> LightBowAndBowIDs = new List<int>
    {
        41000000, 41010000, 41020000, 41030000, 41040000, 41060000, 41070000,
        40000000, 40010000, 40020000, 40030000, 40050000, 
        // 40500000, 41510000, (DLC)
    };
    public static List<int> CrossBowIDs = new List<int>
    {
        43000000, 43020000, 43030000, 43050000, 43060000, 43080000, 43110000,
        // 43500000, 43510000, (DLC) 
    };
    public static List<int> BallistaOrGreatBowIDs = new List<int>
    { // includes throwing blades
        44000000, 44010000, 42010000, 42030000, 42040000, 
        // 42500000, 63500000, 44500000, (DLC)
    };
    public static List<int> DlcAndSalt = new List<int>
    {
        1500000,  // main gauche
        1020000,  // parrying dagger
        22500000, // Claws of Night
        68500000, // Beast Claw
        68510000, // Red Bear Beast Claw
        21540000, // Golem Fist
        21510000, // Pata
        23500000, // Devonia's Hammer
        4500000,  // Ancient Meteoric Ore
        40500000, // Bone Bow
        43500000, // Repeating Crossbow
        43510000, // Spread Crossbow
        41510000, // Ansbach's Bow
        30510000, // small shield
        21550000, // small shield
        16520000, // swift spear
        16540000, // bloodfiend's fork
        64520000, // Curseblade's Cirque
        7510000,  // Falx
        7520000,  // Dancing Blade of Ranah
        7530000,  // Horned Warrior's Sword 
        7080000,  // scavengers
        13500000, // Serpent Flail
        66500000, // Great Katana
        9500000, // Sword of Knight
        12520000, // Blacksteel Greathammer
        17520000, // Barbed Staff spear
        3520000, // Lizard Greatsword
        11500000, // Flowerstone Gavel
        67500000, // Milady
        67510000, // Leda's Sword
        19500000, // Obsidian Lomina
        31500000, // 
        31510000, //
        31520000, // 
        31530000, //
    };

    // SORCERIES
    static List<int> SorceryIDs = new List<int>()
    {
        4000, 4001, 4010, 4020, 4021, 4030, 4040, 4050, 4060, 4070,
        4080, 4090, 4100, 4110, 4120, 4130, 4140, 4200, 4210, 4220,
        4300, 4301, 4302, 4360, 4361, 4370, 4380, 4381, 4390, 4400,
        4410, 4420, 4430, 4431, 4440, 4450, 4460, 4470, 4480, 4490,
        4500, 4510, 4520, 4600, 4610, 4620, 4630, 4640, 5100, 5110,
        4650, 4660, 4670, 4700, 4701, 4710, 4720, 4721, 4800, 4810,
        4820, 4830, 4900, 4910, 5000, 5001, 5010, 5020, 5030, 6500, 
        // DLC
        2004300, 2004310, 2004320, 2004500, 2004510, 2004700, 2004710,
        2004900, 2004910, 2005000, 2006200, 2006210, 2007410, 2007420,
    };
    public static List<int> StartingSorceryIDs = new List<int>()
    {
        4000, 4010, 4040, 4460, 4470, 4660, 4070, 4390, 4440, 6500,
        4100, 4140, 4370, 4400, 4490, 4001, 4670, 5000, 4480, 4640,
        4720, 4090, 4610, 4630, 4710,
    };
    public static List<int> StartingIncantationIDs = new List<int>()
    {
        6000, 6420, 6820, 6220, 6850, 6010, 6460, 6510, 6800, 7230,
        7200, 7210, 6040, 6320, 6400, 6421, 6840, 7220, 6030, 6300,
        6520, 6700, 6770, 6810, 7010, 6050, 6422, 6830, 6960, 7000,
        7020, 7030, 7040, 6001, 7310,
    };
    public static List<int> StartingWeaponIDs = new List<int>()
    {   // TODO non-exhaustive list of weapons. Until level calc is working again a smaller list of weapons that are more likely to be equippable

        14000000, 14020000, 14030000, 14040000, 14060000, 14080000, 14100000, 14110000, 14120000, 14140000, // axes
        18000000, 18010000, 18020000, 18030000, 18040000, 18050000, 18070000, 18080000, 18110000, 18140000, // Halberds 

        2000000, 2010000, 2020000, 2040000, 2050000, 2060000, 2070000, 2110000, 2140000, 2150000,
        2180000, 2190000, 2200000, 2210000, 2220000, 2230000, 2240000, 2250000, 2260000, // straight swords

        2080000, 7050000, 7060000, 7070000, 7080000, 7120000, 7140000, 7000000, 7010000, 7020000,
        7030000, 7040000, 7100000, 7110000,                                              // curved swords
        11000000, 11010000, 11030000, 11040000, 11050000, 11060000, 11070000, 11080000, 11090000, 11100000, // hammers
        
        16000000, 16010000, 16020000, 16030000, 16040000, 16050000, 16060000, 16070000, 16080000, 16090000,
        16110000, 16120000, 16130000, 16140000, 16150000, 16160000,                             // spears

        3070000, 3180000, 3020000, 3030000, 3040000, 3050000, 3010000, 3190000, 3160000, 3200000, // great swords
        8010000, 8020000, 8030000, 8050000, 8060000, 19000000, 19010000, 19020000, 19060000, 6000000, // CGS / Scythe
        10000000, 10010000, 10030000, 10080000, 9000000, 9030000, 9040000, 9060000, 9070000, 9080000, // twin blades / katana
        23130000, 15050000, 15020000, 15060000, 12000000, 12020000, 12180000, 12010000, 4040000, 4000000, // troll's hammer, longhaft axe
    };

    // ARMOR
    public static List<int> HeadArmorIDs = new List<int>()
    {   // non-exhaustive list of armor, deliberate at this time
        //DLC 
        5253000, 5021000, 5180000, 5120000, 3010000, 5230000, 5320000, 5090000, 5130000,
        5060000, 5250000, 5100000, 5101000, 5330000, 5191000, 5190000, 5140000, 5220000,
        5221000, 5000000, 5020000, 5160000, 5260000, 5280000, 5300000, 5030000, 3000000,
        5010000, 5183000, 5110000, 5270000, 5150000, 5272000, 5210000, 5080000, 5200000,
        
        //BASE
        1060000, 370000, 380000, 581000, 650000, 1130000, 850000, 963000, 870000, 1080000,
        130000, 640000, 1880000, 580000, 770000, 250000, 1040000, 1840000, 801000, 460000,
        120000, 200000, 180000, 170000, 881000, 940000, 340000, 891000, 730000, 670000,
        651000, 540000, 140000, 210000, 230000, 860000, 300000, 1120000, 292000, 1300000,
        290000, 291000, 760000, 1401000, 980000, 811000, 571000, 231000, 150000,
    };
    public static List<int> BodyArmorIDs = new List<int>()
    {   // non-exhaustive list of armor, deliberate at this time
        //DLC
        5252100, 5253100, 3000100, 3001100, 5180100, 5181100, 5120100, 5121100, 3010100,
        5190100, 5231100, 5230100, 5060100, 5191100, 5250100, 5100100, 5101100, 5020100,
        5000100, 5140100, 5141100, 5220100, 5002100, 5160100, 5260100, 5280100, 5030100,
        5031100, 5010100, 5270100, 5271100, 5111100, 5131100, 5210100, 5090100, 5070100,
        5080100, 5081100, 5200100,

        //BASE
        670100, 171100, 240100, 1100100, 1930100, 940100, 80100, 1010100, 661100, 872100,
        1070100, 341100, 951100, 1040100, 311100, 331100, 1991100, 652100, 1400100, 181100,
        200100, 320100, 740100, 761100, 481100, 641100, 870100, 50100, 520100, 1050100,
        292100, 930100, 151100, 131100, 771100, 580100, 1740100, 911100, 861100,
    };
    public static List<int> ArmsArmorIDs = new List<int>()
    {   // non-exhaustive list of armor, deliberate at this time
        //DLC
        5253200, 3000200, 5180200, 5120200, 3010200, 5090200, 5130200, 5230200, 5060200,
        5250200, 5100200, 5020200, 5190200, 5140200, 5220200, 5000200, 5160200, 5260200,
        5280200, 5030200, 5010200, 5270200, 5200200,

        //BASE
        581200, 1000200, 351200, 790200, 730200, 880200, 460200, 990200, 360200, 1700200,
        1040200, 330200, 50200, 640200, 580200, 760200, 470200, 260200, 230200, 290200,
        150200, 872200, 670200, 1070200, 860200, 200200, 1100200, 120200
    };
    public static List<int> LegsArmorIDs = new List<int>()
    {   // non-exhaustive list of armor, deliberate at this time
        //DLC
        5253300, 3000300, 5180300, 5120300, 3010300, 5230300, 5090300, 5130300, 5060300, 5250300,
        5100300, 5020300, 5190300, 5140300, 5220300, 5000300, 5160300, 5260300, 5280300, 5030300,
        5010300, 5270300, 5240300, 5080300, 5200300, 5110300, 5150300, 

        //BASE
        120300, 730300, 340300, 880300, 940300, 460300, 300300, 520300, 530300, 650300,
        870300, 640300, 760300, 1130300, 470300, 320300, 600300, 872300, 90300, 150300,
        290300, 310300, 580300, 1040300, 1740300, 1070300, 540300, 330300, 620300, 390300,
        1010300, 280300, 130300, 860300, 1100300
     };
    public static IReadOnlyDictionary<byte, List<int>> ArmorLists = new Dictionary<byte, List<int>>()
    {
        { Const.HelmType, HeadArmorIDs },
        { Const.BodyType, BodyArmorIDs },
        { Const.ArmType, ArmsArmorIDs },
        { Const.LegType, LegsArmorIDs }
    };
    public static IReadOnlyDictionary<ushort, List<int>> WeaponSpellDropLists = new Dictionary<ushort, List<int>>()
    {
        { Const.DaggerType, DaggerIDs },
        { Const.StraightSwordType, StraightSwordIDs },
        { Const.GreatswordType, GreatswordIDs },
        { Const.ColossalSwordType, ColossalSwordIDs },
        { Const.CurvedSwordType, CurvedSwordIDs },
        { Const.CurvedGreatswordType, CurvedGreatSwordIDs },
        { Const.KatanaType, KatanaIDs },
        { Const.TwinbladeType, TwinbladeIDs },
        { Const.ThrustingSwordType, ThrustingSwordIDs },
        { Const.HeavyThrustingType, HeavyThrustingIDs },
        { Const.AxeType, AxeIDs },
        { Const.GreataxeType, GreataxeIDs },
        { Const.HammerType, HammerIDs },
        { Const.GreatHammerType, GreatHammerIDs },
        { Const.FlailType, FlailIDs },
        { Const.SpearType, SpearIDs },
        { Const.GreatSpearType, GreatSpearIDs },
        { Const.HalberdType, HalberdIDs },
        { Const.FistType, FistIDs },
        { Const.ClawType, ClawIDs },
        { Const.WhipType, WhipIDs },
        { Const.ColossalWeaponType, ColossalWeaponIDs },
        { Const.SmallShieldType, SmallShieldIDs },
        { Const.MediumShieldType, MediumShieldIDs },
        { Const.GreatShieldType, GreatShieldIDs },
        { Const.CrossbowType, CrossBowIDs },
        { Const.BowType, LightBowAndBowIDs },
        { Const.LightBowType, LightBowAndBowIDs },
        { Const.GreatbowType, BallistaOrGreatBowIDs },
        { Const.BallistaType, BallistaOrGreatBowIDs },
        // TODO add spells
        // { Const. },
        // { Const. },
    };

    public static IReadOnlyDictionary<int, string> EquipmentNameList = new Dictionary<int, string>()
    {
        { 63500000, "Smithscipt Dagger" },
        { 62500000, "Dueling Shield" },
        { 62510000, "Carian Thrusting Shield" },
        { 64500000, "Backhand Blade" },
        { 64510000, "Smithscript Cirque" },
        { 64520000, "Curseblade's Cirque" },
        { 66500000, "Great Katana" },
        { 66510000, "Dragon-Hunter's Great Katana" },
        { 66520000, "Rakshasa's Great Katana" },
        { 67500000, "Milady" },
        { 67510000, "Leda's Sword" },
        { 14000000, "Battle Axe" },
        { 14010000, "Forked Hatchet" },
        { 14020000, "Hand Axe" },
        { 14030000, "Jawbone Axe" },
        { 14040000, "Iron Cleaver" },
        { 14050000, "Ripple Blade" },
        { 14060000, "Celebrant's Cleaver" },
        { 14080000, "Icerind Hatchet" },
        { 14100000, "Highland Axe" },
        { 14110000, "Sacrificial Axe" },
        { 14120000, "Rosus' Axe" },
        { 14140000, "Stormhawk Axe" },
        { 14500000, "Smithscript Axe" },
        { 14510000, "Death Knight's Twin Axes" },
        { 14520000, "Messmer Soldier's Axe" },
        { 14540000, "Forked-Tongue Hatchet" },
        { 8010000, "Onyx Lord's Greatsword" },
        { 8020000, "Dismounter" },
        { 8030000, "Bloodhound's Fang" },
        { 8040000, "Magma Wyrm's Scalesword" },
        { 8050000, "Zamor Curved Sword" },
        { 8060000, "Omen Cleaver" },
        { 8070000, "Monk's Flameblade" },
        { 8080000, "Beastman's Cleaver" },
        { 8510000, "Freyja's Greatsword" },
        { 8520000, "Horned Warrior's Greatsword" },
        { 4000000, "Greatsword" },
        { 4010000, "Watchdog's Greatsword" },
        { 4030000, "Troll's Golden Sword" },
        { 4040000, "Zweihander" },
        { 4060000, "Royal Greatsword" },
        { 4070000, "Godslayer's Greatsword" },
        { 4080000, "Ruins Greatsword" },
        { 4100000, "Grafted Blade Greatsword" },
        { 4110000, "Troll Knight's Sword" },
        { 4500000, "Ancient Meteoric Ore Greatsword" },
        { 4520000, "Fire Knight's Greatsword" },
        { 4540000, "Moonrithyll's Knight Sword" },
        { 12510000, "Anvil Hammer" },
        { 12530000, "Bloodfiend's Arm" },
        { 23000000, "Prelate's Inferno Crozier" },
        { 23010000, "Watchdog's Staff" },
        { 23020000, "Great Club" },
        { 23030000, "Envoy's Greathorn" },
        { 23040000, "Duelist Greataxe" },
        { 23060000, "Dragon Greatclaw" },
        { 23070000, "Staff of the Avatar" },
        { 23080000, "Fallingstar Beast Jaw" },
        { 23100000, "Ghiza's Wheel" },
        { 23110000, "Giant-Crusher" },
        { 23120000, "Golem's Halberd" },
        { 23130000, "Troll's Hammer" },
        { 23140000, "Rotten Staff" },
        { 23150000, "Rotten Greataxe" },
        { 23500000, "Devonia's Hammer" },
        { 2080000, "Nox Flowing Sword" },
        { 7000000, "Falchion" },
        { 7010000, "Beastman's Curved Sword" },
        { 7020000, "Shotel" },
        { 7030000, "Shamshir" },
        { 7040000, "Bandit's Curved Sword" },
        { 7050000, "Magma Blade" },
        { 7060000, "Flowing Curved Sword" },
        { 7070000, "Wing of Astel" },
        { 7080000, "Scavenger's Curved Sword" },
        { 7100000, "Eclipse Shotel" },
        { 7110000, "Serpent-God's Curved Sword" },
        { 7120000, "Mantis Blade" },
        { 7140000, "Scimitar" },
        { 7150000, "Grossmesser" },
        { 7500000, "Spirit Sword" },
        { 7510000, "Falx" },
        { 7520000, "Dancing Blade of Ranah" },
        { 7530000, "Horned Warrior's Sword" },
        { 15000000, "Greataxe" },
        { 15010000, "Warped Axe" },
        { 15020000, "Great Omenkiller Cleaver" },
        { 15030000, "Crescent Moon Axe" },
        { 15050000, "Longhaft Axe" },
        { 15060000, "Rusted Anchor" },
        { 15080000, "Executioner's Greataxe" },
        { 15120000, "Butchering Knife" },
        { 15130000, "Gargoyle's Great Axe" },
        { 15140000, "Gargoyle's Black Axe" },
        { 15500000, "Death Knight's Longhaft Axe" },
        { 15510000, "Bonny Butchering Knife" },
        { 12000000, "Large Club" },
        { 12010000, "Greathorn Hammer" },
        { 12020000, "Battle Hammer" },
        { 12060000, "Great Mace" },
        { 12080000, "Curved Great Club" },
        { 12130000, "Celebrant's Skull" },
        { 12140000, "Pickaxe" },
        { 12150000, "Beastclaw Greathammer" },
        { 12160000, "Envoy's Long Horn" },
        { 12170000, "Cranial Vessel Candlestand" },
        { 12180000, "Great Stars" },
        { 12190000, "Brick Hammer" },
        { 12200000, "Devourer's Scepter" },
        { 12210000, "Rotten Battle Hammer" },
        { 12500000, "Smithscript Greathammer" },
        { 12520000, "Black Steel Greathammer" },
        { 16550000, "Bloodfiend's Sacred Spear" },
        { 17020000, "Siluria's Tree" },
        { 17050000, "Vyke's War Spear" },
        { 17060000, "Lance" },
        { 17070000, "Treespear" },
        { 17510000, "Messmer Soldier's Spear" },
        { 17520000, "Barbed Staff-Spear" },
        { 2090000, "Inseparable Sword" },
        { 3000000, "Bastard Sword" },
        { 3010000, "Forked Greatsword" },
        { 3020000, "Iron Greatsword" },
        { 3030000, "Lordsworn's Greatsword" },
        { 3040000, "Knight's Greatsword" },
        { 3050000, "Flamberge" },
        { 3060000, "Ordovis's Greatsword" },
        { 3070000, "Alabaster Lord's Sword" },
        { 3080000, "Banished Knight's Greatsword" },
        { 3090000, "Dark Moon Greatsword" },
        { 3130000, "Helphen's Steeple" },
        { 3150000, "Marais Executioner's Sword" },
        { 3160000, "Sword of Milos" },
        { 3170000, "Golden Order Greatsword" },
        { 3180000, "Claymore" },
        { 3190000, "Gargoyle's Greatsword" },
        { 3200000, "Death's Poker" },
        { 3210000, "Gargoyle's Blackblade" },
        { 3520000, "Lizard Greatsword" },
        { 3550000, "Greatsword of Solitude" },
        { 18000000, "Halberd" },
        { 18010000, "Pest's Glaive" },
        { 18020000, "Lucerne" },
        { 18030000, "Banished Knight's Halberd" },
        { 18040000, "Commander's Standard" },
        { 18050000, "Nightrider Glaive" },
        { 18060000, "Ripple Crescent Halberd" },
        { 18070000, "Vulgar Militia Saw" },
        { 18080000, "Golden Halberd" },
        { 18090000, "Glaive" },
        { 18100000, "Loretta's War Sickle" },
        { 18110000, "Guardian's Swordspear" },
        { 18130000, "Vulgar Militia Shotel" },
        { 18140000, "Dragon Halberd" },
        { 18150000, "Gargoyle's Halberd" },
        { 18160000, "Gargoyle's Black Halberd" },
        { 18500000, "Spirit Glaive" },
        { 11000000, "Mace" },
        { 11010000, "Club" },
        { 11030000, "Curved Club" },
        { 11040000, "Warpick" },
        { 11050000, "Morning Star" },
        { 11060000, "Varré's Bouquet" },
        { 11070000, "Spiked Club" },
        { 11080000, "Hammer" },
        { 11090000, "Monk's Flamemace" },
        { 11100000, "Envoy's Horn" },
        { 11110000, "Scepter of the All-Knowing" },
        { 11120000, "Nox Flowing Hammer" },
        { 11130000, "Ringed Finger" },
        { 11140000, "Stone Club" },
        { 11500000, "Flowerstone Gavel" },
        { 60500000, "Dryleaf Arts" },
        { 60510000, "Dane's Footwork" },
        { 6000000, "Bloody Helice" },
        { 6010000, "Godskin Stitcher" },
        { 6020000, "Great Épée" },
        { 6500000, "Queelign's Greatsword" },
        { 2520000, "Star-Lined Sword" },
        { 9000000, "Uchigatana" },
        { 9010000, "Nagakiba" },
        { 9030000, "Meteoric Ore Blade" },
        { 9040000, "Rivers of Blood" },
        { 9060000, "Moonveil" },
        { 9070000, "Dragonscale Blade" },
        { 9080000, "Serpentbone Blade" },
        { 9500000, "Sword of Night" },
        { 19000000, "Scythe" },
        { 19010000, "Grave Scythe" },
        { 19020000, "Halo Scythe" },
        { 19060000, "Winged Scythe" },
        { 19500000, "Obsidian Lamina" },
        { 16000000, "Short Spear" },
        { 16010000, "Spear" },
        { 16020000, "Crystal Spear" },
        { 16030000, "Clayman's Harpoon" },
        { 16040000, "Cleanrot Spear" },
        { 16050000, "Partisan" },
        { 16060000, "Celebrant's Rib-Rake" },
        { 16070000, "Pike" },
        { 16080000, "Torchpole" },
        { 16090000, "Bolt of Gransax" },
        { 16110000, "Cross-Naginata" },
        { 16120000, "Death Ritual Spear" },
        { 16130000, "Inquisitor's Girandole" },
        { 16140000, "Spiked Spear" },
        { 16150000, "Iron Spear" },
        { 16160000, "Rotten Crystal Spear" },
        { 16500000, "Smithscript Spear" },
        { 16520000, "Swift Spear" },
        { 16540000, "Bloodfiend's Fork" },
        { 2000000, "Longsword" },
        { 2010000, "Short Sword" },
        { 2020000, "Broadsword" },
        { 2040000, "Lordsworn's Straight Sword" },
        { 2050000, "Weathered Straight Sword" },
        { 2060000, "Ornamental Straight Sword" },
        { 2070000, "Golden Epitaph" },
        { 2110000, "Coded Sword" },
        { 2140000, "Sword of Night and Flame" },
        { 2150000, "Crystal Sword" },
        { 2180000, "Carian Knight's Sword" },
        { 2190000, "Sword of St. Trina" },
        { 2200000, "Miquellan Knight's Sword" },
        { 2210000, "Cane Sword" },
        { 2220000, "Regalia of Eochaid" },
        { 2230000, "Noble's Slender Sword" },
        { 2240000, "Warhawk's Talon" },
        { 2250000, "Lazuli Glintstone Sword" },
        { 2260000, "Rotten Crystal Sword" },
        { 2510000, "Velvet Sword of St. Trina" },
        { 2540000, "Stone-Sheathed Sword" },
        { 2550000, "Sword of Light" },
        { 2560000, "Sword of Darkness" },
        { 10000000, "Twinblade" },
        { 10010000, "Godskin Peeler" },
        { 10030000, "Twinned Knight Swords" },
        { 10050000, "Eleonora's Poleblade" },
        { 10080000, "Gargoyle's Twinblade" },
        { 10090000, "Gargoyle's Black Blades" },
        { 10500000, "Euporia" },
        { 10510000, "Black Steel Twinblade" },
        { 34000000, "Finger Seal" },
        { 34010000, "Godslayer's Seal" },
        { 34020000, "Giant's Seal" },
        { 34030000, "Gravel Stone Seal" },
        { 34040000, "Clawmark Seal" },
        { 34060000, "Golden Order Seal" },
        { 34070000, "Erdtree Seal" },
        { 34080000, "Dragon Communion Seal" },
        { 34090000, "Frenzied Flame Seal" },
        { 34500000, "Dryleaf Seal" },
        { 34510000, "Fire Knight's Seal" },
        { 34520000, "Spiraltree Seal" },
        { 5040, "Death Lightning" },
        { 6000, "Catch Flame" },
        { 6001, "O, Flame!" },
        { 6010, "Flame Sling" },
        { 6020, "Flame, Fall Upon Them" },
        { 6030, "Whirl, O Flame!" },
        { 6040, "Flame, Cleanse Me" },
        { 6050, "Flame, Grant Me Strength" },
        { 6060, "Flame, Protect Me" },
        { 6100, "Giantsflame Take Thee" },
        { 6110, "Flame of the Fell God" },
        { 6120, "Burn, O Flame!" },
        { 6210, "Black Flame" },
        { 6220, "Surge, O Flame!" },
        { 6230, "Scouring Black Flame" },
        { 6240, "Black Flame Ritual" },
        { 6250, "Black Flame Blade" },
        { 6260, "Black Flame's Protection" },
        { 6270, "Noble Presence" },
        { 6300, "Bloodflame Talons" },
        { 6310, "Bloodboon" },
        { 6320, "Bloodflame Blade" },
        { 6330, "Barrier of Gold" },
        { 6340, "Protection of the Erdtree" },
        { 6400, "Rejection" },
        { 6410, "Wrath of Gold" },
        { 6420, "Urgent Heal" },
        { 6421, "Heal" },
        { 6422, "Great Heal" },
        { 6423, "Lord's Heal" },
        { 6424, "Erdtree Heal" },
        { 6430, "Blessing's Boon" },
        { 6431, "Blessing of the Erdtree" },
        { 6440, "Cure Poison" },
        { 7310, "The Flame of Frenzy" },
        { 7230, "Poison Armament" },
        { 7220, "Poison Mist" },
        { 7210, "Swarm of Flies" },
        { 7200, "Pest Threads" },
        { 7040, "Glintstone Breath" },
        { 7030, "Rotten Breath" },
        { 7020, "Dragonice" },
        { 7010, "Magma Breath" },
        { 7000, "Dragonfire" },
        { 6960, "Electrify Armament" },
        { 6850, "Bestial Constitution" },
        { 6840, "Bestial Vitality" },
        { 6830, "Gurranq's Beast Claw" },
        { 6820, "Beast Claw" },
        { 6810, "Stone of Gurranq" },
        { 6800, "Bestial Sling" },
        { 6770, "Order's Blade" },
        { 6700, "Discus of Light" },
        { 6520, "Shadow Bait" },
        { 6510, "Assassin's Approach" },
        { 6460, "Magic Fortification" },
        { 33000000, "Glintstone Staff" },
        { 33040000, "Crystal Staff" },
        { 33050000, "Gelmir Glintstone Staff" },
        { 33060000, "Demi-Human Queen's Staff" },
        { 33090000, "Carian Regal Scepter" },
        { 33120000, "Digger's Staff" },
        { 33130000, "Astrologer's Staff" },
        { 33170000, "Carian Glintblade Staff" },
        { 33180000, "Prince of Death's Staff" },
        { 33190000, "Albinauric Staff" },
        { 33200000, "Academy Glintstone Staff" },
        { 33210000, "Carian Glintstone Staff" },
        { 33230000, "Azur's Glintstone Staff" },
        { 33240000, "Lusat's Glintstone Staff" },
        { 33250000, "Meteorite Staff" },
        { 33260000, "Staff of the Guilty" },
        { 33270000, "Rotten Crystal Staff" },
        { 33280000, "Staff of Loss" },
        { 33520000, "Maternal Staff" },
        { 4000, "Glintstone Pebble" },
        { 4001, "Great Glintstone Shard" },
        { 4010, "Swift Glintstone Shard" },
        { 4040, "Glintstone Stars" },
        { 4070, "Glintstone Arc" },
        { 4090, "Crystal Burst" },
        { 4100, "Shatter Earth" },
        { 4130, "Terra Magica" },
        { 4140, "Starlight" },
        { 4370, "Magic Downpour" },
        { 4390, "Magic Glintblade" },
        { 4400, "Glintstone Icecrag" },
        { 4420, "Freezing Mist" },
        { 4440, "Carian Slicer" },
        { 4460, "Scholar's Armament" },
        { 4470, "Scholar's Shield" },
        { 4480, "Lucidity" },
        { 4490, "Frozen Armament" },
        { 4610, "Night Shard" },
        { 4630, "Thops's Barrier" },
        { 4640, "Carian Retaliation" },
        { 4660, "Unseen Blade" },
        { 4670, "Unseen Form" },
        { 4710, "Rock Sling" },
        { 4720, "Gravity Well" },
        { 4800, "Magma Shot" },
        { 4900, "Briars of Sin" },
        { 4910, "Briars of Punishment" },
        { 5000, "Rancorcall" },
        { 5100, "Oracle Bubbles" },
        { 6500, "Night Maiden's Mist" },
        { 24000000, "Torch" },
        { 24020000, "Steel-Wire Torch" },
        { 24040000, "St. Trina's Torch" },
        { 24050000, "Ghostflame Torch" },
        { 24060000, "Beast-Repellent Torch" },
        { 24070000, "Sentry's Torch" },
        { 24500000, "Nanaya's Torch" },
        { 24510000, "Lamenting Visage" },
        { 1000000, "Dagger" },
        { 1010000, "Black Knife" },
        { 1020000, "Parrying Dagger" },
        { 1030000, "Miséricorde" },
        { 1040000, "Reduvia" },
        { 1050000, "Crystal Knife" },
        { 1060000, "Celebrant's Sickle" },
        { 1070000, "Glintstone Kris" },
        { 1080000, "Scorpion's Stinger" },
        { 1090000, "Great Knife" },
        { 1100000, "Wakizashi" },
        { 1110000, "Cinquedea" },
        { 1130000, "Ivory Sickle" },
        { 1140000, "Bloodstained Dagger" },
        { 1150000, "Erdsteel Dagger" },
        { 1160000, "Blade of Calling" },
        { 1500000, "Main-gauche" },
        { 1510000, "Fire Knight Shortsword" },
        { 21000000, "Caestus" },
        { 21010000, "Spiked Caestus" },
        { 21070000, "Iron Ball" },
        { 21080000, "Star Fist" },
        { 21100000, "Katar" },
        { 21110000, "Clinging Bone" },
        { 21120000, "Veteran's Prosthesis" },
        { 21130000, "Cipher Pata" },
        { 21500000, "Thiollier's Hidden Needle" },
        { 21510000, "Pata" },
        { 21520000, "Poisoned Hand" },
        { 21530000, "Madding Hand" },
        { 21540000, "Golem Fist" },
        { 13000000, "Nightrider Flail" },
        { 13010000, "Flail" },
        { 13020000, "Family Heads" },
        { 13040000, "Chainlink Flail" },
        { 13500000, "Serpent Flail" },
        { 20000000, "Whip" },
        { 20020000, "Thorned Whip" },
        { 20030000, "Magma Whip Candlestick" },
        { 20050000, "Hoslow's Petal Whip" },
        { 20070000, "Urumi" },
        { 20500000, "Tooth Whip" },
        { 61500000, "Firespark Perfume Bottle" },
        { 61510000, "Chilling Perfume Bottle" },
        { 61520000, "Frenzyflame Perfume Bottle" },
        { 61530000, "Lightning Perfume Bottle" },
        { 61540000, "Deadly Poison Perfume Bottle" },
        { 22000000, "Hookclaws" },
        { 22010000, "Venomous Fang" },
        { 22020000, "Bloodhound Claws" },
        { 22030000, "Raptor Talons" },
        { 22500000, "Claws of Night" },
        { 68500000, "Beast Claw" },
        { 68510000, "Red Bear's Claw" },
        { 2530000, "Carian Sorcery Sword" },
        { 5000000, "Estoc" },
        { 5010000, "Cleanrot Knight's Sword" },
        { 5020000, "Rapier" },
        { 5030000, "Rogier's Rapier" },
        { 5040000, "Antspur Rapier" },
        { 5050000, "Frozen Needle" },
        { 5060000, "Noble's Estoc" },
        { 32000000, "Dragon Towershield" },
        { 32020000, "Distinguished Greatshield" },
        { 32030000, "Crucible Hornshield" },
        { 32040000, "Dragonclaw Shield" },
        { 32050000, "Briar Greatshield " },
        { 32080000, "Erdtree Greatshield" },
        { 32090000, "Golden Beast Crest Shield" },
        { 32120000, "Jellyfish Shield" },
        { 32130000, "Fingerprint Stone Shield" },
        { 32140000, "Icon Shield" },
        { 32150000, "One-Eyed Shield" },
        { 32160000, "Visage Shield" },
        { 32170000, "Spiked Palisade Shield" },
        { 32190000, "Manor Towershield" },
        { 32200000, "Crossed-Tree Towershield" },
        { 32210000, "Inverted Hawk Towershield" },
        { 32220000, "Ant's Skull Plate" },
        { 32230000, "Redmane Greatshield" },
        { 32240000, "Eclipse Crest Greatshield" },
        { 32250000, "Cuckoo Greatshield" },
        { 32260000, "Golden Greatshield" },
        { 32270000, "Gilded Greatshield" },
        { 32280000, "Haligtree Crest Greatshield" },
        { 32290000, "Wooden Greatshield" },
        { 32300000, "Lordsworn's Shield" },
        { 32500000, "Black Steel Greatshield" },
        { 32520000, "Verdigris Greatshield" },
        { 31000000, "Kite Shield" },
        { 31010000, "Marred Leather Shield" },
        { 31020000, "Marred Wooden Shield" },
        { 31030000, "Banished Knight's Shield" },
        { 31040000, "Albinauric Shield" },
        { 31050000, "Sun Realm Shield" },
        { 31060000, "Silver Mirrorshield" },
        { 31070000, "Round Shield" },
        { 31080000, "Scorpion Kite Shield" },
        { 31090000, "Twinbird Kite Shield" },
        { 31100000, "Blue-Gold Kite Shield" },
        { 31130000, "Brass Shield" },
        { 31140000, "Great Turtle Shell" },
        { 31170000, "Shield of the Guilty" },
        { 31190000, "Carian Knight's Shield" },
        { 31230000, "Large Leather Shield" },
        { 31240000, "Horse Crest Wooden Shield" },
        { 31250000, "Candletree Wooden Shield" },
        { 31260000, "Flame Crest Wooden Shield" },
        { 31270000, "Hawk Crest Wooden Shield" },
        { 31280000, "Beast Crest Heater Shield" },
        { 31290000, "Red Crest Heater Shield" },
        { 31300000, "Blue Crest Heater Shield" },
        { 31310000, "Eclipse Crest Heater Shield" },
        { 31320000, "Inverted Hawk Heater Shield" },
        { 31330000, "Heater Shield" },
        { 31340000, "Black Leather Shield" },
        { 31500000, "Messmer Soldier Shield" },
        { 31510000, "Wolf Crest Shield" },
        { 31520000, "Serpent Crest Shield" },
        { 31530000, "Golden Lion Shield" },
        { 21550000, "Shield of Night" },
        { 30000000, "Buckler" },
        { 30010000, "Perfumer's Shield" },
        { 30020000, "Man-Serpent's Shield" },
        { 30030000, "Rickety Shield" },
        { 30040000, "Pillory Shield" },
        { 30060000, "Beastman's Jar-Shield" },
        { 30070000, "Red Thorn Roundshield" },
        { 30080000, "Scripture Wooden Shield" },
        { 30090000, "Riveted Wooden Shield" },
        { 30100000, "Blue-White Wooden Shield" },
        { 30110000, "Rift Shield" },
        { 30120000, "Iron Roundshield" },
        { 30130000, "Gilded Iron Shield" },
        { 30140000, "Ice Crest Shield" },
        { 30150000, "Smoldering Shield" },
        { 30190000, "Spiralhorn Shield" },
        { 30200000, "Coil Shield" },
        { 30510000, "Smithscript Shield" },
        { 40000000, "Shortbow" },
        { 40010000, "Misbegotten Shortbow" },
        { 40020000, "Red Branch Shortbow" },
        { 40030000, "Harp Bow" },
        { 40050000, "Composite Bow" },
        { 40500000, "Bone Bow" },
        { 41510000, "Ansbach's Bow" },
        { 41000000, "Longbow" },
        { 41010000, "Albinauric Bow" },
        { 41020000, "Horn Bow" },
        { 41030000, "Erdtree Bow" },
        { 41040000, "Serpent Bow" },
        { 41060000, "Pulley Bow" },
        { 41070000, "Black Bow" },
        { 42010000, "Golem Greatbow" },
        { 42030000, "Erdtree Greatbow" },
        { 42040000, "Greatbow" },
        { 42500000, "Igon's Greatbow" },
        { 43500000, "Repeating Crossbow" },
        { 43510000, "Spread Crossbow" },
        { 43000000, "Soldier's Crossbow" },
        { 43020000, "Light Crossbow" },
        { 43030000, "Heavy Crossbow" },
        { 43050000, "Pulley Crossbow" },
        { 43060000, "Full Moon Crossbow" },
        { 43080000, "Arbalest" },
        { 43110000, "Crepus's Black-Key Crossbow" },
        { 44500000, "Rabbath's Cannon" },
        { 44000000, "Hand Ballista" },
        { 44010000, "Jar Cannon" },
        // { , "" },
    };
}