using ERBingoRandomizer.FileHandler;
using ERBingoRandomizer.Params;
using ERBingoRandomizer.Randomizer.Strategies;
using ERBingoRandomizer.Randomizer.Strategies.CharaInitParam;
using ERBingoRandomizer.Utility;
using FSParam;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static ERBingoRandomizer.Params.EquipParamWeapon;


namespace ERBingoRandomizer.Randomizer;

public partial class BingoRandomizer {
    private RandoResource _resources;
    // Strategies
    private readonly IBingoClassStrategy _classRandomizer;

    // TODO: Switch this out based on season, or use granular feature object for individual features.
    private static bool Season3 = true;

    //static async method that behaves like a constructor    
    public static async Task<BingoRandomizer> BuildRandomizerAsync(string path, string seed,
        CancellationToken cancellationToken) {
        BingoRandomizer rando = new(path, seed, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        await Task.Run(() => rando.init());
        return rando;
    }
    // Cancellation Token
    private readonly CancellationToken _cancellationToken;
    private BingoRandomizer(string path, string seed, CancellationToken cancellationToken) {
        _resources = new RandoResource(path, seed, cancellationToken);
        if (Season3)
        {
            _classRandomizer = new Season3ClassRandomizer(new Season2LevelRandomizer(_resources.Random), _resources);
        }
        else
        {
            _classRandomizer = new Season2ClassRandomizer(new Season2LevelRandomizer(_resources.Random), _resources);
        }
        _cancellationToken = cancellationToken;
    }

    private Task init() {
        return _resources.Init();
    }

    public Task RandomizeRegulation() {
        //calculateLevels();
        _classRandomizer.RandomizeCharaInitParam();
        _cancellationToken.ThrowIfCancellationRequested();
        randomizeItemLotParams();
        _cancellationToken.ThrowIfCancellationRequested();
        randomizeShopLineupParam();
        _cancellationToken.ThrowIfCancellationRequested();
        randomizeShopLineupParamMagic();
        _cancellationToken.ThrowIfCancellationRequested();
        patchAtkParam();
        _cancellationToken.ThrowIfCancellationRequested();
        changeUpgradeMaterialType();
        _cancellationToken.ThrowIfCancellationRequested();
        if (Season3) {
            unlockSeason3Graces();
        }
        _cancellationToken.ThrowIfCancellationRequested();
        writeFiles();
        Logger.WriteLog(_resources.Seed);
        return Task.CompletedTask;
    }

    private void changeUpgradeMaterialType() {
        foreach (Param.Row row in _resources.EquipMtrlSetParam.Rows) {
            EquipMtrlSetParam mtrl = new EquipMtrlSetParam(row);
            
            int id = mtrl.materialId01;
            int cat = mtrl.materialCate01;
            int num = mtrl.itemNum01;
            if (cat == 4 && id >= 10100 && id < 10110 && num > 1) {
                mtrl.itemNum01 = 1;
            }
        }
    }

    public SeedInfo GetSeedInfo() {
        return new SeedInfo(_resources.Seed, Util.GetShaRegulation256Hash());
    }
    private void randomizeItemLotParams() {
        OrderedDictionary categoryDictEnemy = new();
        OrderedDictionary categoryDictMap = new();

        IEnumerable<Param.Row> itemLotParamMap =
            _resources.ItemLotParamMap.Rows.Where(id => !Unk.unkItemLotParamMapWeapons.Contains(id.ID));
        IEnumerable<Param.Row> itemLotParamEnemy =
            _resources.ItemLotParamEnemy.Rows.Where(id => !Unk.unkItemLotParamEnemyWeapons.Contains(id.ID));

        foreach (Param.Row row in itemLotParamEnemy.Concat(itemLotParamMap)) {
            Param.Column[] itemIds = row.Cells.Take(Const.ItemLots).ToArray();
            Param.Column[] categories = row.Cells.Skip(Const.CategoriesStart).Take(Const.ItemLots).ToArray();
            Param.Column[] chances = row.Cells.Skip(Const.ChanceStart).Take(Const.ItemLots).ToArray();
            int totalWeight = chances.Sum(a => (ushort)a.GetValue(row));
            for (int i = 0; i < Const.ItemLots; i++) {
                int category = (int)categories[i].GetValue(row);
                if (category != Const.ItemLotWeaponCategory && category != Const.ItemLotCustomWeaponCategory) {
                    continue;
                }

                int id = (int)itemIds[i].GetValue(row);
                int sanitizedId = RemoveWeaponLevels(id);
                if (category == Const.ItemLotWeaponCategory) {
                    if (!_resources.WeaponDictionary.TryGetValue(sanitizedId, out EquipParamWeapon? wep)) {
                        continue;
                    }

                    if (wep.wepType is WeaponType.GlintstoneStaff or WeaponType.FingerSeal) {
                        continue;
                    }

                    if (id != sanitizedId) {
                        int difference = id - sanitizedId;
                        string differenceString = difference != 0 ? $" +{difference}" : string.Empty;
                        _resources.WeaponNameDictionary[id] =
                            $"{_resources.WeaponNameDictionary[sanitizedId]}{differenceString}";
                    }

                    ushort chance = (ushort)chances[i].GetValue(row);
                    if (chance == totalWeight) {
                        addToOrderedDict(categoryDictMap, wep.wepType, new ItemLotEntry(id, category));
                        break; // Break here because the entire item lot param is just a single entry.
                    }

                    addToOrderedDict(categoryDictEnemy, wep.wepType, new ItemLotEntry(id, category));
                }
                else {
                    // category == Const.ItemLotCustomWeaponCategory
                    if (!_resources.CustomWeaponDictionary.TryGetValue(id, out EquipParamWeapon? wep)) {
                        continue;
                    }

                    if (wep.wepType is WeaponType.GlintstoneStaff or WeaponType.FingerSeal) {
                        continue;
                    }

                    Param.Row paramRow = _resources.EquipParamCustomWeapon[id]!;
                    EquipParamCustomWeapon customWeapon = new(paramRow);
                    if (!_resources.WeaponNameDictionary.ContainsKey(customWeapon.baseWepId)) {
                        int baseWeaponId = customWeapon.baseWepId;
                        int customSanitizedId = RemoveWeaponLevels(baseWeaponId);
                        int difference = baseWeaponId - customSanitizedId;
                        string differenceString = difference != 0 ? $" +{difference}" : string.Empty;
                        _resources.WeaponNameDictionary[id] =
                            $"{_resources.WeaponNameDictionary[baseWeaponId]}{differenceString}";
                    }

                    ushort chance = (ushort)chances[i].GetValue(row);
                    if (chance == totalWeight) {
                        addToOrderedDict(categoryDictMap, wep.wepType, new ItemLotEntry(id, category));
                        break;
                    }

                    addToOrderedDict(categoryDictEnemy, wep.wepType, new ItemLotEntry(id, category));
                }
            }
        }

        foreach (Param.Row row in _resources.ShopLineupParam.Rows) {
            if ((byte)row["equipType"]!.Value.Value != Const.ShopLineupWeaponCategory || row.ID >= 101900) {
                continue;
            }

            ShopLineupParam lot = new(new Param.Row(row));
            int sanitizedId = RemoveWeaponLevels(lot.equipId);
            if (!_resources.WeaponDictionary.TryGetValue(sanitizedId, out EquipParamWeapon? wep)) {
                continue;
            }

            if (wep.wepType is WeaponType.GlintstoneStaff or WeaponType.FingerSeal) {
                continue;
            }

            // if (lot.equipId != sanitizedId) {
            //     _resources.WeaponNameDictionary[lot.equipId] = $"{_resources.WeaponNameDictionary[sanitizedId]} +{lot.equipId - sanitizedId}";
            // }
            addToOrderedDict(categoryDictMap, wep.wepType, new ItemLotEntry(lot.equipId, 2));
        }

        dedupeAndRandomizeVectors(categoryDictMap);
        dedupeAndRandomizeVectors(categoryDictEnemy);

        //Console.WriteLine("categoryDictMap");
        //dumpCategoriesAndCounts(categoryDictMap);
        //dumpCategoriesAndNames(categoryDictMap);
        Console.WriteLine("categoryDictEnemy");
        dumpCategoriesAndNames(categoryDictMap);

        Dictionary<int, ItemLotEntry> guaranteedDropReplace = getReplacementHashmap(categoryDictMap);
        Dictionary<int, ItemLotEntry> chanceDropReplace = getReplacementHashmap(categoryDictEnemy);
        Logger.LogItem(">> Item Replacements - all instances of item on left will be replaced with item on right");
        Logger.LogItem("> Guaranteed Weapons");
        logReplacementDictionary(guaranteedDropReplace);
        Logger.LogItem("> Chance Weapons");
        logReplacementDictionary(chanceDropReplace);
        Logger.LogItem("");

        foreach (Param.Row row in _resources.ItemLotParamEnemy.Rows.Concat(_resources.ItemLotParamMap.Rows)) {
            Param.Column[] itemIds = row.Cells.Take(Const.ItemLots).ToArray();
            Param.Column[] categories = row.Cells.Skip(Const.CategoriesStart).Take(Const.ItemLots).ToArray();
            for (int i = 0; i < Const.ItemLots; i++) {
                int category = (int)categories[i].GetValue(row);
                if (category != Const.ItemLotWeaponCategory && category != Const.ItemLotCustomWeaponCategory) {
                    continue;
                }

                int id = (int)itemIds[i].GetValue(row);
                if (category == Const.ItemLotWeaponCategory) {
                    if (!_resources.WeaponDictionary.TryGetValue(RemoveWeaponLevels(id), out _)) {
                        continue;
                    }

                    if (guaranteedDropReplace.TryGetValue(id, out ItemLotEntry entry)) {
                        itemIds[i].SetValue(row, entry.Id);
                        categories[i].SetValue(row, entry.Category);
                        break;
                    }
                    if (!chanceDropReplace.TryGetValue(id, out entry)) {
                        continue;
                    }
                    itemIds[i].SetValue(row, entry.Id);
                    categories[i].SetValue(row, entry.Category);
                }
                else {
                    // category == Const.ItemLotCustomWeaponCategory
                    if (!_resources.CustomWeaponDictionary.TryGetValue(id, out _)) {
                        continue;
                    }

                    if (guaranteedDropReplace.TryGetValue(id, out ItemLotEntry entry)) {
                        itemIds[i].SetValue(row, entry.Id);
                        categories[i].SetValue(row, entry.Category);
                    }
                    if (!chanceDropReplace.TryGetValue(id, out entry)) {
                        continue;
                    }
                    itemIds[i].SetValue(row, entry.Id);
                    categories[i].SetValue(row, entry.Category);
                }
            }
        }
    }
    private void dumpCategoriesAndNames(OrderedDictionary dictionary) {
        foreach (object? key in dictionary.Keys) {
            List<ItemLotEntry> list = (List<ItemLotEntry>)dictionary[key]!;
            EquipParamWeapon.WeaponType type = (EquipParamWeapon.WeaponType)key;
            Console.WriteLine($"{type}");
            foreach (ItemLotEntry itemLotEntry in list) {
                int id = RemoveWeaponLevels(itemLotEntry.Id);
                string name = _resources.WeaponFmg[id];
                if (string.IsNullOrWhiteSpace(name)) {
                    name = $"{_resources.WeaponNameDictionary[itemLotEntry.Id]}";
                }
                Console.WriteLine($"\t{name}");
            }
        }
    }
    private void dumpCategoriesAndCounts(OrderedDictionary dictionary) {
        foreach (object? key in dictionary.Keys) {
            List<ItemLotEntry> list = (List<ItemLotEntry>)dictionary[key]!;
            EquipParamWeapon.WeaponType type = (EquipParamWeapon.WeaponType)key;
            Console.WriteLine($"{type} = {list.Count}");
        }
    }
    private void randomizeShopLineupParam() {
        List<ShopLineupParam> shopLineupParamRemembranceList = new();
        foreach (Param.Row row in _resources.ShopLineupParam.Rows) {
            if ((byte)row["equipType"]!.Value.Value != Const.ShopLineupWeaponCategory ||
                (row.ID < 101900 || row.ID > 101980)) {
                continue;
            }

            ShopLineupParam lot = new(new Param.Row(row));
            int sanitizedId = RemoveWeaponLevels(lot.equipId);
            if (!_resources.WeaponDictionary.TryGetValue(sanitizedId, out _)) {
                continue;
            }

            // if (lot.equipId != sanitizedId) {
            //     _resources.WeaponNameDictionary[lot.equipId] = $"{_resources.WeaponNameDictionary[sanitizedId]} +{lot.equipId - sanitizedId}";
            // }
            shopLineupParamRemembranceList.Add(lot);
        }

        List<Param.Row> staves = _resources.WeaponTypeDictionary[WeaponType.GlintstoneStaff];
        List<Param.Row> seals = _resources.WeaponTypeDictionary[WeaponType.FingerSeal];
        List<int> shopLineupParamList = _resources.WeaponDictionary.Keys.Select(RemoveWeaponMetadata).Distinct()
            .Where(i => shopLineupParamRemembranceList.All(s => s.equipId != i))
            .Where(id => staves.All(s => s.ID != id) && seals.All(s => s.ID != id))
            .ToList();
        shopLineupParamList.Shuffle(_resources.Random);
        shopLineupParamRemembranceList.Shuffle(_resources.Random);

        Logger.LogItem(
            ">> Shop Replacements - Random item selected from pool of all weapons (not including infused weapons). Remembrances are randomized amongst each-other.");

        foreach (Param.Row row in _resources.ShopLineupParam.Rows) {
            logShopId(row.ID);
            if ((byte)row["equipType"]!.Value.Value != Const.ShopLineupWeaponCategory || row.ID > 101980) {
                continue;
            }

            ShopLineupParam lot = new(row);
            if (!_resources.WeaponDictionary.TryGetValue(RemoveWeaponLevels(lot.equipId), out EquipParamWeapon? wep)) {
                continue;
            }

            if (wep.wepType is WeaponType.GlintstoneStaff or WeaponType.FingerSeal) {
                continue;
            }

            replaceShopLineupParam(lot, shopLineupParamList, shopLineupParamRemembranceList);
        }
    }
    private void randomizeShopLineupParamMagic() {
        OrderedDictionary magicCategoryDictMap = new();
        List<ShopLineupParam> shopLineupParamRemembranceList = new();
        List<ShopLineupParam> shopLineupParamDragonList = new();
        foreach (Param.Row row in _resources.ShopLineupParam.Rows) {
            if ((byte)row["equipType"]!.Value.Value != Const.ShopLineupGoodsCategory || row.ID > 101980) {
                continue;
            }

            ShopLineupParam lot = new(new Param.Row(row));
            if (!_resources.MagicDictionary.TryGetValue(lot.equipId, out Magic? magic)) {
                continue;
            }
            if (row.ID < 101950) {
                if (lot.mtrlId == -1) {
                    addToOrderedDict(magicCategoryDictMap, magic.ezStateBehaviorType, lot.equipId);
                    continue;
                }
                shopLineupParamRemembranceList.Add(lot);
            }
            else {
                // Dragon Communion Shop 101950 - 101980 
                shopLineupParamDragonList.Add(lot);
            }
        }

        foreach (Param.Row row in _resources.ItemLotParamEnemy.Rows.Concat(_resources.ItemLotParamMap.Rows)) {
            Param.Column[] itemIds = row.Cells.Take(Const.ItemLots).ToArray();
            Param.Column[] categories = row.Cells.Skip(Const.CategoriesStart).Take(Const.ItemLots).ToArray();
            Param.Column[] chances = row.Cells.Skip(Const.ChanceStart).Take(Const.ItemLots).ToArray();
            int totalWeight = chances.Sum(a => (ushort)a.GetValue(row));
            for (int i = 0; i < Const.ItemLots; i++) {
                int category = (int)categories[i].GetValue(row);
                if (category != Const.ItemLotGoodsCategory) {
                    continue;
                }

                int id = (int)itemIds[i].GetValue(row);
                if (!_resources.MagicDictionary.TryGetValue(id, out Magic? magic)) {
                    continue;
                }
                ushort chance = (ushort)chances[i].GetValue(row);
                if (chance == totalWeight) {
                    addToOrderedDict(magicCategoryDictMap, magic.ezStateBehaviorType, id);
                    break;
                }
                addToOrderedDict(magicCategoryDictMap, magic.ezStateBehaviorType, id);
            }
        }

        dedupeAndRandomizeShopVectors(magicCategoryDictMap);

        Dictionary<int, int> magicShopReplacement = getShopReplacementHashmap(magicCategoryDictMap);
        shopLineupParamRemembranceList.Shuffle(_resources.Random);
        shopLineupParamDragonList.Shuffle(_resources.Random);
        Logger.LogItem("\n>> All Magic Replacement.");
        logReplacementDictionaryMagic(magicShopReplacement);

        Logger.LogItem("\n>> Shop Magic Replacement.");
        foreach (Param.Row row in _resources.ShopLineupParam.Rows) {
            logShopIdMagic(row.ID);
            if ((byte)row["equipType"]!.Value.Value != Const.ShopLineupGoodsCategory || row.ID > 101980) {
                continue;
            }

            ShopLineupParam lot = new(row);
            if (!_resources.MagicDictionary.TryGetValue(lot.equipId, out _)) {
                continue;
            }
            if (row.ID < 101950) {
                replaceShopLineupParamMagic(lot, magicShopReplacement, shopLineupParamRemembranceList);
            }
            else {
                ShopLineupParam newDragonIncant = getNewId(lot.equipId, shopLineupParamDragonList);
                Logger.LogItem($"{_resources.GoodsFmg[lot.equipId]} -> {_resources.GoodsFmg[newDragonIncant.equipId]}");
                copyShopLineupParam(lot, newDragonIncant);
            }
        }

        foreach (Param.Row row in _resources.ItemLotParamEnemy.Rows.Concat(_resources.ItemLotParamMap.Rows)) {
            Param.Column[] itemIds = row.Cells.Take(Const.ItemLots).ToArray();
            Param.Column[] categories = row.Cells.Skip(Const.CategoriesStart).Take(Const.ItemLots).ToArray();
            for (int i = 0; i < Const.ItemLots; i++) {
                int category = (int)categories[i].GetValue(row);
                if (category != Const.ItemLotGoodsCategory) {
                    continue;
                }

                int id = (int)itemIds[i].GetValue(row);
                if (!_resources.MagicDictionary.TryGetValue(id, out Magic _)) {
                    continue;
                }

                if (!magicShopReplacement.TryGetValue(id, out int entry)) {
                    continue;
                }
                itemIds[i].SetValue(row, entry);
            }
        }
    }
    private void unlockSeason3Graces() {
        // There are a few parts to this:
        // - Set many flags when Torrent is unlocked. Do this every time we load in, especially if the mod gets updated or wasn't installed right.
        // - To avoid map notification spam, don't set the event flag which notifies the in-game map to do the notification.
        // - Adjust logic for Altus detection event so the custom bonfires don't count for it.

        int altusId = 76303; // Altus Highway Junction
        int gelmirId = 76353; // Road of Iniquity
        List<int> bonfireFlags = new() {
            71190, // Roundtable
            // -- v1
            76154, // Ailing Village Outskirts
            76413, // Inner Aeonia
            altusId, // Altus Highway Junction
            gelmirId, // Road of Iniquity
            71222, // Siofra River Bank
            // -- v2
            76521, // Snow Valley Ruins Overlook
            76551, // Inner Consecrated Snowfield
            71504, // Haligtree Roots
            76203, // Scenic Isle
            76225, // Ruined Labyrinth
            71216, // Lake of Rot Shoreside
        };
        List<int> mapFlags = new() {
            62010,  // Map: Limgrave, West
            62011,  // Map: Weeping Peninsula
            62012,  // Map: Limgrave, East
            62020,  // Map: Liurnia, East
            62021,  // Map: Liurnia, North
            62022,  // Map: Liurnia, West
            62030,  // Map: Altus Plateau
            62031,  // Map: Leyndell, Royal Capital
            62032,  // Map: Mt. Gelmir
            62040,  // Map: Caelid
            62041,  // Map: Dragonbarrow
            62050,  // Map: Mountaintops of the Giants, West
            62051,  // Map: Mountaintops of the Giants, East
            62060,  // Map: Ainsel River
            62061,  // Map: Lake of Rot
            62063,  // Map: Siofra River
            62062,  // Map: Mohgwyn Palace
            62064,  // Map: Deeproot Depths
            62052,  // Map: Consecrated Snowfield
        };
        List<int> otherFlags = new() {
            82001, // Underground map layer enabled
            10009655, // First approached about Roundtable
            105, // Visited Roundtable
        };
        EMEVD common = _resources.CommonEmevd;
        if (common == null) {
            throw new InvalidOperationException($"Missing emevd {Const.CommonEventPath}");
        }
        List<EMEVD.Instruction> newInstrs = new()
        {
            // IfEventFlag(MAIN, ON, TargetEventFlagType.EventFlag, 60100)
            new EMEVD.Instruction(3, 0, new List<object> { (sbyte)0, (byte)1, (byte)0, 60100 }),
            // OpenWorldMapPoint(111000, 100)
            // (May be redundant to unlocking all maps)
            new EMEVD.Instruction(2003, 78, new List<object> { 111000, 100f }),
        };
        foreach (int flag in bonfireFlags.Concat(mapFlags).Concat(otherFlags))
        {
            // SetEventFlag(TargetEventFlagType.EventFlag, flag, ON)
            newInstrs.Add(new EMEVD.Instruction(2003, 66, new List<object> { (byte)0, flag, (byte)1 }));
        }
        int newEventId = 279551111;  // Arbitrary number
        EMEVD.Event newEvent = new EMEVD.Event(newEventId, EMEVD.Event.RestBehaviorType.Default);
        newEvent.Instructions = newInstrs;
        common.Events.Add(newEvent);

        // Process to edit other events
        EMEVD.Event? constrEvent = common.Events.Find(e => e.ID == 0);
        EMEVD.Event? grantMapEvent = common.Events.Find(e => e.ID == 1600);
        EMEVD.Event? reachedAltusEvent = common.Events.Find(e => e.ID == 3044);
        if (constrEvent == null || grantMapEvent == null || reachedAltusEvent == null) {
            throw new InvalidOperationException($"{Const.CommonEventPath} missing one of required events [0, 1600, 3044]");
        }
        // Initialize new event
        constrEvent.Instructions.Add(new EMEVD.Instruction(2000, 0, new List<object> { 0, newEventId, 0 }));
        // Map unlock spam fix
        for (int i = 0; i < grantMapEvent.Instructions.Count; i++)
        {
            EMEVD.Instruction ins = grantMapEvent.Instructions[i];
            if (ins.Bank == 2003 && ins.ID == 66)
            {
                // Label18()
                grantMapEvent.Instructions[i] = new EMEVD.Instruction(1014, 18);
                grantMapEvent.Parameters.RemoveAll(p => p.InstructionIndex == i);
            }
        }
        // Altus check. Dynamically rewriting this is possible but annoying, so recreate it from scratch.
        List<EMEVD.Instruction> rewriteInstrs = new() {
            // EndIfPlayerIsInWorldType(EventEndType.End, WorldType.OtherWorld)
            new EMEVD.Instruction(1003, 14, new List<object> { (byte)0, (byte)1 }),
            // SetEventFlag(TargetEventFlagType.EventFlag, 3063, OFF)
            new EMEVD.Instruction(2003, 66, new List<object> { (byte)0, (uint)3063, (byte)0 }),
            // Cut condition: IfBatchEventFlags(MAIN, LogicalOperationType.NotAllOFF, TargetEventFlagType.EventFlag, 76300, 76399)
            // IfBatchEventFlags(OR_01, LogicalOperationType.NotAllOFF, TargetEventFlagType.EventFlag, 76300, altusId - 1)
            new EMEVD.Instruction(3, 1, new List<object> { (sbyte)-1, (byte)2, (byte)0, 76300, altusId - 1 }),
            // IfBatchEventFlags(OR_01, LogicalOperationType.NotAllOFF, TargetEventFlagType.EventFlag, altusId + 1, gelmirId - 1)
            new EMEVD.Instruction(3, 1, new List<object> { (sbyte)-1, (byte)2, (byte)0, altusId + 1, gelmirId - 1 }),
            // IfBatchEventFlags(OR_01, LogicalOperationType.NotAllOFF, TargetEventFlagType.EventFlag, gelmirId + 1, 76399)
            new EMEVD.Instruction(3, 1, new List<object> { (sbyte)-1, (byte)2, (byte)0, gelmirId + 1, 76399 }),
            // IfConditionGroup(MAIN, ON, OR_01)
            new EMEVD.Instruction(0, 0, new List<object> { (sbyte)0, (byte)1, (sbyte)-1 }),
            // SetEventFlag(TargetEventFlagType.EventFlag, 3063, ON)
            new EMEVD.Instruction(2003, 66, new List<object> { (byte)0, (uint)3063, (byte)1 }),
            // EndUnconditionally(EventEndType.End)
        };
        reachedAltusEvent.Instructions = rewriteInstrs;
        reachedAltusEvent.Parameters.Clear();
    }
    private void patchAtkParam() {
        Param.Row swarmOfFlies1 = _resources.AtkParamPc[72100] ??
                                  throw new InvalidOperationException("Entry 72100 not found in AtkParam_Pc");
        Param.Row swarmOfFlies2 = _resources.AtkParamPc[72101] ??
                                  throw new InvalidOperationException("Entry 72101 not found in AtkParam_Pc");

        AtkParam swarmAtkParam1 = new(swarmOfFlies1);
        AtkParam swarmAtkParam2 = new(swarmOfFlies2);
        patchSpEffectAtkPowerCorrectRate(swarmAtkParam1);
        patchSpEffectAtkPowerCorrectRate(swarmAtkParam2);
    }
    private static void patchSpEffectAtkPowerCorrectRate(AtkParam atkParam) {
        atkParam.spEffectAtkPowerCorrectRate_byPoint = 100;
        atkParam.spEffectAtkPowerCorrectRate_byRate = 100;
        atkParam.spEffectAtkPowerCorrectRate_byDmg = 100;
    }
}
