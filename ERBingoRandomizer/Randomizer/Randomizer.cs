using ERBingoRandomizer.FileHandler;
using ERBingoRandomizer.Params;
using FSParam;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static ERBingoRandomizer.Const;
using static ERBingoRandomizer.Utility.Config;
using static FSParam.Param;


namespace ERBingoRandomizer.Randomizer;

public partial class BingoRandomizer {
    public SeedInfo SeedInfo { get; private set; }

    private readonly string _path;
    private readonly string _regulationPath;
    private BND4 _regulationBnd;
    private readonly string _seed;
    private int _seedInt;
    private readonly Random _random;
    private BHD5Reader _bhd5Reader;
    private IntPtr _oodlePtr;
    // FMGs
    private BND4 _menuMsgBND;
    private FMG _lineHelpFmg;
    private FMG _menuTextFmg;
    private FMG _weaponFmg;
    private FMG _protectorFmg;
    private FMG _goodsFmg;
    // Params
    private List<PARAMDEF> _paramDefs;
    private Param _equipParamWeapon;
    private Param _equipParamCustomWeapon;
    private Param _equipParamGoods;
    private Param _equipParamProtector;
    private Param _charaInitParam;
    private Param _goodsParam;
    private Param _itemLotParam_map;
    private Param _itemLotParam_enemy;
    private Param _shopLineupParam;
    private Param _atkParam_Pc;
    // Dictionaries
    private Dictionary<int, EquipParamWeapon> _weaponDictionary;
    private Dictionary<int, EquipParamWeapon> _customWeaponDictionary;
    private Dictionary<int, string> _weaponNameDictionary;
    private Dictionary<int, EquipParamGoods> _goodsDictionary;
    private Dictionary<int, Magic> _magicDictionary;
    private Dictionary<ushort, List<Row>> _weaponTypeDictionary;
    private Dictionary<byte, List<Row>> _armorTypeDictionary;
    private Dictionary<byte, List<Row>> _magicTypeDictionary;
    // Cancellation Token
    private readonly CancellationToken _cancellationToken;
    private BingoRandomizer(string path, string seed, CancellationToken cancellationToken) {
        _path = Path.GetDirectoryName(path) ?? throw new InvalidOperationException("Path.GetDirectoryName(path) was null. Incorrect path provided.");
        _regulationPath = $"{_path}/{RegulationName}";
        _seed = string.IsNullOrWhiteSpace(seed) ? Random.Shared.NextInt64().ToString() : seed.Trim();
        byte[] hashData = SHA256.HashData(Encoding.UTF8.GetBytes(_seed));
        _seedInt = getSeedFromHashData(hashData);
        _random = new Random(_seedInt);
        _cancellationToken = cancellationToken;
    }
    public Task RandomizeRegulation() {
        //calculateLevels();
        _randomizerLog = new List<string>();
        randomizeCharaInitParam();
        _cancellationToken.ThrowIfCancellationRequested();
        randomizeItemLotParams();
        _cancellationToken.ThrowIfCancellationRequested();
        randomizeShopLineupParam();
        _cancellationToken.ThrowIfCancellationRequested();
        randomizeShopLineupParamMagic();
        _cancellationToken.ThrowIfCancellationRequested();
        patchAtkParam();
        _cancellationToken.ThrowIfCancellationRequested();
        writeFiles();
        writeLog();
        SeedInfo = new SeedInfo(_seed,
            BitConverter.ToString(SHA256.HashData(File.ReadAllBytes($"{BingoPath}/{RegulationName}"))).Replace("-", ""));
        string seedJson = JsonSerializer.Serialize(SeedInfo);
        File.WriteAllText(LastSeedPath, seedJson);
        return Task.CompletedTask;
    }
    private void patchAtkParam() {
        Row? swarmOfFlies1 = _atkParam_Pc[72100];
        Row? swarmOfFlies2 = _atkParam_Pc[72101];

        AtkParam swarmAtkParam1 = new(swarmOfFlies1 ?? throw new InvalidOperationException());
        AtkParam swarmAtkParam2 = new(swarmOfFlies2 ?? throw new InvalidOperationException());
        patchSpEffectAtkPowerCorrectRate(swarmAtkParam1);
        patchSpEffectAtkPowerCorrectRate(swarmAtkParam2);
    }
    private void calculateLevels() {
        for (int i = 0; i < 10; i++) {
            Row? row = _charaInitParam[i + 3000];
            if (row != null) {
                CharaInitParam chr = new(row);

                Debug.WriteLine($"{_menuTextFmg[i + 288100]} {chr.soulLv} {addLevels(chr)}");
            }
        }
    }
    private int addLevels(CharaInitParam chr) {
        return chr.baseVit
            + chr.baseWil
            + chr.baseEnd
            + chr.baseStr
            + chr.baseDex
            + chr.baseMag
            + chr.baseFai
            + chr.baseLuc;
        ;
    }
    private void randomizeCharaInitParam() {
        logItem(">>Class Randomization - All items are randomized, with each class having a .001% chance to gain or lose and item. Spells given class meets min stat requirements");
        logItem("Ammo is give if you get a ranged weapon. Catalyst is give if you have spells.\n");
        IEnumerable<int> remembranceItems = _shopLineupParam.Rows.Where(r => r.ID >= 101900 && r.ID <= 101929).Select(r => new ShopLineupParam(r).equipId);
        List<Row> staves = _weaponTypeDictionary[StaffType];
        List<Row> seals = _weaponTypeDictionary[SealType];
        List<int> weapons = _weaponDictionary.Keys.Select(id => removeWeaponMetadata(id)).Distinct()
            .Where(id => remembranceItems.All(i => i != id)).Where(id => staves.All(s => s.ID != id) && seals.All(s => s.ID != id))
            .OrderBy(i => _random.Next()).ToList();

        List<int> spells = _magicDictionary.Keys.Select(id => id).Distinct()
            .Where(id => remembranceItems.All(r => r != id))
            .Where(id => staves.All(s => s.ID != id) && seals.All(s => s.ID != id)).OrderBy(i => _random.Next()).ToList();

        for (int i = 0; i < 10; i++) {
            Row? row = _charaInitParam[i + 3000];
            if (row != null) {
                CharaInitParam param = new(row);
                randomizeCharaInitEntry(param, weapons, spells);
                if (row.ID == 3008) {
                    guaranteePrisonerHasSpells(param, spells);
                }
                if (row.ID == 3006) {
                    guaranteeConfessorHasIncantation(param, spells);
                }
                logCharaInitEntry(param, i + 288100);
                addDescriptionString(param, ChrInfoMapping[i]);
            }
        }
    }
    private void guaranteePrisonerHasSpells(CharaInitParam chr, List<int> spells) {
        if (hasSpellOfType(chr, SorceryType)) {
            return;
        }
        // Get a new random chr until it has the required stats.
        while (chr.baseMag < MinInt) {
            randomizeLevels(chr);
        }

        chr.equipSpell01 = -1;
        chr.equipSpell02 = -1;
        randomizeSorceries(chr, spells);

        // Get Incantations if the new class has the requirements. 
        // if (chr.baseFai >= MinFai) {
        //     randomizeIncantations(chr, spells);
        // }
    }
    private void guaranteeConfessorHasIncantation(CharaInitParam chr, List<int> spells) {
        if (hasSpellOfType(chr, IncantationType)) {
            return;
        }
        // Get a new random chr until it has the required stats.
        while (chr.baseFai < MinFai) {
            randomizeLevels(chr);
        }

        chr.equipSpell01 = -1;
        chr.equipSpell02 = -1;
        randomizeIncantations(chr, spells);

        // Get Sorceries if the new class has the requirements. 
        // if (chr.baseMag >= MinInt) {
        //     randomizeSorceries(chr, spells);
        // }
    }
    private void randomizeCharaInitEntry(CharaInitParam chr, List<int> weapons, List<int> spells) {
        chr.wepleft = getRandomWeapon(chr.wepleft, weapons);
        chr.wepRight = getRandomWeapon(chr.wepRight, weapons);
        chr.subWepLeft = -1;
        chr.subWepRight = -1;
        chr.subWepLeft3 = -1;
        chr.subWepRight3 = -1;

        chr.equipHelm = chanceGetRandomArmor(chr.equipHelm, HelmType);
        chr.equipArmer = chanceGetRandomArmor(chr.equipArmer, BodyType);
        chr.equipGaunt = chanceGetRandomArmor(chr.equipGaunt, ArmType);
        chr.equipLeg = chanceGetRandomArmor(chr.equipLeg, LegType);

        randomizeLevels(chr);

        chr.equipArrow = NoItem;
        chr.arrowNum = ushort.MaxValue;
        if (hasWeaponOfType(chr, BowType, LightBowType)) {
            giveArrows(chr);
        }
        chr.equipSubArrow = NoItem;
        chr.subArrowNum = ushort.MaxValue;
        if (hasWeaponOfType(chr, GreatbowType)) {
            giveGreatArrows(chr);
        }
        chr.equipBolt = NoItem;
        chr.boltNum = ushort.MaxValue;
        if (hasWeaponOfType(chr, CrossbowType)) {
            giveBolts(chr);
        }
        chr.equipSubBolt = NoItem;
        chr.subBoltNum = ushort.MaxValue;
        if (hasWeaponOfType(chr, BallistaType)) {
            giveBallistaBolts(chr);
        }

        chr.equipSpell01 = -1;
        chr.equipSpell02 = -1;
        // if (chr.baseMag >= MinInt) {
        //     randomizeSorceries(chr, spells);
        // }
        // if (chr.baseFai >= MinFai) {
        //     randomizeIncantations(chr, spells);
        // }
    }
    private void randomizeItemLotParams() {
        OrderedDictionary categoryDictEnemy = new();
        OrderedDictionary categoryDictMap = new();
        OrderedDictionary categoryDictMagic = new();

        foreach (Row row in _itemLotParam_enemy.Rows.Concat(_itemLotParam_map.Rows)) {
            Column[] itemIds = row.Cells.Take(ItemLots).ToArray();
            Column[] categories = row.Cells.Skip(CategoriesStart).Take(ItemLots).ToArray();
            Column[] chances = row.Cells.Skip(ChanceStart).Take(ItemLots).ToArray();
            int totalWeight = chances.Sum(a => (ushort)a.GetValue(row));
            for (int i = 0; i < ItemLots; i++) {
                int category = (int)categories[i].GetValue(row);
                if (category != ItemLotWeaponCategory && category != ItemLotCustomWeaponCategory && category != ItemLotGoodsCategory) {
                    continue;
                }

                EquipParamWeapon? wep;
                int id = (int)itemIds[i].GetValue(row);
                int sanitizedId = removeWeaponLevels(id);
                if (category == ItemLotWeaponCategory) {
                    if (!_weaponDictionary.TryGetValue(sanitizedId, out wep))
                        continue;
                    if (id != sanitizedId) {
                        _weaponNameDictionary[id] = $"{_weaponNameDictionary[sanitizedId]} + {id - sanitizedId}";
                    }
                    ushort chance = (ushort)chances[i].GetValue(row);
                    if (chance == totalWeight) {
                        addToOrderedDict(categoryDictMap, wep.wepType, new ItemLotEntry(id, category));
                        break; // Break here because the entire item lot param is just a single entry.
                    }

                    addToOrderedDict(categoryDictEnemy, wep.wepType, new ItemLotEntry(id, category));
                }
                else if (category == ItemLotCustomWeaponCategory) {
                    if (!_customWeaponDictionary.TryGetValue(id, out wep))
                        continue;
                    ushort chance = (ushort)chances[i].GetValue(row);
                    if (chance == totalWeight) {
                        addToOrderedDict(categoryDictMap, wep.wepType, new ItemLotEntry(id, category));
                        break;
                    }

                    addToOrderedDict(categoryDictEnemy, wep.wepType, new ItemLotEntry(id, category));

                }
                else { // category == ItemLotGoodsCategory
                    if (!_magicDictionary.TryGetValue(id, out Magic? magic))
                        continue;
                    ushort chance = (ushort)chances[i].GetValue(row);
                    if (chance == totalWeight) {
                        addToOrderedDict(categoryDictMagic, magic.ezStateBehaviorType, new ItemLotEntry(id, category));
                        break;
                    }
                    addToOrderedDict(categoryDictMagic, magic.ezStateBehaviorType, new ItemLotEntry(id, category));
                }

            }
        }

        dedupeAndRandomizeVectors(categoryDictMap);
        dedupeAndRandomizeVectors(categoryDictEnemy);
        dedupeAndRandomizeVectors(categoryDictMagic);

        Dictionary<int, ItemLotEntry> guaranteedDropReplace = getReplacementHashmap(categoryDictMap);
        Dictionary<int, ItemLotEntry> chanceDropReplace = getReplacementHashmap(categoryDictEnemy);
        Dictionary<int, ItemLotEntry> magicReplace = getReplacementHashmap(categoryDictMagic);
        logItem(">> Item Replacements - all instances of item on left will be replaced with item on right");
        logItem("> Guaranteed Weapons");
        logReplacementDictionary(guaranteedDropReplace);
        logItem("> Chance Weapons");
        logReplacementDictionary(chanceDropReplace);
        logItem("> Magic");
        logReplacementDictionaryMagic(magicReplace);
        logItem("");


        foreach (Row row in _itemLotParam_enemy.Rows.Concat(_itemLotParam_map.Rows)) {
            Column[] itemIds = row.Cells.Take(ItemLots).ToArray();
            Column[] categories = row.Cells.Skip(CategoriesStart).Take(ItemLots).ToArray();
            for (int i = 0; i < ItemLots; i++) {
                int category = (int)categories[i].GetValue(row);
                if (category != ItemLotWeaponCategory && category != ItemLotCustomWeaponCategory && category != ItemLotGoodsCategory) {
                    continue;
                }

                int id = (int)itemIds[i].GetValue(row);
                if (category == ItemLotWeaponCategory) {
                    if (!_weaponDictionary.TryGetValue(removeWeaponLevels(id), out EquipParamWeapon _))
                        continue;

                    ItemLotEntry entry;
                    if (guaranteedDropReplace.TryGetValue(id, out entry)) {
                        itemIds[i].SetValue(row, entry.Id);
                        categories[i].SetValue(row, entry.Category);
                        break;
                    }
                    if (!chanceDropReplace.TryGetValue(id, out entry))
                        continue;
                    itemIds[i].SetValue(row, entry.Id);
                    categories[i].SetValue(row, entry.Category);
                }
                else if (category == ItemLotCustomWeaponCategory) {
                    if (!_customWeaponDictionary.TryGetValue(id, out EquipParamWeapon _))
                        continue;

                    ItemLotEntry entry;
                    if (guaranteedDropReplace.TryGetValue(id, out entry)) {
                        itemIds[i].SetValue(row, entry.Id);
                        categories[i].SetValue(row, entry.Category);
                    }
                    if (!chanceDropReplace.TryGetValue(id, out entry))
                        continue;
                    itemIds[i].SetValue(row, entry.Id);
                    categories[i].SetValue(row, entry.Category);
                }
                else { // category == ItemLotGoodsCategory
                    if (!_magicDictionary.TryGetValue(id, out Magic _))
                        continue;

                    ItemLotEntry entry;
                    if (!magicReplace.TryGetValue(id, out entry))
                        continue;
                    itemIds[i].SetValue(row, entry.Id);
                    categories[i].SetValue(row, entry.Category);
                    break;
                }
            }
        }
    }
    private void randomizeShopLineupParam() {
        List<ShopLineupParam> shopLineupParamRemembranceList = new();
        foreach (Row row in _shopLineupParam.Rows) {
            if ((byte)row["equipType"]!.Value.Value != ShopLineupWeaponCategory || row.ID < 101900 || row.ID > 101980) {
                continue;
            }

            ShopLineupParam lot = new(new Row(row));
            int sanitizedId = removeWeaponLevels(lot.equipId);
            if (_weaponDictionary.TryGetValue(sanitizedId, out _)) {
                if (lot.equipId != sanitizedId) {
                    _weaponNameDictionary[lot.equipId] = $"{_weaponNameDictionary[sanitizedId]} +{lot.equipId - sanitizedId}";
                }
                shopLineupParamRemembranceList.Add(lot);
            }
        }

        List<int> shopLineupParamList = _weaponDictionary.Keys.Select(removeWeaponMetadata).Distinct().Where(i => shopLineupParamRemembranceList.All(s => s.equipId != i)).OrderBy(i => _random.Next()).ToList();
        shopLineupParamRemembranceList = shopLineupParamRemembranceList.OrderBy(i => _random.Next()).ToList();
        logItem(">> Shop Replacements - Random item selected from pool of all weapons (not including infused weapons). Remembrances are randomized amongst each-other.");

        foreach (Row row in _shopLineupParam.Rows) {
            logShopId(row.ID);
            if ((byte)row["equipType"]!.Value.Value != ShopLineupWeaponCategory || row.ID > 101980) {
                continue;
            }

            ShopLineupParam lot = new(row);
            if (_weaponDictionary.TryGetValue(removeWeaponLevels(lot.equipId), out _)) {
                replaceShopLineupParam(lot, shopLineupParamList, shopLineupParamRemembranceList);
            }
        }
    }
    private void randomizeShopLineupParamMagic() {
        OrderedDictionary magicCategoryDictMap = new();
        List<ShopLineupParam> shopLineupParamRemembranceList = new();
        List<ShopLineupParam> shopLineupParamDragonList = new();
        foreach (Row row in _shopLineupParam.Rows) {
            if ((byte)row["equipType"]!.Value.Value != ShopLineupGoodsCategory || row.ID > 101980) {
                continue;
            }

            ShopLineupParam lot = new(new Row(row));
            if (_magicDictionary.TryGetValue(lot.equipId, out Magic? magic)) {
                if (row.ID < 101950) {
                    if (lot.mtrlId == -1) {
                        addToOrderedDict(magicCategoryDictMap, magic.ezStateBehaviorType, lot);
                        continue;
                    }
                    shopLineupParamRemembranceList.Add(lot);
                } else { // Dragon Communion Shop
                    shopLineupParamDragonList.Add(lot);
                }
            }
        }

        dedupeAndRandomizeShopVectors(magicCategoryDictMap);

        Dictionary<int, ShopLineupParam> magicShopReplacement = getShopReplacementHashmap(magicCategoryDictMap);
        // for (int i = 0; i < magicCategoryDictMap.Count; i++) {
        //     List<ShopLineupParam> value = (List<ShopLineupParam>)magicCategoryDictMap[i]!;
        //     magicCategoryDictMap[i] = value.Where(s => shopLineupParamRemembranceList.All(r => r.equipId != s.equipId)).OrderBy(_ => _random.Next()).ToList();
        // }
        shopLineupParamRemembranceList = shopLineupParamRemembranceList.OrderBy(i => _random.Next()).ToList();
        shopLineupParamDragonList = shopLineupParamDragonList.OrderBy(i => _random.Next()).ToList();
        logItem("\n>> Shop Magic - Random item selected from pool of all weapons (not including infused weapons). Remembrances are randomized amongst each-other.");

        foreach (Row row in _shopLineupParam.Rows) {
            logShopIdMagic(row.ID);
            if ((byte)row["equipType"]!.Value.Value != ShopLineupGoodsCategory || row.ID > 101980) {
                continue;
            }

            ShopLineupParam lot = new(row);
            if (_magicDictionary.TryGetValue(lot.equipId, out _)) {
                if (row.ID < 101950) {
                    replaceShopLineupParamMagic(lot, magicShopReplacement, shopLineupParamRemembranceList);
                }
                else {
                    ShopLineupParam newDragonIncant = getNewId(lot.equipId, shopLineupParamDragonList);
                    logItem($"{_goodsFmg[lot.equipId]} -> {_goodsFmg[newDragonIncant.equipId]}");
                    copyShopLineupParam(lot, newDragonIncant);
                }
            }
        }
    }
}
