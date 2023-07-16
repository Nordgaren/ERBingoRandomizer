using ERBingoRandomizer.FileHandler;
using ERBingoRandomizer.Params;
using ERBingoRandomizer.Resources.MSB;
using ERBingoRandomizer.Utility;
using FSParam;
using SoulsFormats;
using System;
using System.Collections;
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
using static ERBingoRandomizer.Utility.Const;
using static ERBingoRandomizer.Utility.Config;
using static FSParam.Param;


namespace ERBingoRandomizer.Randomizer;

public partial class BingoRandomizer {
    public SeedInfo SeedInfo { get; private set; }

    private string _path;
    private string _regulationPath;
    private BND4 _regulationBnd;
    private string _seed;
    private int _seedInt;
    private Random _random;
    private BHD5Reader _bhd5Reader;
    private IntPtr _oodlePtr;
    // FMGs
    private BND4 _menuMsgBND;
    private FMG _lineHelpFmg;
    private FMG _menuTextFmg;
    private FMG _weaponFmg;
    private FMG _protectorFmg;
    private FMG _goodsFmg;
    private FMG _accessoryFmg;
    // MSBs
    private List<MSBE> _msbs;

    private List<PARAMDEF> _paramDefs;
    // Params
    private Param _equipParamWeapon;
    private Param _equipParamCustomWeapon;
    private Param _equipParamGoods;
    private Param _equipParamProtector;
    private Param _equipParamAccessory;
    private Param _charaInitParam;
    private Param _magicParam;
    private Param _itemLotParam_map;
    private Param _itemLotParam_enemy;
    private Param _shopLineupParam;

    // Dictionaries
    private Dictionary<int, EquipParamWeapon> _weaponDictionary;
    private Dictionary<int, string> _weaponNameDictionary;
    private Dictionary<int, EquipParamWeapon> _customWeaponDictionary;
    private Dictionary<int, Row> _armorDictionary;
    private Dictionary<int, Row> _goodsDictionary;
    private Dictionary<int, Magic> _magicDictionary;
    private Dictionary<int, Row> _accessoryDictionary;
    private Dictionary<ushort, List<Row>> _weaponTypeDictionary;
    private Dictionary<byte, List<Row>> _armorTypeDictionary;
    private Dictionary<byte, List<Row>> _magicTypeDictionary;

