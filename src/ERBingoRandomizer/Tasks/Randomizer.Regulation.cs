using Project.FileHandler;
using Project.Params;
using Project.Settings;
using Project.Utility;
using FSParam;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project.Tasks;

public partial class Randomizer
{
    public SeedInfo SeedInfo { get; private set; }
    private readonly string _seed;
    private readonly Random _random;
    private Param _itemLotParam_map;
    private Param _itemLotParam_enemy;
    private Param _shopLineupParam;
    private Param _atkParam_Pc;
    private Param _equipMtrlSetParam;
    // Dictionaries
    private Dictionary<int, EquipParamWeapon> _weaponDictionary;
    private Dictionary<int, EquipParamWeapon> _customWeaponDictionary;
    private Dictionary<int, string> _weaponNameDictionary;
    private Dictionary<int, EquipParamGoods> _goodsDictionary;
    private Dictionary<int, Magic> _magicDictionary;
    private Dictionary<ushort, List<Param.Row>> _weaponTypeDictionary;
    private Dictionary<byte, List<Param.Row>> _armorTypeDictionary;
    private Dictionary<byte, List<Param.Row>> _magicTypeDictionary;
    public Task RandomizeRegulation()
    {
        _randomizerLog = new List<string>();
        randomizeStartingClassParams();
        _cancellationToken.ThrowIfCancellationRequested();
        randomizeItemLotParams();
        _cancellationToken.ThrowIfCancellationRequested();
        randomizeShopLineupParam();
        _cancellationToken.ThrowIfCancellationRequested();
        randomizeShopLineupParamMagic();
        _cancellationToken.ThrowIfCancellationRequested();
        patchAtkParam();
        _cancellationToken.ThrowIfCancellationRequested();
        patchSmithingStones();
        _cancellationToken.ThrowIfCancellationRequested();
        writeFiles();
        writeLog();
        SeedInfo = new SeedInfo(_seed, Util.GetShaRegulation256Hash());
        string seedJson = JsonSerializer.Serialize(SeedInfo);
        File.WriteAllText(Config.LastSeedPath, seedJson);
        return Task.CompletedTask;
    }
    private void randomizeStartingClassParams()
    {
        logItem("Starting Class Randomization");
        logItem($"Seed: {_seed}");
        logItem("Level estimate (x) appears if you cannot wield the weapon, assumes you are benefiting from two-handing.");

        List<Param.Row> staves = _weaponTypeDictionary[Const.StaffType];
        List<Param.Row> seals = _weaponTypeDictionary[Const.SealType];
        IEnumerable<int> remembranceItems = _shopLineupParam.Rows.Where(r => r.ID is >= 101900 and <= 101929)
            .Select(r => new ShopLineupParam(r).equipId);

        List<int> spells = _magicDictionary.Keys.Select(id => id).Distinct()
            .Where(id => remembranceItems.All(r => r != id))
            .Where(id => staves.All(s => s.ID != id) && seals.All(s => s.ID != id))
            .ToList();
        spells.Shuffle(_random); // TODO investigate if thise matters

        for (int i = 0; i < Config.NumberOfClasses; i++)
        {
            Param.Row? row = _charaInitParam[Config.FirstClassId + i];
            if (row == null)
            { continue; }

            int index = _random.Next(Equipment.SideWeaponLists.Count);
            List<int> sideArms = Equipment.SideWeaponLists[index];
            index = _random.Next(Equipment.MainWeaponLists.Count);
            List<int> main = Equipment.MainWeaponLists[index];
            // List<int> main = Equipment.StartingWeaponIDs;

            CharaInitParam startingClass = new(row);
            randomizeEquipment(startingClass, main, sideArms);
            allocateStatsAndSpells(row.ID, startingClass, spells);
            logCharaInitEntry(startingClass, i + 288100); // TODO update config
            addDescriptionString(startingClass, Const.ChrInfoMapping[i]);
        }
    }
    private void randomizeItemLotParams()
    {
        OrderedDictionary categoryDictEnemy = new();
        OrderedDictionary categoryDictMap = new();

        IEnumerable<Param.Row> itemLotParamMap = _itemLotParam_map.Rows.Where(id => !Unk.unkItemLotParamMapWeapons.Contains(id.ID));
        IEnumerable<Param.Row> itemLotParamEnemy = _itemLotParam_enemy.Rows.Where(id => !Unk.unkItemLotParamEnemyWeapons.Contains(id.ID));

        foreach (Param.Row row in itemLotParamEnemy.Concat(itemLotParamMap))
        {   // going through enemy weapon drops
            Param.Column[] itemIds = row.Cells.Take(Const.ItemLots).ToArray();
            Param.Column[] categories = row.Cells.Skip(Const.CategoriesStart).Take(Const.ItemLots).ToArray();
            Param.Column[] chances = row.Cells.Skip(Const.ChanceStart).Take(Const.ItemLots).ToArray();
            int totalWeight = chances.Sum(a => (ushort)a.GetValue(row));
            for (int i = 0; i < Const.ItemLots; i++)
            {
                int category = (int)categories[i].GetValue(row);
                if (category != Const.ItemLotWeaponCategory && category != Const.ItemLotCustomWeaponCategory)
                { continue; }

                int id = (int)itemIds[i].GetValue(row);
                //> Should work if it were item lot entries

                //^ TODO revisit for longterm
                int sanitizedId = washWeaponLevels(id);
                if (category == Const.ItemLotWeaponCategory)
                {
                    if (!_weaponDictionary.TryGetValue(sanitizedId, out EquipParamWeapon? wep)) { continue; }
                    if ((wep.wepType is Const.StaffType or Const.SealType)) { continue; }

                    if (id != sanitizedId)
                    {
                        _weaponNameDictionary[id] = $"{_weaponNameDictionary[sanitizedId]} + {id - sanitizedId}";
                    }
                    ushort chance = (ushort)chances[i].GetValue(row);
                    if (chance == totalWeight)
                    {
                        // these are all category 2         key(weapontype), new ItemLotEntry(id, 2);
                        addToOrderedDict(categoryDictMap, wep.wepType, new ItemLotEntry(id, category)); // (IOrderedDictionary orderedDict, object key, T type)
                        break; // Break here because the entire item lot param is just a single entry.  // could just add DLC Weapons to DictMap
                    }
                    addToOrderedDict(categoryDictEnemy, wep.wepType, new ItemLotEntry(id, category));
                }
                else
                { // category == Const.ItemLotCustomWeaponCategory
                    if (!_customWeaponDictionary.TryGetValue(id, out EquipParamWeapon? wep)) { continue; }
                    if (wep.wepType is Const.StaffType or Const.SealType) { continue; }

                    ushort chance = (ushort)chances[i].GetValue(row);

                    if (chance == totalWeight)
                    {
                        addToOrderedDict(categoryDictMap, wep.wepType, new ItemLotEntry(id, category));
                        break;
                    }
                    addToOrderedDict(categoryDictEnemy, wep.wepType, new ItemLotEntry(id, category));
                }
            }
        }

        // TODO potential to inject here
        addToOrderedDict(categoryDictMap, Const.AxeType, new ItemLotEntry(14510000, 2)); // Death Knight's Twin Axes
        addToOrderedDict(categoryDictMap, Const.AxeType, new ItemLotEntry(14540000, 2)); // Forked Tongue Hatchet
        addToOrderedDict(categoryDictMap, Const.FistType, new ItemLotEntry(21510000, 2)); // Pata
        addToOrderedDict(categoryDictMap, Const.FistType, new ItemLotEntry(21520000, 2)); // Poisoned Hand
        addToOrderedDict(categoryDictMap, Const.ClawType, new ItemLotEntry(22500000, 2)); // Claws of Night
        addToOrderedDict(categoryDictMap, Const.ColossalSwordType, new ItemLotEntry(4500000, 2)); // Ancient Meteoric Ore
        addToOrderedDict(categoryDictMap, Const.ColossalWeaponType, new ItemLotEntry(12530000, 2)); // Bloodfiend's Arm
        addToOrderedDict(categoryDictMap, Const.ColossalWeaponType, new ItemLotEntry(23500000, 2)); // Devonia's Hammer
        addToOrderedDict(categoryDictMap, Const.CurvedSwordType, new ItemLotEntry(7510000, 2)); // Falx
        addToOrderedDict(categoryDictMap, Const.KatanaType, new ItemLotEntry(9500000, 2)); // Sword of Night
        addToOrderedDict(categoryDictMap, Const.KatanaType, new ItemLotEntry(2520000, 2)); // Star Lined Sword
        addToOrderedDict(categoryDictMap, Const.GreatswordType, new ItemLotEntry(3520000, 2)); // Lizard Greatsword
        addToOrderedDict(categoryDictMap, Const.GreatswordType, new ItemLotEntry(3550000, 2)); // Greatsword of Solitude
        addToOrderedDict(categoryDictMap, Const.HalberdType, new ItemLotEntry(18500000, 2)); // Spirit Glaive
        addToOrderedDict(categoryDictEnemy, Const.HalberdType, new ItemLotEntry(18500000, 2)); // Spirit Glaive
        addToOrderedDict(categoryDictMap, Const.HammerType, new ItemLotEntry(11500000, 2)); // Flowerstone Gavel
        addToOrderedDict(categoryDictMap, Const.ReaperType, new ItemLotEntry(19500000, 2)); // Obsidian Lamina
        addToOrderedDict(categoryDictMap, Const.GreatHammerType, new ItemLotEntry(12520000, 2)); // Blacksteel Greathammer
        addToOrderedDict(categoryDictMap, Const.GreataxeType, new ItemLotEntry(15500000, 2)); // Death Knight's Longhaft Axe
        addToOrderedDict(categoryDictMap, Const.GreatSpearType, new ItemLotEntry(17520000, 2)); // Barbed Staff-Spear
        addToOrderedDict(categoryDictMap, Const.StraightSwordType, new ItemLotEntry(2550000, 2)); // Sword of Light
        addToOrderedDict(categoryDictMap, Const.StraightSwordType, new ItemLotEntry(2560000, 2)); // Sword of Darkness
        addToOrderedDict(categoryDictMap, Const.TwinbladeType, new ItemLotEntry(10510000, 2)); // Blacksteel Twinblade
        addToOrderedDict(categoryDictMap, Const.LightBowType, new ItemLotEntry(40500000, 2)); // Bone Bow
        addToOrderedDict(categoryDictMap, Const.BowType, new ItemLotEntry(41510000, 2)); // Ansbach's Bow
        addToOrderedDict(categoryDictMap, Const.BowType, new ItemLotEntry(43500000, 2)); // Repeating Crossbow
        addToOrderedDict(categoryDictMap, Const.BowType, new ItemLotEntry(43510000, 2)); // Spread Crossbow
        addToOrderedDict(categoryDictEnemy, Const.ColossalSwordType, new ItemLotEntry(4520000, 2)); // Fire Knight's Greatsword
        addToOrderedDict(categoryDictEnemy, Const.ColossalSwordType, new ItemLotEntry(4500000, 2)); // Ancient Meteoric Ore
        addToOrderedDict(categoryDictMap, Const.HeavyThrustingType, new ItemLotEntry(6500000, 2));  // Queelign's

        addToOrderedDict(categoryDictMap, Const.HeavyThrustingType, new ItemLotEntry(62510000, 2)); // Carian Thrusting Shield
        addToOrderedDict(categoryDictMap, Const.GreatswordType, new ItemLotEntry(67500000, 2)); // Milady
        addToOrderedDict(categoryDictMap, Const.GreatswordType, new ItemLotEntry(67510000, 2)); // Leda's Sword
        addToOrderedDict(categoryDictMap, Const.CurvedGreatswordType, new ItemLotEntry(64520000, 2)); // Curseblade's Cirque
        addToOrderedDict(categoryDictMap, Const.CurvedGreatswordType, new ItemLotEntry(64500000, 2)); // Backhand Blade

        addToOrderedDict(categoryDictEnemy, Const.CurvedGreatswordType, new ItemLotEntry(64520000, 2)); // Curseblade's Cirque
        addToOrderedDict(categoryDictEnemy, Const.CurvedGreatswordType, new ItemLotEntry(64500000, 2)); // Backhand Blade
        addToOrderedDict(categoryDictEnemy, Const.KatanaType, new ItemLotEntry(66510000, 2)); // Dragon-Hunters
        addToOrderedDict(categoryDictEnemy, Const.KatanaType, new ItemLotEntry(66500000, 2)); // Great Katana
        addToOrderedDict(categoryDictEnemy, Const.KatanaType, new ItemLotEntry(66520000, 2)); // Rakshasa's

        dedupeAndRandomizeVectors(categoryDictMap);
        dedupeAndRandomizeVectors(categoryDictEnemy);

        Dictionary<int, ItemLotEntry> guaranteedDropReplace = getReplacementHashmap(categoryDictMap);
        Dictionary<int, ItemLotEntry> chanceDropReplace = getReplacementHashmap(categoryDictEnemy);

        // Application now has weapons set to randomize
        // logItem(">> Item Replacements - all instances of item on left will be replaced with item on right");
        // logItem("## Guaranteed Weapons");
        // logReplacementDictionary(guaranteedDropReplace);
        // logItem("\n## Chance Weapons");
        // logReplacementDictionary(chanceDropReplace);
        logItem("");

        foreach (Param.Row row in _itemLotParam_enemy.Rows.Concat(_itemLotParam_map.Rows))
        {
            Param.Column[] itemIds = row.Cells.Take(Const.ItemLots).ToArray();
            Param.Column[] categories = row.Cells.Skip(Const.CategoriesStart).Take(Const.ItemLots).ToArray();
            for (int i = 0; i < Const.ItemLots; i++)
            {
                int category = (int)categories[i].GetValue(row);
                if (category != Const.ItemLotWeaponCategory && category != Const.ItemLotCustomWeaponCategory)
                { continue; }

                int id = (int)itemIds[i].GetValue(row);
                if (category == Const.ItemLotWeaponCategory)
                {
                    // if (!_weaponDictionary.TryGetValue(washWeaponLevels(id), out _))
                    // { continue; }

                    if (guaranteedDropReplace.TryGetValue(id, out ItemLotEntry entry))
                    {
                        itemIds[i].SetValue(row, entry.Id);
                        categories[i].SetValue(row, entry.Category);
                        break;
                    }
                    if (!chanceDropReplace.TryGetValue(id, out entry))
                    { continue; }

                    itemIds[i].SetValue(row, entry.Id);
                    categories[i].SetValue(row, entry.Category);
                }
                else
                { // category == Const.ItemLotCustomWeaponCategory
                    if (!_customWeaponDictionary.TryGetValue(id, out _))
                    { continue; }

                    if (guaranteedDropReplace.TryGetValue(id, out ItemLotEntry entry))
                    {
                        itemIds[i].SetValue(row, entry.Id);
                        categories[i].SetValue(row, entry.Category);
                    }
                    if (!chanceDropReplace.TryGetValue(id, out entry))
                    { continue; }

                    itemIds[i].SetValue(row, entry.Id);
                    categories[i].SetValue(row, entry.Category);
                }
            }
        }
    }
    private void randomizeShopLineupParam() //TODO add armor randomization, TODO randomize away from Carian Regal Scepter
    {
        string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // TODO remove

        List<ShopLineupParam> shopLineupParamRemembranceList = new();
        foreach (Param.Row row in _shopLineupParam.Rows)
        {
            /*  
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "WriteLines.txt"), true))
                {
                    foreach (Param.Column col in row.Cells)
                    {
                        if ((byte)row["equipType"]!.Value.Value == Const.ShopLineupArmorCategory)
                        {

                            outputFile.WriteLine($" id {row.ID}, value: {col.GetValue(row)} <>");
                        }
                    }
                }
            */

            if ((byte)row["equipType"]!.Value.Value != Const.ShopLineupWeaponCategory || (row.ID < 101900 || row.ID > 101980))
            { continue; } // assures only weapons are randomized TODO update for armor

            ShopLineupParam lot = new(new Param.Row(row));
            int sanitizedId = washWeaponLevels(lot.equipId);

            if (!_weaponDictionary.TryGetValue(sanitizedId, out _))
            { continue; }

            if (lot.equipId != sanitizedId)
            {
                _weaponNameDictionary[lot.equipId] = $"{_weaponNameDictionary[sanitizedId]} +{lot.equipId - sanitizedId}";
            }
            shopLineupParamRemembranceList.Add(lot);
        }

        // logItem("<> Shop Replacements - Random item selected from pool of all weapons (not including infused weapons). Remembrances are randomized amongst each-other.");

        List<int> RemembranceWeaponIDs = new List<int>()
        {
            3100000, 3140000, 4020000, 4050000, 6040000, 8100000, 9020000, 11150000,
            13030000, 15040000, 15110000, 17010000,  20060000, 21060000, 23050000,
            42000000,
            // 4530000, 4550000,  // Radahn's DLC swords
            // DLC
            3500000, 3510000, 8500000, 17500000, 18510000, 23510000, 23520000, 67520000,
        };

        List<List<int>> WeaponShopLists = new List<List<int>>() {
            Equipment.LightBowAndBowIDs, Equipment.SmallShieldIDs, Equipment.MediumShieldIDs, Equipment.TorchIDs,
            Equipment.ColossalWeaponIDs, Equipment.CurvedGreatSwordIDs, Equipment.HammerIDs, Equipment.StraightSwordIDs,
            Equipment.CurvedSwordIDs, Equipment.ReaperIDs, Equipment.TwinbladeIDs, Equipment.DaggerIDs,
            Equipment.FistIDs, Equipment.GreatswordIDs, Equipment.DlcGreatswordIDs, Equipment.DlcSideIDs,
            Equipment.DlcHeavyShopIDs, Equipment.DlcLightShopIDs, Equipment.DlcSmithingIDs, Equipment.DlcRangedIds,
        };

        foreach (Param.Row row in _shopLineupParam.Rows)
        {
            if ((byte)row["equipType"]!.Value.Value != Const.ShopLineupWeaponCategory
                || row.ID > 101980
            ) // TODO find out what this row.ID is removes 20 lots
            { continue; }

            ShopLineupParam lot = new(row);

            if (!_weaponDictionary.TryGetValue(washWeaponLevels(lot.equipId), out EquipParamWeapon? wep))
            { continue; }

            if (lot.equipId == Const.CarianRegalScepter || !(wep.wepType is Const.StaffType or Const.SealType))
            {   // about 60 item shop allocations
                int index = _random.Next(WeaponShopLists.Count);
                List<int> weaponIndex = WeaponShopLists[index];

                replaceShopLineupParam(lot, weaponIndex, RemembranceWeaponIDs);
            }
        }
        List<int> baseHeadProtectors = new List<int>()
        {
            40000, 160000, 210000, 280000, 620000, 630000, 660000, 670000, 730000, 870000,
            880000, 890000, 1401000, 1500000, 1100000,
        };
        List<int> baseArmProtectors = new List<int>() // arm protectors found in shops
        {
            40200, 210200, 280200, 630200, 660200, 670200, 730200, 870200, 880200, 930200, 1500200,
        };
        List<int> baseBodyProtectors = new List<int>()
        {
            40100, 210100, 280100, 622100, 630100, 660100, 670100, 730100, 870100, 880100, 890100, 931100,
            962100, 1500100, 1102100, 1100100,
        };
        List<int> baseLegProtectors = new List<int>()
        {
            40300, 210300, 280300, 620300, 630300, 660300, 670300, 730300, 870300, 880300, 890300, 930300,
            960300, 1500300,
        };

        foreach (Param.Row row in _shopLineupParam.Rows)
        {
            if ((byte)row["equipType"]!.Value.Value == Const.ShopLineupArmorCategory)
            {
                if (row.ID > 101980) { continue; }

                ShopLineupParam lot = new(row);

                if (baseHeadProtectors.Contains(lot.equipId)) // chain coif (helmet)
                {
                    int index = _random.Next(Equipment.HeadArmorIDs.Count);
                    lot.equipId = Equipment.HeadArmorIDs[index];
                }
                if (baseBodyProtectors.Contains(lot.equipId)) // Chain armor
                {
                    int index = _random.Next(Equipment.BodyArmorIDs.Count);
                    lot.equipId = Equipment.BodyArmorIDs[index];
                }
                if (baseArmProtectors.Contains(lot.equipId))
                {
                    int index = _random.Next(Equipment.ArmsArmorIDs.Count);
                    lot.equipId = Equipment.ArmsArmorIDs[index];
                }
                if (lot.equipId == 1100300) // Chain Leggings
                {
                    int index = _random.Next(Equipment.LegsArmorIDs.Count);
                    lot.equipId = Equipment.LegsArmorIDs[index];
                }

            }
        }
    }
    private void randomizeShopLineupParamMagic()
    {
        OrderedDictionary magicCategoryDictMap = new();
        List<ShopLineupParam> shopLineupParamRemembranceList = new();
        List<ShopLineupParam> shopLineupParamDragonList = new();
        foreach (Param.Row row in _shopLineupParam.Rows)
        {
            if ((byte)row["equipType"]!.Value.Value != Const.ShopLineupGoodsCategory || row.ID > 101980)
            { continue; }

            ShopLineupParam lot = new(new Param.Row(row));
            if (!_magicDictionary.TryGetValue(lot.equipId, out Magic? magic)) { continue; }

            if (row.ID < 101950)
            {
                if (lot.mtrlId == -1)
                {
                    addToOrderedDict(magicCategoryDictMap, magic.ezStateBehaviorType, lot.equipId);
                    continue;
                }
                shopLineupParamRemembranceList.Add(lot);
            }
            else
            { // Dragon Communion Shop 101950 - 101980 
                shopLineupParamDragonList.Add(lot);
            }
        }

        foreach (Param.Row row in _itemLotParam_enemy.Rows.Concat(_itemLotParam_map.Rows))
        {
            Param.Column[] itemIds = row.Cells.Take(Const.ItemLots).ToArray();
            Param.Column[] categories = row.Cells.Skip(Const.CategoriesStart).Take(Const.ItemLots).ToArray();
            Param.Column[] chances = row.Cells.Skip(Const.ChanceStart).Take(Const.ItemLots).ToArray();
            int totalWeight = chances.Sum(a => (ushort)a.GetValue(row));
            for (int i = 0; i < Const.ItemLots; i++)
            {
                int category = (int)categories[i].GetValue(row);
                if (category != Const.ItemLotGoodsCategory) { continue; }

                int id = (int)itemIds[i].GetValue(row);
                if (!_magicDictionary.TryGetValue(id, out Magic? magic)) { continue; }

                ushort chance = (ushort)chances[i].GetValue(row);
                if (chance == totalWeight)
                {
                    addToOrderedDict(magicCategoryDictMap, magic.ezStateBehaviorType, id);
                    break;
                }
                addToOrderedDict(magicCategoryDictMap, magic.ezStateBehaviorType, id);
            }
        }

        dedupeAndRandomizeShopVectors(magicCategoryDictMap);

        Dictionary<int, int> magicShopReplacement = getShopReplacementHashmap(magicCategoryDictMap);
        shopLineupParamRemembranceList.Shuffle(_random); // TODO investigate if thise matters
        shopLineupParamDragonList.Shuffle(_random); // TODO investigate if thise matters
                                                    // logItem("\n## All Magic Replacement.");
                                                    // logReplacementDictionaryMagic(magicShopReplacement);

        // logItem("\n~* Shop Magic Replacement.");
        foreach (Param.Row row in _shopLineupParam.Rows)
        {
            // logShopIdMagic(row.ID);
            if ((byte)row["equipType"]!.Value.Value != Const.ShopLineupGoodsCategory || row.ID > 101980)
            { continue; }

            ShopLineupParam lot = new(row);
            if (!_magicDictionary.TryGetValue(lot.equipId, out _)) { continue; }

            if (row.ID < 101950)
            { replaceShopLineupParamMagic(lot, magicShopReplacement, shopLineupParamRemembranceList); }

            else
            {
                ShopLineupParam newDragonIncant = getNewId(lot.equipId, shopLineupParamDragonList);
                // logItem($"{_goodsFmg[lot.equipId]} -> {_goodsFmg[newDragonIncant.equipId]}");
                copyShopLineupParam(lot, newDragonIncant);
            }
        }

        foreach (Param.Row row in _itemLotParam_enemy.Rows.Concat(_itemLotParam_map.Rows))
        {
            Param.Column[] itemIds = row.Cells.Take(Const.ItemLots).ToArray();
            Param.Column[] categories = row.Cells.Skip(Const.CategoriesStart).Take(Const.ItemLots).ToArray();
            for (int i = 0; i < Const.ItemLots; i++)
            {
                int category = (int)categories[i].GetValue(row);
                if (category != Const.ItemLotGoodsCategory) { continue; }

                int id = (int)itemIds[i].GetValue(row);
                if (!_magicDictionary.TryGetValue(id, out Magic _)) { continue; }

                if (!magicShopReplacement.TryGetValue(id, out int entry)) { continue; }

                itemIds[i].SetValue(row, entry);
            }
        }
    }
    private void patchAtkParam()
    {
        Param.Row swarmOfFlies1 = _atkParam_Pc[72100] ?? throw new InvalidOperationException("Entry 72100 not found in AtkParam_Pc");
        Param.Row swarmOfFlies2 = _atkParam_Pc[72101] ?? throw new InvalidOperationException("Entry 72101 not found in AtkParam_Pc");

        AtkParam swarmAtkParam1 = new(swarmOfFlies1);
        AtkParam swarmAtkParam2 = new(swarmOfFlies2);
        patchSpEffectAtkPowerCorrectRate(swarmAtkParam1);
        patchSpEffectAtkPowerCorrectRate(swarmAtkParam2);
    }

    private void patchSmithingStones()
    {
        foreach (Param.Row row in _equipMtrlSetParam.Rows)
        {
            int id = (int)row["materialId01"]!.Value.Value;
            int category = (byte)row["materialCate01"]!.Value.Value;
            int numberRequired = (sbyte)row["itemNum01"]!.Value.Value;

            if (category == 4 && numberRequired > 1 && id >= 10100 && id < 10110)
            {
                row["itemNum01"].Value.SetValue(Const.SmithingCost);
                // if (numberRequired == 4) { } // if (numberRequired == 6) { } //  EquipMtrlSetParam param = new(row); //  param.itemNum01 = (sbyte)3;
            }
        }
    }
}