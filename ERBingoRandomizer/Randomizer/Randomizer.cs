using ERBingoRandomizer;
using ERBingoRandomizer.BHDReader;
using ERBingoRandomizer.Resources.MSB;
using FSParam;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Serialization;
using static ERBingoRandomizer.Const;
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
    private FMG _lineHelpFmg;
    private FMG _weaponFmg;
    private FMG _protectorFmg;
    private FMG _goodsFmg;
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
    private Dictionary<int, Row> _weaponDictionary;
    private Dictionary<int, Row> _armorDictionary;
    private Dictionary<int, Row> _goodsDictionary;
    private Dictionary<int, Row> _magicDictionary;
    private Dictionary<int, Row> _accessoryDictionary;
    private Dictionary<ushort, List<Row>> _weaponTypeDictionary;
    private Dictionary<byte, List<Row>> _armorTypeDictionary;
    private Dictionary<int, List<Row>> _magicTypeDictionary;

    // HashSets
    private HashSet<int> _itemLotParam_mapRefs;
    private HashSet<int> _itemLotParam_enemyRefs;
    //static async method that behave like a constructor       
    public static async Task<BingoRandomizer> BuildRandomizerAsync(string path, string seed) {
        BingoRandomizer rando = new(path, seed);
        await Task.Run(() => rando.init());
        return rando;
    }
    private BingoRandomizer(string path, string seed) {
        _path = Path.GetDirectoryName(path) ?? throw new InvalidOperationException("Path.GetDirectoryName(path) was null. Incorrect path provided.");
        _regulationPath = _path + RegulationName;
        _seed = string.IsNullOrWhiteSpace(seed) ? Random.Shared.NextInt64().ToString() : seed.Trim();
        byte[] hashData = SHA256.HashData(Encoding.UTF8.GetBytes(_seed));
        _seedInt = getSeedFromHashData(hashData);
        _random = new Random(_seedInt);
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
        
        byte[] bnd = _regulationBnd.Write();
        SeedInfo = new SeedInfo(_seed, BitConverter.ToString(SHA256.HashData(bnd)).Replace("-", ""));
        return Task.CompletedTask;
    }
    static readonly int[] ChrInfoMapping = {
        297130, 297131, 297132, 297133, 297134, 297135, 297138, 297136, 297137, 297139,
    };
    private void randomizeCharaInitParam() {
        for (int i = 0; i < 10; i++) {
            Row? row = _charaInitParam[i + 3000];
            if (row != null) {
                randomizeCharaInitEntry(row);
                //addDescriptionString(row, ChrInfoMapping[i]);
            }
            
        }
    }
    private void randomizeCharaInitEntry(Row chr) {
        Cell wepLeft = chr["equip_Wep_Left"].Value;
        wepLeft.Value = chanceGetRandomWeapon((int)wepLeft.Value);
        Cell wepRight = chr["equip_Wep_Right"].Value;
        wepRight.Value = getRandomWeapon((int)wepRight.Value);
        Cell subWepLeft = chr["equip_Subwep_Left"].Value;
        subWepLeft.Value = chanceGetRandomWeapon((int)subWepLeft.Value);
        Cell subWepRight = chr["equip_Subwep_Right"].Value;
        subWepRight.Value = chanceGetRandomWeapon((int)subWepRight.Value);
        Cell subWepLeft3 = chr["equip_Subwep_Left3"].Value;
        subWepLeft3.Value = chanceGetRandomWeapon((int)subWepLeft3.Value);
        Cell subWepRight3 = chr["equip_Subwep_Right3"].Value;
        subWepRight3.Value = chanceGetRandomWeapon((int)subWepRight3.Value);
        
        Cell equipHelm = chr["equip_Helm"].Value;
        equipHelm.Value = chanceGetRandomHelm((int)equipHelm.Value);
        Cell equipArmer = chr["equip_Armer"].Value;
        equipArmer.Value = chanceGetRandomBody((int)equipArmer.Value);
        Cell equipGaunt = chr["equip_Gaunt"].Value;
        equipGaunt.Value = chanceGetRandomArms((int)equipGaunt.Value);
        Cell equipLeg = chr["equip_Leg"].Value;
        equipLeg.Value = chanceGetRandomLegs((int)equipLeg.Value);

        randomizeLevels(chr);
    }
    private void addDescriptionString(Row row, int i) {
        throw new NotImplementedException();
    }

    private Task init() {
        _bhd5Reader = new BHD5Reader(_path);
        _oodlePtr = Kernel32.LoadLibrary($"{_path}/oo2core_6_win64.dll");
        getDefs();
        getParams();
        getFmgs();
        buildDictionaries();
        buildMsbList();
        Kernel32.FreeLibrary(_oodlePtr);
        return Task.CompletedTask;
    }
    private void buildMsbList() {
        _msbs = new();
        foreach (string path in Files.Paths) {
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
    void getDefs() {
        _paramDefs = new();
        string[] defs = Util.GetEmbeddedFolder("Resources.Params.Defs");
        foreach (string def in defs) {
            _paramDefs.Add(Util.XmlDeserialize(def));
        }
    }
    void getParams() {
        _regulationBnd = SFUtil.DecryptERRegulation(_regulationPath);
        foreach (BinderFile file in _regulationBnd.Files) {
            setParams(file);
        }
    }
    void getFmgs() {
        byte[]? itemMsgbndBytes = _bhd5Reader.GetFile(ItemmsgbndName);
        if (itemMsgbndBytes == null) {
            throw new InvalidFileException(ItemmsgbndName);
        }
        BND4 itemBnd = BND4.Read(itemMsgbndBytes);
        foreach (BinderFile file in itemBnd.Files) {
            setFmgs(file);
        }

        byte[]? menuMsgbndBytes = _bhd5Reader.GetFile(MenumsgbndName);
        if (itemMsgbndBytes == null) {
            throw new InvalidFileException(MenumsgbndName);
        }
        BND4 menuBnd = BND4.Read(menuMsgbndBytes);
        foreach (BinderFile file in menuBnd.Files) {
            setFmgs(file);
        }
    }
    void buildDictionaries() {
        _weaponDictionary = new();
        _weaponTypeDictionary = new();

        foreach (Row row in _equipParamWeapon.Rows) {
            string rowString = _weaponFmg[row.ID];
            if ((int)row["sortId"].Value.Value == 9999999 || row.ID == 17030000 || string.IsNullOrWhiteSpace(rowString) || rowString.ToLower().Contains("error")) {
                continue;
            }

            ushort weaponType = (ushort)row["wepType"].Value.Value;
            if (!(weaponType >= 81 && weaponType <= 86)) {
                _weaponDictionary.Add(row.ID, row);
            }

            List<Row>? rows;
            if (_weaponTypeDictionary.TryGetValue(weaponType, out rows)) {
                rows.Add(row);
            }
            else {
                rows = new();
                rows.Add(row);
                _weaponTypeDictionary.Add(weaponType, rows);
            }
        }

        _armorDictionary = new();
        _armorTypeDictionary = new();
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
                rows = new();
                rows.Add(row);
                _armorTypeDictionary.Add(protectorCategory, rows);
            }
        }

        _goodsDictionary = new();
        foreach (Row row in _equipParamGoods.Rows) {
            int sortId = (int)row["sortId"].Value.Value;
            string rowString = _protectorFmg[row.ID];
            if (sortId == 9999999 || sortId == 0 || string.IsNullOrWhiteSpace(rowString) || rowString.ToLower().Contains("error")) {
                continue;
            }

            _goodsDictionary.Add(row.ID, row);
        }

        _magicDictionary = new();
        _magicTypeDictionary = new();
        foreach (Row row in _magicParam.Rows) {
            if (!_goodsDictionary.ContainsKey(row.ID)) {
                continue;
            }
            _magicDictionary.Add(row.ID, row);
            byte ezStateBehaviorType = (byte)row["ezStateBehaviorType"].Value.Value;
            List<Row>? rows;
            if (_magicTypeDictionary.TryGetValue(ezStateBehaviorType, out rows)) {
                rows.Add(row);
            }
            else {
                rows = new();
                rows.Add(row);
                _magicTypeDictionary.Add(ezStateBehaviorType, rows);
            }
        }

        _accessoryDictionary = new();
        foreach (Row row in _equipParamAccessory.Rows) {
            int sortId = (int)row["sortId"].Value.Value;
            string rowString = _protectorFmg[row.ID];
            if (sortId == 9999999 || string.IsNullOrWhiteSpace(rowString) || rowString.ToLower().Contains("error")) {
                continue;
            }

            _accessoryDictionary.Add(row.ID, row);
        }
    }
    void setParams(BinderFile file) {
        string fileName = Path.GetFileName(file.Name);
        switch (fileName) {
            case "EquipParamWeapon.param":
                _equipParamWeapon = Param.Read(file.Bytes);
                if (!_equipParamWeapon.ApplyParamDefsCarefully(_paramDefs)) {
                    throw new NoParamDefException(_equipParamWeapon.ParamType);
                }
                break;
            case "EquipParamGoods.param":
                _equipParamGoods = Param.Read(file.Bytes);
                if (!_equipParamGoods.ApplyParamDefsCarefully(_paramDefs)) {
                    throw new NoParamDefException(_equipParamGoods.ParamType);
                }
                break;
            case "EquipParamAccessory.param":
                _equipParamAccessory = Param.Read(file.Bytes);
                if (!_equipParamAccessory.ApplyParamDefsCarefully(_paramDefs)) {
                    throw new NoParamDefException(_equipParamAccessory.ParamType);
                }
                break;
            case "EquipParamProtector.param":
                _equipParamProtector = Param.Read(file.Bytes);
                if (!_equipParamProtector.ApplyParamDefsCarefully(_paramDefs)) {
                    throw new NoParamDefException(_equipParamProtector.ParamType);
                }
                break;
            case "CharaInitParam.param":
                _charaInitParam = Param.Read(file.Bytes);
                if (!_charaInitParam.ApplyParamDefsCarefully(_paramDefs)) {
                    throw new NoParamDefException(_charaInitParam.ParamType);
                }
                break;
            case "Magic.param":
                _magicParam = Param.Read(file.Bytes);
                if (!_magicParam.ApplyParamDefsCarefully(_paramDefs)) {
                    throw new NoParamDefException(_magicParam.ParamType);
                }
                break;
            case "ItemLotParam_map.param":
                _itemLotParam_map = Param.Read(file.Bytes);
                if (!_itemLotParam_map.ApplyParamDefsCarefully(_paramDefs)) {
                    throw new NoParamDefException(_itemLotParam_map.ParamType);
                }
                break;
            case "ItemLotParam_enemy.param":
                _itemLotParam_enemy = Param.Read(file.Bytes);
                if (!_itemLotParam_enemy.ApplyParamDefsCarefully(_paramDefs)) {
                    throw new NoParamDefException(_itemLotParam_enemy.ParamType);
                }
                break;
            case "ShopLineupParam.param":
                _shopLineupParam = Param.Read(file.Bytes);
                if (!_shopLineupParam.ApplyParamDefsCarefully(_paramDefs)) {
                    throw new NoParamDefException(_shopLineupParam.ParamType);
                }
                break;
        }
    }
    void setFmgs(BinderFile file) {
        string fileName = Path.GetFileName(file.Name);
        switch (fileName) {
            case "WeaponName.fmg":
                _weaponFmg = FMG.Read(file.Bytes);
                break;
            case "ProtectorName.fmg":
                _protectorFmg = FMG.Read(file.Bytes);
                break;
            case "GoodsName.fmg":
                _goodsFmg = FMG.Read(file.Bytes);
                break;
            case "GR_LineHelp.fmg":
                _lineHelpFmg = FMG.Read(file.Bytes);
                break;
        }
    }
}
