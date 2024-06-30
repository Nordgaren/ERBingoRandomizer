using System.Collections.Generic;

namespace Project.Settings;

public static class Equipment
{   // TODO make into an initialization method for repeat randomizer calls
    // WEAPONS
    public static List<int> RemembranceWeaponIDs = new List<int>()
    {
        3500000, 17500000, 23510000, 67520000, 18510000, 4530000, 4550000, 3510000,
        33510000, 8500000, 15110000, 23520000, 13030000, 20060000, 3140000, 4050000,
        8100000, 42000000, 17010000, 9020000, 4020000, 15040000, 23050000, 6040000,
        11150000, 3100000, 21060000,
    };
    public static List<int> ClawIDs = new List<int>()
    { // includes Beast Claws
        22000000, 22010000, 22020000, 22030000, 22500000,
        68500000, 68510000,
    };
    public static List<int> CurvedSwordIDs = new List<int>()
    { // includes backhand blades
        2080000, 7000000, 7010000, 7020000, 7030000, 7040000, 7050000,
        7060000, 7070000, 7080000, 7100000, 7110000, 7120000, 7140000,
        7150000, 7500000, 7510000, 7520000, 7530000,
        64500000, 64510000, 64520000,
    };
    public static List<int> DaggerIDs = new List<int>()
    { // includes throwing blades
        1000000, 1010000, 1020000, 1030000, 1040000, 1050000, 1060000, 1070000,
        1080000, 1090000, 1100000, 1110000, 1130000, 1140000, 1150000, 1160000,
        1500000, 1510000, 63500000,
    };
    public static List<int> FistIDs = new List<int>()
    { // includes hand arts
        21000000, 21010000, 21070000, 21080000, 21100000, 21110000, 21120000,
        21130000, 21500000, 21510000, 21520000, 21530000, 21540000,
        60500000, 60510000,
    };
    public static List<int> HeavyThrustingIDs = new List<int>()
    {   // includes Light Greatswords
        6000000, 6010000, 6020000, 6500000,
        67500000, 67510000,
    };
    public static List<int> KatanaIDs = new List<int>()
    {   // includes Great Katanas
        2520000, 9000000, 9010000, 9030000, 9040000,
        9060000, 9070000, 9080000, 9500000,
        66500000, 66510000, 66520000,
    };
    public static List<int> AxeIDs = new List<int>()
    {
        14000000, 14010000, 14020000, 14030000, 14040000, 14050000, 14060000, 14080000,
        14100000, 14110000, 14120000, 14140000, 14500000, 14510000, 14520000, 14540000,
    };
    public static List<int> CurvedGreatSwordIDs = new List<int>()
    {
        8010000, 8020000, 8030000, 8040000, 8050000,
        8060000, 8070000, 8080000, 8510000, 8520000,
    };
    public static List<int> ColossalSwordIDs = new List<int>()
    {
        4000000, 4010000, 4030000, 4040000, 4060000, 4070000,
        4080000, 4100000, 4110000, 4500000, 4520000, 4540000,
    };
    public static List<int> ColossalWeaponIDs = new List<int>()
    {
        12510000, 12530000, 23000000, 23010000, 23020000, 23030000, 23040000,
        23060000, 23070000, 23080000, 23100000, 23110000, 23120000, 23130000,
        23140000, 23150000, 23500000,
    };
    public static List<int> FlailIDs = new List<int>()
    {
        13000000, 13010000, 13020000, 13040000, 13500000,
    };
    public static List<int> GreatSpearIDs = new List<int>
    {   // serpent hunter is deliberately excluded 17030000
        16550000, 17020000, 17050000, 17060000, 17070000, 17510000, 17520000
    };
    public static List<int> GreataxeIDs = new List<int>
    {
        15000000, 15010000, 15020000, 15030000, 15050000, 15060000,
        15080000, 15120000, 15130000, 15140000, 15500000, 15510000,
    };
    public static List<int> GreatHammerIDs = new List<int>
    {
        12000000, 12010000, 12020000, 12060000, 12080000, 12130000, 12140000,
        12150000, 12160000, 12170000, 12180000, 12190000, 12200000, 12210000,
        12500000, 12520000,
    };
    public static List<int> GreatswordIDs = new List<int>
    { // TODO testing
        2090000, 3000000, 3010000, 3020000, 3030000, 3040000, 3050000,
        3060000, 3070000, 3080000, 3090000, 3130000, 3150000, 3160000,
        3170000, 3180000, 3190000, 3200000, 3210000, 3520000, 3550000,
    };
    public static List<int> HalberdIDs = new List<int>
    {
        18000000, 18010000, 18020000, 18030000, 18040000, 18050000, 18060000,
        18070000, 18080000, 18090000, 18100000, 18110000, 18130000, 18140000,
        18150000, 18160000, 18500000,
    };
    public static List<int> HammerIDs = new List<int>
    {
        11000000, 11010000, 11030000, 11040000, 11050000, 11060000, 11070000,
        11080000, 11090000, 11100000, 11110000, 11120000, 11130000, 11140000,
        11500000,
    };
    public static List<int> ReaperIDs = new List<int>
    {
        19000000, 19010000, 19020000, 19060000, 19500000,
    };
    public static List<int> SpearIDs = new List<int>
    {
        16000000, 16010000, 16020000, 16030000, 16040000, 16050000, 16060000, 16070000,
        16080000, 16090000, 16110000, 16120000, 16130000, 16140000, 16150000, 16160000,
        16500000, 16520000, 16540000,
    };
    public static List<int> StraightSwordIDs = new List<int>
    {
        2000000, 2010000, 2020000, 2040000, 2050000, 2060000, 2070000, 2110000, 2140000,
        2150000, 2180000, 2190000, 2200000, 2210000, 2220000, 2230000, 2240000, 2250000,
        2260000, 2510000, 2540000, 2550000, 2560000,
    };
    public static List<int> ThrustingSwordIDs = new List<int>
    {
        2530000, 5000000, 5010000, 5020000, 5030000, 5040000, 5050000, 5060000,
    };
    public static List<int> TwinbladeIDs = new List<int>
    {
        10000000, 10010000, 10030000, 10050000, 10080000, 10090000, 10500000, 10510000,
    };
    public static List<int> WhipIDs = new List<int>
    {
        20000000, 20020000, 20030000, 20050000, 20070000, 20500000,
    };
    public static List<int> PerfumeBottleIDs = new List<int>
    {
        61500000, 61510000, 61520000, 61520000, 61540000,
    };
    public static List<int> GreatShieldIDs = new List<int>
    {
        32000000, 32020000, 32030000, 32040000, 32050000, 32080000, 32090000, 32120000,
        32130000, 32140000, 32150000, 32160000, 32170000, 32190000, 32200000, 32210000,
        32220000, 32230000, 32240000, 32250000, 32260000, 32270000, 32280000, 32290000,
        32300000, 32500000, 32520000,
    };
    public static List<int> MediumShieldIDs = new List<int>
    {
        31000000, 31010000, 31020000, 31030000, 31040000, 31050000, 31060000, 31070000,
        31080000, 31090000, 31100000, 31130000, 31140000, 31170000, 31190000, 31230000,
        31240000, 31250000, 31260000, 31270000, 31280000, 31290000, 31300000, 31310000,
        31320000, 31330000, 31340000, 31500000, 31510000, 31520000, 31530000,
    };
    public static List<int> SmallShieldIDs = new List<int>
    {
        21550000, 30000000, 30010000, 30020000, 30030000, 30040000, 30060000, 30070000,
        30080000, 30090000, 30100000, 30110000, 30120000, 30130000, 30140000, 30150000,
        30190000, 30200000, 30510000,
    };
    public static List<int> TorchIDs = new List<int>
    {
        24000000, 24020000, 24040000, 24050000, 24060000, 24070000, 24500000, 24510000,
    };

