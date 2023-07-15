using ERBingoRandomizer.FileHandler;
using ERBingoRandomizer.Params;
using ERBingoRandomizer.Resources.MSB;
using ERBingoRandomizer.Utility;
using FSParam;
using SoulsFormats;
using System;
using System.Collections.Generic;
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
    private FMG _weaponFmg;
    private FMG _protectorFmg;
    private FMG _goodsFmg;
    private FMG _accessoryFmg;
    // MSBs
    private List<MSBE> _msbs;

    private List<PARAMDEF> _paramDefs;
    // Params
    private Param _equipParamWeapon;
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
        randomizeCharaInitParam();
        _cancellationToken.ThrowIfCancellationRequested();
        writeFiles();
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
    private void randomizeCharaInitParam() {
        for (int i = 0; i < 10; i++) {
            Row? row = _charaInitParam[i + 3000];
            if (row != null) {
                CharaInitParam param = new(row);
                randomizeCharaInitEntry(param);
                addDescriptionString(param, ChrInfoMapping[i]);
            }
        }
    }
    private void randomizeCharaInitEntry(CharaInitParam chr) {
        chr.wepleft = chanceGetRandomWeapon(chr.wepleft);
        chr.wepRight = getRandomWeapon(chr.wepRight);
        chr.subWepLeft = chanceGetRandomWeapon(chr.subWepLeft);
        chr.subWepRight = chanceGetRandomWeapon(chr.subWepRight);
        chr.subWepLeft3 = chanceGetRandomWeapon(chr.subWepLeft3);
        chr.subWepRight3 = chanceGetRandomWeapon(chr.subWepRight3);

        chr.equipHelm = chanceGetRandomHelm(chr.equipHelm);
        chr.equipArmer = chanceGetRandomBody(chr.equipArmer);
        chr.equipGaunt = chanceGetRandomArms(chr.equipGaunt);
        chr.equipLeg = chanceGetRandomLegs(chr.equipLeg);

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

    private Task init() {
        _bhd5Reader = new BHD5Reader(_path, false, _cancellationToken);
        _cancellationToken.ThrowIfCancellationRequested();
        _oodlePtr = Kernel32.LoadLibrary($"{_path}/oo2core_6_win64.dll");
        _cancellationToken.ThrowIfCancellationRequested();
        getDefs();
        _cancellationToken.ThrowIfCancellationRequested();
        getParams();
        _cancellationToken.ThrowIfCancellationRequested();
        getFmgs();
        _cancellationToken.ThrowIfCancellationRequested();
        buildDictionaries();
        _cancellationToken.ThrowIfCancellationRequested();
        // buildMsbList();
        // _cancellationToken.ThrowIfCancellationRequested();
        Kernel32.FreeLibrary(_oodlePtr);
        return Task.CompletedTask;
    }
    private void buildMsbList() {
        _msbs = new List<MSBE>();
        foreach (string path in MsbFiles.Files) {
            MSBE msb = MSBE.Read(_bhd5Reader.GetFile(path));
            addRefs(msb);
            _msbs.Add(msb);
        }
    }
    private void addRefs(MSBE msb) {
        foreach (MSBE.Event.Treasure? treasure in msb.Events.Treasures) {
            if (treasure == null) {
                continue;
            }
            _itemLotParam_mapRefs.Add(treasure.ItemLotID);
        }
    }
    private void getDefs() {
        _paramDefs = new List<PARAMDEF>();
        string[] defs = Util.GetEmbeddedFolder("Resources.Params.Defs");
        foreach (string def in defs) {
            _paramDefs.Add(Util.XmlDeserialize(def));
        }
    }
    private void getParams() {
        _regulationBnd = SFUtil.DecryptERRegulation(_regulationPath);
        foreach (BinderFile file in _regulationBnd.Files) {
            getParams(file);
        }
    }
    private void getFmgs() {
        byte[] itemMsgbndBytes = getOrOpenFile(ItemMsgBNDPath);
        if (itemMsgbndBytes == null) {
            throw new InvalidFileException(ItemMsgBNDPath);
        }
        BND4 itemBnd = BND4.Read(itemMsgbndBytes);
        foreach (BinderFile file in itemBnd.Files) {
            getFmgs(file);
        }

        _cancellationToken.ThrowIfCancellationRequested();

        byte[] menuMsgbndBytes = getOrOpenFile(MenuMsgBNDPath);
        if (itemMsgbndBytes == null) {
            throw new InvalidFileException(MenuMsgBNDPath);
        }
        _menuMsgBND = BND4.Read(menuMsgbndBytes);
        foreach (BinderFile file in _menuMsgBND.Files) {
            getFmgs(file);
        }
    }
    private byte[] getOrOpenFile(string path) {
        if (!File.Exists($"{CachePath}/{path}")) {
            byte[]? file = _bhd5Reader.GetFile(path);
            Directory.CreateDirectory(Path.GetDirectoryName($"{CachePath}/{path}"));
            File.WriteAllBytes($"{CachePath}/{path}", file);
            return file;
        }

        return File.ReadAllBytes($"{CachePath}/{path}");
    }
    private void buildDictionaries() {
        _weaponDictionary = new Dictionary<int, EquipParamWeapon>();
        _weaponTypeDictionary = new Dictionary<ushort, List<Row>>();

        foreach (Row row in _equipParamWeapon.Rows) {
            string rowString = _weaponFmg[row.ID];
            if ((int)row["sortId"].Value.Value == 9999999 || row.ID == 17030000 || string.IsNullOrWhiteSpace(rowString) || rowString.ToLower().Contains("error")) {
                continue;
            }

            EquipParamWeapon wep = new(row);
            if (!(wep.wepType >= 81 && wep.wepType <= 86)) {
                _weaponDictionary.Add(row.ID, new EquipParamWeapon(row));
            }

            List<Row>? rows;
            if (_weaponTypeDictionary.TryGetValue(wep.wepType, out rows)) {
                rows.Add(row);
            }
            else {
                rows = new List<Row>();
                rows.Add(row);
                _weaponTypeDictionary.Add(wep.wepType, rows);
            }
        }

        _armorDictionary = new Dictionary<int, Row>();
        _armorTypeDictionary = new Dictionary<byte, List<Row>>();
        foreach (Row row in _equipParamProtector.Rows) {
            int sortId = (int)row["sortId"].Value.Value;
            string rowString = _protectorFmg[row.ID];
            if (sortId == 9999999 || sortId == 99999 || string.IsNullOrWhiteSpace(rowString) || rowString.ToLower().Contains("error")) {
                continue;
            }

            _armorDictionary.Add(row.ID, row);
            byte protectorCategory = (byte)row["protectorCategory"].Value.Value;
            List<Row>? rows;
            if (_armorTypeDictionary.TryGetValue(protectorCategory, out rows)) {
                rows.Add(row);
            }
            else {
                rows = new List<Row> {
                    row,
                };
                _armorTypeDictionary.Add(protectorCategory, rows);
            }
        }

        _goodsDictionary = new Dictionary<int, Row>();
        foreach (Row row in _equipParamGoods.Rows) {
            int sortId = (int)row["sortId"].Value.Value;
            string rowString = _goodsFmg[row.ID];
            if (sortId == 9999999 || sortId == 0 || string.IsNullOrWhiteSpace(rowString) || rowString.ToLower().Contains("error")) {
                continue;
            }

            _goodsDictionary.Add(row.ID, row);
        }

        _magicDictionary = new Dictionary<int, Magic>();
        _magicTypeDictionary = new Dictionary<byte, List<Row>>();
        foreach (Row row in _magicParam.Rows) {
            if (!_goodsDictionary.ContainsKey(row.ID)) {
                continue;
            }
            Magic magic = new(row);
            _magicDictionary.Add(row.ID, magic);
            List<Row>? rows;
            if (_magicTypeDictionary.TryGetValue(magic.ezStateBehaviorType, out rows)) {
                rows.Add(row);
            }
            else {
                rows = new List<Row>();
                rows.Add(row);
                _magicTypeDictionary.Add(magic.ezStateBehaviorType, rows);
            }
        }

        _accessoryDictionary = new Dictionary<int, Row>();
        foreach (Row row in _equipParamAccessory.Rows) {
            int sortId = (int)row["sortId"].Value.Value;
            string rowString = _accessoryFmg[row.ID];
            if (sortId == 9999999 || string.IsNullOrWhiteSpace(rowString) || rowString.ToLower().Contains("error")) {
                continue;
            }

            _accessoryDictionary.Add(row.ID, row);
        }
    }
    private void getParams(BinderFile file) {
        string fileName = Path.GetFileName(file.Name);
        switch (fileName) {
            case EquipParamWeaponName:
                _equipParamWeapon = Param.Read(file.Bytes);
                if (!_equipParamWeapon.ApplyParamDefsCarefully(_paramDefs)) {
                    throw new NoParamDefException(_equipParamWeapon.ParamType);
                }
                break;
            case EquipParamGoodsName:
                _equipParamGoods = Param.Read(file.Bytes);
                if (!_equipParamGoods.ApplyParamDefsCarefully(_paramDefs)) {
                    throw new NoParamDefException(_equipParamGoods.ParamType);
                }
                break;
            case EquipParamAccessoryName:
                _equipParamAccessory = Param.Read(file.Bytes);
                if (!_equipParamAccessory.ApplyParamDefsCarefully(_paramDefs)) {
                    throw new NoParamDefException(_equipParamAccessory.ParamType);
                }
                break;
            case EquipParamProtectorName:
                _equipParamProtector = Param.Read(file.Bytes);
                if (!_equipParamProtector.ApplyParamDefsCarefully(_paramDefs)) {
                    throw new NoParamDefException(_equipParamProtector.ParamType);
                }
                break;
            case CharaInitParamName:
                _charaInitParam = Param.Read(file.Bytes);
                if (!_charaInitParam.ApplyParamDefsCarefully(_paramDefs)) {
                    throw new NoParamDefException(_charaInitParam.ParamType);
                }
                break;
            case MagicName:
                _magicParam = Param.Read(file.Bytes);
                if (!_magicParam.ApplyParamDefsCarefully(_paramDefs)) {
                    throw new NoParamDefException(_magicParam.ParamType);
                }
                break;
            case ItemLotParam_mapName:
                _itemLotParam_map = Param.Read(file.Bytes);
                if (!_itemLotParam_map.ApplyParamDefsCarefully(_paramDefs)) {
                    throw new NoParamDefException(_itemLotParam_map.ParamType);
                }
                break;
            case ItemLotParam_enemyName:
                _itemLotParam_enemy = Param.Read(file.Bytes);
                if (!_itemLotParam_enemy.ApplyParamDefsCarefully(_paramDefs)) {
                    throw new NoParamDefException(_itemLotParam_enemy.ParamType);
                }
                break;
            case ShopLineupParamName:
                _shopLineupParam = Param.Read(file.Bytes);
                if (!_shopLineupParam.ApplyParamDefsCarefully(_paramDefs)) {
                    throw new NoParamDefException(_shopLineupParam.ParamType);
                }
                break;
        }
    }
    private void getFmgs(BinderFile file) {
        string fileName = Path.GetFileName(file.Name);
        switch (fileName) {
            case WeaponNameName:
                _weaponFmg = FMG.Read(file.Bytes);
                break;
            case ProtectorNameName:
                _protectorFmg = FMG.Read(file.Bytes);
                break;
            case GoodsNameName:
                _goodsFmg = FMG.Read(file.Bytes);
                break;
            case AccessoryNameName:
                _accessoryFmg = FMG.Read(file.Bytes);
                break;
            case GR_LineHelpName:
                _lineHelpFmg = FMG.Read(file.Bytes);
                break;
        }
    }
    private void setBndFile(BND4 files, string fileName, byte[] bytes) {
        foreach (BinderFile file in files.Files) {
            if (file.Name.EndsWith(fileName)) {
                file.Bytes = bytes;
            }
        }
    }
}
