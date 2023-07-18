using ERBingoRandomizer.FileHandler;
using ERBingoRandomizer.Params;
using FSParam;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static ERBingoRandomizer.Utility.Const;
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
    private Param _magicParam;
    private Param _itemLotParam_map;
    private Param _itemLotParam_enemy;
    private Param _shopLineupParam;
    // Dictionaries
    private Dictionary<int, EquipParamWeapon> _weaponDictionary;
    private Dictionary<int, EquipParamWeapon> _customWeaponDictionary;
    private Dictionary<int, string> _weaponNameDictionary;
    private Dictionary<int, Row> _goodsDictionary;
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
        _randomizerLog = new List<string>();
        randomizeCharaInitParam();
        _cancellationToken.ThrowIfCancellationRequested();
        randomizeItemLotParams();
        _cancellationToken.ThrowIfCancellationRequested();
        randomizeShopLineupParam();
        _cancellationToken.ThrowIfCancellationRequested();
        writeFiles();
        writeLog();
        SeedInfo = new SeedInfo(_seed,
            BitConverter.ToString(SHA256.HashData(File.ReadAllBytes($"{BingoPath}/{RegulationName}"))).Replace("-", ""));
        string seedJson = JsonSerializer.Serialize(SeedInfo);
        File.WriteAllText(LastSeedPath, seedJson);
        return Task.CompletedTask;
    }
    private void randomizeCharaInitParam() {
        logItem(">>Class Randomization - All items are randomized, with each class having a .001% chance to gain or lose and item. Spells given class meets min stat requirements");
        logItem("Ammo is give if you get a ranged weapon. Catalyst is give if you have spells.\n");
        List<int> weapons = _weaponDictionary.Keys.Select(id => removeWeaponMetadata(id)).Distinct().OrderBy(i => _random.Next()).ToList();
        for (int i = 0; i < 10; i++) {
            Row? row = _charaInitParam[i + 3000];
            if (row != null) {
                CharaInitParam param = new(row);
                randomizeCharaInitEntry(param, weapons);
                logCharaInitEntry(param, i + 288100);
                addDescriptionString(param, ChrInfoMapping[i]);
            }
        }
    }
    private void randomizeCharaInitEntry(CharaInitParam chr, List<int> weapons) {
        chr.wepleft = chanceGetRandomWeapon(chr.wepleft, weapons);
        chr.wepRight = getRandomWeapon(chr.wepRight, weapons);
        chr.subWepLeft = chanceGetRandomWeapon(chr.subWepLeft, weapons);
        chr.subWepRight = chanceGetRandomWeapon(chr.subWepRight, weapons);
        chr.subWepLeft3 = chanceGetRandomWeapon(chr.subWepLeft3, weapons);
        chr.subWepRight3 = chanceGetRandomWeapon(chr.subWepRight3, weapons);

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
        if (chr.baseMag >= MinInt) {
            randomizeSorceries(chr);
        }
        if (chr.baseFai >= MinFai) {
            randomizeIncantations(chr);
        }
    }
    private void randomizeItemLotParams() {
        OrderedDictionary categoryDictEnemy = new();
        OrderedDictionary categoryDictMap = new();

        foreach (Row row in _itemLotParam_enemy.Rows.Concat(_itemLotParam_map.Rows)) {
            Column[] itemIds = row.Cells.Take(ItemLots).ToArray();
            Column[] categories = row.Cells.Skip(CategoriesStart).Take(ItemLots).ToArray();
            Column[] chances = row.Cells.Skip(ChanceStart).Take(ItemLots).ToArray();
            int totalWeight = chances.Sum(a => (ushort)a.GetValue(row));
            for (int i = 0; i < ItemLots; i++) {
                int category = (int)categories[i].GetValue(row);
                if (category != ItemLotWeaponCategory && category != ItemLotCustomWeaponCategory) {
                    continue;
                }


                EquipParamWeapon wep;
                int id = (int)itemIds[i].GetValue(row);
                int sanitizedId = removeWeaponLevels(id);
                if (category == ItemLotWeaponCategory) {
                    if (_weaponDictionary.TryGetValue(sanitizedId, out wep)) {
                        if (id != sanitizedId) {
                            _weaponNameDictionary[id] = $"{_weaponNameDictionary[sanitizedId]} + {id - sanitizedId}";
                        }
                        ushort chance = (ushort)chances[i].GetValue(row);
                        if (chance == totalWeight) {
                            addToOrderedDict(categoryDictMap, wep, id);
                            break;
                        }

                        addToOrderedDict(categoryDictEnemy, wep, id);
                    }
                    continue;
                }

                if (_customWeaponDictionary.TryGetValue(id, out wep)) {
                    ushort chance = (ushort)chances[i].GetValue(row);
                    if (chance == totalWeight) {
                        addToOrderedDict(categoryDictMap, wep, id);
                    }
                    else {
                        addToOrderedDict(categoryDictEnemy, wep, id);
                    }
                }


            }
        }

        dedupVectors(categoryDictMap);
        dedupVectors(categoryDictEnemy);

        Dictionary<int, int> guaranteedDropReplace = getReplacementHashmap(categoryDictMap);
        Dictionary<int, int> chanceDropReplace = getReplacementHashmap(categoryDictEnemy);
        logItem(">>Item Replacements - all instances of item on left will be replaced with item on right");
        logReplacementDictionary(guaranteedDropReplace);
        logReplacementDictionary(chanceDropReplace);
        logItem("");


        foreach (Row row in _itemLotParam_enemy.Rows.Concat(_itemLotParam_map.Rows)) {
            Column[] itemIds = row.Cells.Take(ItemLots).ToArray();
            Column[] categories = row.Cells.Skip(CategoriesStart).Take(ItemLots).ToArray();
            for (int i = 0; i < ItemLots; i++) {
                int category = (int)categories[i].GetValue(row);
                if (category != ItemLotWeaponCategory && category != ItemLotCustomWeaponCategory) {
                    continue;
                }

                int id = (int)itemIds[i].GetValue(row);
                if (category == ItemLotWeaponCategory) {
                    if (_weaponDictionary.TryGetValue(removeWeaponLevels(id), out EquipParamWeapon _)) {
                        int newId;
                        if (guaranteedDropReplace.TryGetValue(id, out newId)) {
                            itemIds[i].SetValue(row, newId);
                        }
                        else if (chanceDropReplace.TryGetValue(id, out newId)) {
                            itemIds[i].SetValue(row, newId);
                        }
                    }
                    continue;
                }
                if (_customWeaponDictionary.TryGetValue(id, out EquipParamWeapon _)) {
                    int newId;
                    if (guaranteedDropReplace.TryGetValue(id, out newId)) {
                        itemIds[i].SetValue(row, newId);
                    }
                    else if (chanceDropReplace.TryGetValue(id, out newId)) {
                        itemIds[i].SetValue(row, newId);
                    }
                }
            }
        }
    }
    private void randomizeShopLineupParam() {
        List<ShopLineupParam> shopLineupParamRemembranceList = new();
        foreach (Row row in _shopLineupParam.Rows) {
            if ((byte)row["equipType"].Value.Value != ShopLineupWeaponCategory || row.ID < 101900 || row.ID > 101929) {
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

        List<int> shopLineupParamList = _weaponDictionary.Keys.Select(i => removeWeaponMetadata(i)).Distinct().OrderBy(i => _random.Next()).ToList();
        shopLineupParamRemembranceList = shopLineupParamRemembranceList.OrderBy(i => _random.Next()).ToList();
        logItem(">> Shop Replacements - Random item selected from pool of all weapons (not including infused weapons). Remembrances are randomized amongst each-other.");

        foreach (Row row in _shopLineupParam.Rows) {
            logShopId(row.ID);
            if ((byte)row["equipType"].Value.Value != ShopLineupWeaponCategory || row.ID > 112091) {
                continue;
            }

            ShopLineupParam lot = new(row);
            EquipParamWeapon wep;
            if (_weaponDictionary.TryGetValue(removeWeaponLevels(lot.equipId), out wep)) {
                replaceShopLineupParam(lot, shopLineupParamList, wep, shopLineupParamRemembranceList);
            }
        }
    }
}