    // not sure what to do with dueling shields yet: 62500000, 62510000,

    public static List<int> LightBowAndBowIDs = new List<int>
    {
        41510000, 41000000, 41010000, 41020000, 41030000, 41040000, 41060000, 41070000,
        40000000, 40010000, 40020000, 40030000, 40050000, 40500000,
    };
    public static List<int> CrossBowIDs = new List<int>
    {
        43500000, 43510000, 43000000, 43020000, 43030000, 43050000, 43060000, 43080000, 43110000,
    };
    public static List<int> BallistaOrGreatBowIDs = new List<int>
    {
        44500000, 44000000, 44010000, 42010000, 42030000, 42040000, 42500000,
    };
    public static IReadOnlyList<List<int>> WeaponShopLists = new List<List<int>>()
    {
        ClawIDs, CurvedSwordIDs, DaggerIDs, FistIDs, HeavyThrustingIDs,
        KatanaIDs, AxeIDs, CurvedGreatSwordIDs, ColossalSwordIDs, ColossalWeaponIDs,
        FlailIDs, GreatSpearIDs, GreataxeIDs, GreatHammerIDs, GreatswordIDs,
        HalberdIDs, HammerIDs, ReaperIDs, SpearIDs, StraightSwordIDs,
        ThrustingSwordIDs, TwinbladeIDs, GreatShieldIDs, MediumShieldIDs, SmallShieldIDs,
        WhipIDs, PerfumeBottleIDs, LightBowAndBowIDs, BallistaOrGreatBowIDs,
    };
    // INCANTATIONS