    // HashSets
    private HashSet<int> _itemLotParam_mapRefs;
    private HashSet<int> _itemLotParam_enemyRefs;
    private readonly CancellationToken _cancellationToken;
    //static async method that behave like a constructor       
    public static async Task<BingoRandomizer> BuildRandomizerAsync(string path, string seed, CancellationToken cancellationToken) {
        BingoRandomizer rando = new(path, seed, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        await Task.Run(() => rando.init());
        return rando;
    }
    private BingoRandomizer(string path, string seed, CancellationToken cancellationToken) {
        _path = Path.GetDirectoryName(path) ?? throw new InvalidOperationException("Path.GetDirectoryName(path) was null. Incorrect path provided.");
        _regulationPath = $"{_path}/{RegulationName}";
        _seed = string.IsNullOrWhiteSpace(seed) ? Random.Shared.NextInt64().ToString() : seed.Trim();
        byte[] hashData = SHA256.HashData(Encoding.UTF8.GetBytes(_seed));
        _seedInt = getSeedFromHashData(hashData);
        _random = new Random(_seedInt);
        _cancellationToken = cancellationToken;
    }
    private int getSeedFromHashData(byte[] hashData) {
        IEnumerable<byte[]> chunks = hashData.Chunk(4);
        int num = 0;
        foreach (byte[] chunk in chunks) {
            num ^= BitConverter.ToInt32(chunk);
        }

        return num;
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
    private void writeFiles() {
        if (Directory.Exists(BingoPath)) {
            Directory.Delete(BingoPath, true);
        }
        Directory.CreateDirectory(Path.GetDirectoryName($"{BingoPath}/{RegulationName}"));
        setBndFile(_regulationBnd, CharaInitParamName, _charaInitParam.Write());
        setBndFile(_regulationBnd, ItemLotParam_mapName, _itemLotParam_map.Write());
        setBndFile(_regulationBnd, ItemLotParam_enemyName, _itemLotParam_enemy.Write());
        setBndFile(_regulationBnd, ShopLineupParamName, _shopLineupParam.Write());
        SFUtil.EncryptERRegulation($"{BingoPath}/{RegulationName}", _regulationBnd);
        Directory.CreateDirectory(Path.GetDirectoryName($"{BingoPath}/{MenuMsgBNDPath}"));
        setBndFile(_menuMsgBND, GR_LineHelpName, _lineHelpFmg.Write());
        File.WriteAllBytes($"{BingoPath}/{MenuMsgBNDPath}", _menuMsgBND.Write());

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
                            _weaponNameDictionary[id] = $"{_weaponNameDictionary[sanitizedId]} + {id - sanitizedId}" ;
                        }
                        ushort chance = (ushort)chances[i].GetValue(row);
                        if (chance == totalWeight) {
                            addToOrderedDict(categoryDictMap, wep, id);
                        }
                        else {
                            addToOrderedDict(categoryDictEnemy, wep, id);
                        }
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
        logItem("Item Replacements - all instances of item on left will be replaced with item on right");
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
    private void logReplacementDictionary(Dictionary<int, int> dict) {
        foreach (KeyValuePair<int, int> pair in dict) {
            logItem($"{_weaponNameDictionary[pair.Key]} -> {_weaponNameDictionary[pair.Value]}");
        }
    }
    private void randomizeShopLineupParam() {
        OrderedDictionary shopLineupParamDictionary = new();
        List<ShopLineupParam> shopLineupParamRemembranceList = new();

        foreach (Row row in _shopLineupParam.Rows) {
            if ((byte)row["equipType"].Value.Value != ShopLineupWeaponCategory || row.ID >= 9000000) {
                continue;
            }

            ShopLineupParam lot = new(new Row(row));
            int sanitizedId = removeWeaponLevels(lot.equipId);
            EquipParamWeapon wep;
            if (_weaponDictionary.TryGetValue(sanitizedId, out wep)) {
                if (lot.equipId != sanitizedId) {
                    _weaponNameDictionary[lot.equipId] = $"{_weaponNameDictionary[sanitizedId]} +{lot.equipId - sanitizedId}" ;
                }
                addToShopLineupParamDict(lot, shopLineupParamDictionary, wep, shopLineupParamRemembranceList);
            }
            else if (_customWeaponDictionary.TryGetValue(lot.equipId, out wep)) {
                addToShopLineupParamDict(lot, shopLineupParamDictionary, wep, shopLineupParamRemembranceList);
            }
        }

        shuffleVectors(shopLineupParamDictionary);
        shopLineupParamRemembranceList = shopLineupParamRemembranceList.OrderBy(i => _random.Next()).ToList();
        logItem("Shop Replacements - Random item selected (by weapon category) from a pool of items consisting of only shop entries. Remembrances are randomized amongst each-other.");

        foreach (Row row in _shopLineupParam.Rows) {
            printShopId(row.ID);
            if ((byte)row["equipType"].Value.Value != ShopLineupWeaponCategory || row.ID >= 9000000) {
                continue;
            }


            ShopLineupParam lot = new(row);
            EquipParamWeapon wep;
            if (_weaponDictionary.TryGetValue(removeWeaponLevels(lot.equipId), out wep)) {
                replaceShopLineupParam(lot, shopLineupParamDictionary, wep, shopLineupParamRemembranceList);
            }
            else if (_weaponDictionary.TryGetValue(removeWeaponLevels(lot.equipId), out wep)) {
                replaceShopLineupParam(lot, shopLineupParamDictionary, wep, shopLineupParamRemembranceList);
            }
        }
    }
    private void printShopId(int rowId) {
        switch (rowId) {
            case 100000:
                logItem("\nGatekeeper Gostoc");
                break;
            case 100100:
                logItem("\nPatches");
                break;
            case 100325:
                logItem("\nPidia Carian Servant");
                break;
            case 100500:
                logItem("\nMerchant Kale");
                break;
            case 100525:
                logItem("\nMerchant - North Limgrave");
                break;
            case 100550:
                logItem("\nMerchant - East Limgrave");
                break;
            case 100575:
                logItem("\nMerchant - Coastal Cave");
                break;
            case 100600:
                logItem("\nMerchant - East Weeping Peninsula");
                break;
            case 100625:
                logItem("\nMerchant - Liurnia of the Lakes");
                break;
            case 100650:
                logItem("\nIsolated Merchant - Weeping Peninsula");
                break;
            case 100700:
                logItem("\nMerchant - North Liurnia");
                break;
            case 100725:
                logItem("\nHermit Merchant - Leyndell");
                break;
            case 100750:
                logItem("\nMerchant - Altus Plateau");
                break;
            case 100875:
                logItem("\nIsolated Merchant - Dragonbarrow");
                break;
            case 100925:
                logItem("\nMerchant - Siofra River");
                break;
            case 101800:
                logItem("\nTwin Maiden Husks");
                break;
            case 101900:
                logItem("\nRemembrances");
                break;
        }
    }
    private void replaceShopLineupParam(ShopLineupParam lot, OrderedDictionary shopLineupParamDictionary, EquipParamWeapon wep, List<ShopLineupParam> shopLineupParamRememberanceList) {
        if (lot.mtrlId == -1) {
            ShopLineupParam newLineup = getNewId(lot.equipId, (List<ShopLineupParam>)shopLineupParamDictionary[(object)wep.wepType]);
            logItem($"{_weaponFmg[lot.equipId]} -> {_weaponFmg[newLineup.equipId]}");
            copyShopLineupParam(lot, newLineup);
            return;
        }
        ShopLineupParam newRemembrance = getNewId(lot.equipId, shopLineupParamRememberanceList);
        logItem($"{_weaponFmg[lot.equipId]} -> {_weaponFmg[newRemembrance.equipId]}");
        copyShopLineupParam(lot, newRemembrance);
    }
    private static void addToShopLineupParamDict(ShopLineupParam lot, OrderedDictionary shopLineupParamDictionary, EquipParamWeapon wep, List<ShopLineupParam> shopLineupParamRememberanceList) {
        if (lot.mtrlId == -1) {
            addToOrderedDict(shopLineupParamDictionary, wep, lot);
            return;
        }
        shopLineupParamRememberanceList.Add(lot);
    }
    private void randomizeCharaInitParam() {
        for (int i = 0; i < 10; i++) {
            Row? row = _charaInitParam[i + 3000];
            if (row != null) {
                CharaInitParam param = new(row);
                randomizeCharaInitEntry(param);
                logCharaInitEntry(param, i + 288100);
                addDescriptionString(param, ChrInfoMapping[i]);
            }
        }
    }
    private void logCharaInitEntry(CharaInitParam chr, int i) {
        logItem($"{_menuTextFmg[i]} -");
        logItem("Weapons");
        if (chr.wepleft != -1) {
            logItem($"Left: {_weaponFmg[chr.wepleft]}");
        }
        if (chr.wepRight != -1) {
            logItem($"Right: {_weaponFmg[chr.wepRight]}");
        }
        if (chr.subWepLeft != -1) {
            logItem($"Left 2: {_weaponFmg[chr.subWepLeft]}");
        }
        if (chr.subWepRight != -1) {
            logItem($"Right 2: {_weaponFmg[chr.subWepRight]}");
        }
        if (chr.subWepLeft3 != -1) {
            logItem($"Left 3: {_weaponFmg[chr.subWepLeft3]}");
        }
        if (chr.subWepRight3 != -1) {
            logItem($"Right 3: {_weaponFmg[chr.subWepRight3]}");
        }
        logItem("");
        logItem("Armor");
        if (chr.equipHelm != -1) {
            logItem($"Helm: {_protectorFmg[chr.equipHelm]}");
        }
        if (chr.equipArmer != -1) {
            logItem($"Body: {_protectorFmg[chr.equipArmer]}");
        }
        if (chr.equipGaunt != -1) {
            logItem($"Arms: {_protectorFmg[chr.equipGaunt]}");
        }
        if (chr.equipLeg != -1) {
            logItem($"Legs: {_protectorFmg[chr.equipLeg]}");
        }
        logItem("");
        logItem("Levels");
        logItem($"Vigor: {chr.baseVit}");
        logItem($"Attunement: {chr.baseWil}");
        logItem($"Endurance: {chr.baseEnd}");
        logItem($"Strength: {chr.baseStr}");
        logItem($"Dexterity: {chr.baseDex}");
        logItem($"Intelligence: {chr.baseMag}");
        logItem($"Faith: {chr.baseFai}");
        logItem($"Arcane: {chr.baseLuc}");
        logItem("");
        logItem("Ammo");
        if (chr.equipArrow != -1) {
            logItem($"{_weaponFmg[chr.equipArrow]}[{chr.arrowNum}]");
        }
        if (chr.equipSubArrow != -1) {
            logItem($"{_weaponFmg[chr.equipSubArrow]}[{chr.subArrowNum}]");
        }
        if (chr.equipBolt != -1) {
            logItem($"{_weaponFmg[chr.equipBolt]}[{chr.boltNum}]");
        }
        if (chr.equipSubBolt != -1) {
            logItem($"{_weaponFmg[chr.equipSubBolt]}[{chr.subBoltNum}]");
        }
        logItem("");
        logItem("Spells");
        if (chr.equipSpell01 != -1) {
            logItem($"{_goodsFmg[chr.equipSpell01]}");
        }
        if (chr.equipSpell02 != -1) {
            logItem($"{_goodsFmg[chr.equipSpell02]}");
        }
        logItem("");
    }
    private void randomizeCharaInitEntry(CharaInitParam chr) {
        chr.wepleft = chanceGetRandomWeapon(chr.wepleft);
        chr.wepRight = getRandomWeapon(chr.wepRight);
        chr.subWepLeft = chanceGetRandomWeapon(chr.subWepLeft);
        chr.subWepRight = chanceGetRandomWeapon(chr.subWepRight);
        chr.subWepLeft3 = chanceGetRandomWeapon(chr.subWepLeft3);
        chr.subWepRight3 = chanceGetRandomWeapon(chr.subWepRight3);

        chr.equipHelm = chanceGetRandomArmor(chr.equipHelm, HelmType);
        chr.equipArmer = chanceGetRandomArmor(chr.equipArmer, BodyType);
        chr.equipGaunt = chanceGetRandomArmor(chr.equipGaunt, ArmType);
        chr.equipLeg = chanceGetRandomArmor(chr.equipLeg, LegType);

        randomizeLevels(chr);

        chr.equipArrow = NoItem;
        chr.arrowNum = ushort.MaxValue;
        if (hasBow(chr)) {
            giveArrows(chr);
        }
        chr.equipSubArrow = NoItem;
        chr.subArrowNum = ushort.MaxValue;
        if (hasGreatbow(chr)) {
            giveGreatArrows(chr);
        }
        chr.equipBolt = NoItem;
        chr.boltNum = ushort.MaxValue;
        if (hasCrossbow(chr)) {
            giveBolts(chr);
        }
        chr.equipSubBolt = NoItem;
        chr.subBoltNum = ushort.MaxValue;
        if (hasBallista(chr)) {
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
    private void addDescriptionString(CharaInitParam chr, int id) {
        //297135
        List<string> str = new();
        str.Add(_weaponFmg[chr.wepRight]);

        if (chr.wepleft != -1) {
            str.Add($"{_weaponFmg[chr.wepleft]}");
        }
        if (chr.subWepLeft != -1) {
            str.Add($"{_weaponFmg[chr.subWepLeft]}");
        }
        if (chr.subWepRight != -1) {
            str.Add($"{_weaponFmg[chr.subWepRight]}");
        }
        if (chr.subWepLeft3 != -1) {
            str.Add($"{_weaponFmg[chr.subWepLeft3]}");
        }
        if (chr.subWepRight3 != -1) {
            str.Add($"{_weaponFmg[chr.subWepRight3]}");
        }
        if (chr.equipArrow != -1) {
            str.Add($"{_weaponFmg[chr.equipArrow]}[{chr.arrowNum}]");
        }
        if (chr.equipSubArrow != -1) {
            str.Add($"{_weaponFmg[chr.equipSubArrow]}[{chr.subArrowNum}]");
        }
        if (chr.equipBolt != -1) {
            str.Add($"{_weaponFmg[chr.equipBolt]}[{chr.boltNum}]");
        }
        if (chr.equipSubBolt != -1) {
            str.Add($"{_weaponFmg[chr.equipSubBolt]}[{chr.subBoltNum}]");
        }
        if (chr.equipSpell01 != -1) {
            str.Add($"{_goodsFmg[chr.equipSpell01]}");
        }
        if (chr.equipSpell02 != -1) {
            str.Add($"{_goodsFmg[chr.equipSpell02]}");
        }

        _lineHelpFmg[id] = string.Join(", ", str); //Util.SplitCharacterText(true, str);
    }
    private void setBndFile(BND4 files, string fileName, byte[] bytes) {
        foreach (BinderFile file in files.Files) {
            if (file.Name.EndsWith(fileName)) {
                file.Bytes = bytes;
            }
        }
    }
}
