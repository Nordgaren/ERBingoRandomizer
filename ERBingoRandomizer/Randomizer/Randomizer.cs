using ERBingoRandomizer;
using ERBingoRandomizer.BHDReader;
using ERBingoRandomizer.MSB;
using FSParam;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using static ERBingoRandomizer.Const;
using static FSParam.Param;

namespace ERBingoRandomizer.Randomizer;

public class BingoRandomizer {
    private string _path;
    private string _regulationPath;
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
    private Dictionary<int, Row> _weaponTypeDictionary;
    private Dictionary<int, Row> _armorDictionary;
    private Dictionary<int, Row> _armorTypeDictionary;
    private Dictionary<int, Row> _goodsDictionary;
    private Dictionary<int, Row> _accessoryDictionary;
    private Dictionary<int, Row> _magicDictionary;
    private Dictionary<int, Row> _magicTypeDictionary;

    // HashSets
    private HashSet<int> _itemLotParam_mapRefs;
    private HashSet<int> _itemLotParam_enemyRefs;
    //static async method that behave like a constructor       
    public static async Task BuildRandomizerAsync(string path, string seed) {
        BingoRandomizer rando = new(path, seed);
        await Task.Run(() => rando.init());
    }
    private BingoRandomizer(string path, string seed) {
        _path = Path.GetDirectoryName(path) ?? throw new InvalidOperationException("Path.GetDirectoryName(path) was null. Incorrect path provided.");
        _regulationPath = _path + REGULATION_NAME;
        _seed = seed == string.Empty ? Random.Shared.NextInt64().ToString() : seed.Trim();
        _seedInt = BitConverter.ToInt32(SHA256.HashData(Encoding.UTF8.GetBytes(_seed)));
        _random = new Random(_seedInt);
    }
    public void RandomizeRegulation() {
        // foreach (Param.Row row in _equipParamWeapon.Rows) {
        //     
        // }
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
            _msbs.Add(MSBE.Read(_bhd5Reader.GetFile(path)));
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
        BND4 bnd = SFUtil.DecryptERRegulation(_regulationPath);
        foreach (BinderFile file in bnd.Files) {
            setParams(file);
        }
    }
    void getFmgs() {
        byte[]? itemMsgbndBytes = _bhd5Reader.GetFile(ITEMMSGBND_NAME);
        if (itemMsgbndBytes == null) {
            throw new InvalidFileException(ITEMMSGBND_NAME);
        }
        BND4 itemBnd = BND4.Read(itemMsgbndBytes);
        foreach (BinderFile file in itemBnd.Files) {
            setFmgs(file);
        }

        byte[]? menuMsgbndBytes = _bhd5Reader.GetFile(MENUMSGBND_NAME);
        if (itemMsgbndBytes == null) {
            throw new InvalidFileException(MENUMSGBND_NAME);
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
            if ((int)row["sortId"].Value.Value != 9999999 && row.ID != 17030000 && !string.IsNullOrWhiteSpace(_weaponFmg[row.ID])) {
                Debug.WriteLine(row.ID);
            }
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