    // SORCERIES
    static List<int> SorceryIDs = new List<int>()
    {
        4000, 4001, 4010, 4020, 4021,
        // glintstone pebble, great glinstone shard, swift glinstone shard, glintstone comet shard, comet 
        4030, 4040, 4050, 4060, 4070, 
        // shard spiral, glinstone stars 
        4080, 4090, 4100, 4110, 4120,
        4130, 4140, 4200, 4210, 4220,
        4300, 4301, 4302, 4360, 4361,
        4370, 4380, 4381, 4390, 4400,
        4410, 4420, 4430, 4431, 4440,
        4450, 4460, 4470, 4480, 4490,
        4500, 4510, 4520, 4600, 4610,
        4620, 4630, 4640, 5100, 5110,
        4650, 4660, 4670, 4700, 4701,
        4710, 4720, 4721, 4800, 4810,
        4820, 4830, 4900, 4910, 5000,
        5001, 5010, 5020, 5030, 6500, 
        // DLC
        2004300, 2004310, 2004320, 2004500,
        2004510, 2004700, 2004710, 2004900,
        2004910, 2005000, 2006200, 2006210,
        2007410, 2007420,
    };
    public static List<int> StartingWeaponIDs = new List<int>()
    {   // TODO non-exhaustive list of weapons. Until level calc is working again a smaller list of weapons that are more likely to be equippable
        // DLC
        19500000, 6500000, 9500000, 2510000, 2520000, 2530000, 2540000, 2550000, 2560000, 4520000,
        1510000, 1500000, 63500000, 14540000, 14500000, 14520000, 14510000, 64500000, 64510000, 64520000, // daggers, axes, backhand blades 
        22500000, 60500000, 60510000, 68500000, 68510000, 21500000, 21510000, 21520000, 21530000, 21540000, // claws / hand arts / beast claws / fist
        7530000, 7520000, 7510000, 7500000, 66500000, 66510000, 66520000, 3520000, 3550000, 18500000, // greatsword / halberd / curved / great katana
        12500000, 12520000, 16550000, 17510000, 17520000, 67500000, 67510000, 15500000,  8520000, 8510000,   // greathammer / greatspears / light greatswords / GCS
        4540000, 4500000, 20500000, 62500000, 62510000, 16540000, 16520000, 10510000,
        
        // Base Game
        14000000, 14010000, 14020000, 14030000, 14040000, 14060000, 14080000, 14100000, 14110000,
        14120000, 14140000, 23130000, 8020000, 8060000, 8030000, 8010000, 8050000, 4040000, 4000000,
        7000000, 7010000, 7030000, 7040000, 7050000, 7080000, 7110000, 7120000, 7140000, 7150000,
        15050000, 15020000, 15060000, 12000000, 12020000, 12140000, 12130000, 12180000, 12010000,
        17060000, 18110000, 18140000, 18000000, 18010000, 18020000, 18030000, 18040000, 18070000, 18080000,
        3070000, 3180000, 3020000, 3030000, 3040000, 3050000, 3010000, 3190000, 3160000, 3200000,
        10000000, 10010000, 10030000, 10080000, 19000000, 19010000, 19020000, 19060000, 6000000, 6020000,
        2060000, 2070000, 2000000, 2010000, 2020000, 2040000, 2050000, 2200000, 2210000, 2230000, 2250000, 2260000,
        11000000, 11010000, 11030000, 11040000, 11050000, 11060000, 11070000, 11080000, 11090000,
        11100000, 11110000, 11120000, 11130000, 16000000, 16050000, 16140000, 16030000, 16080000,
        9000000, 9030000, 9040000, 9060000, 9070000, 9080000,
    };
    public static List<int> SideWeaponIDs = new List<int>()
    {
        // torches, perfume bottles (10)
        24000000, 24020000, 24050000, 24060000, 24070000, 61500000, 61510000, 61520000, 61520000, 61540000,
        // hook claws / fists (10)
        22000000, 22010000, 22020000, 22030000, 21000000, 21010000, 21070000, 21080000, 21100000, 21120000, 
        // great shield (10)
        32030000, 32040000, 32050000, 32080000, 32090000, 32120000, 32130000, 32150000, 32220000, 32230000,
        // medium shield (10)
        31040000, 31050000, 31080000, 31140000, 31280000, 31290000, 31300000, 31320000, 31330000, 31310000,
        // small shields (10)
        30000000, 30010000, 30030000, 30060000, 30070000, 30080000, 30090000, 30110000, 30130000, 30200000,
        // whips (10) 1 sword of st. trina, thrusting swords
        20070000, 20000000, 20020000, 20050000, 2190000, 5050000, 5000000, 5010000, 5020000, 5040000,
        // ranged weapons (23)
        43500000, 43510000, 43000000, 43020000, 43030000, 43050000, 43060000, 43080000, 43110000, 41510000,
        41000000, 41010000, 41020000, 41030000, 41040000, 41060000, 41070000, 40000000, 40010000, 40020000,
        40030000, 40050000, 40500000,
        // daggers (12)
        1000000, 1020000, 1030000, 1040000, 1050000, 1060000, 1070000, 1080000, 1090000, 1100000,
        1130000, 1140000,
    };
    // ARMOR
    public static List<int> HeadArmorIDs = new List<int>()
    {   // non-exhaustive list of armor, deliberate at this time
        //DLC 
        5253000, 5021000, 5180000, 5120000, 3010000, 5230000, 5320000, 5090000, 5130000,
        5060000, 5250000, 5100000, 5101000, 5330000, 5191000, 5190000, 5140000, 5220000,
        5221000, 5000000, 5020000, 5160000, 5260000, 5184000, 5280000, 5300000, 5030000,
        5010000, 5183000, 5110000, 5270000, 5150000, 5272000, 5210000, 5080000, 5200000,
        3000000, 
        
        //BASE
        1060000, 370000, 380000, 581000, 650000, 1130000, 850000, 963000, 870000, 1080000,
        130000, 640000, 1880000, 580000, 770000, 250000, 1040000, 1840000, 801000, 460000,
        120000, 200000, 180000, 170000, 881000, 940000, 340000, 891000, 730000, 670000,
        651000, 540000, 140000, 210000, 230000, 860000, 300000, 1120000, 292000,
        290000, 291000,
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
        150200, 872200, 670200, 1070200, 860200, 200200,
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
        1010300, 280300, 130300, 860300,
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
    {   // TODO fill

    };
}
