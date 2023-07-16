using ERBingoRandomizer.FileHandler;
using ERBingoRandomizer.Params;
using ERBingoRandomizer.Resources.MSB;
using ERBingoRandomizer.Utility;
using FSParam;
using SoulsFormats;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static ERBingoRandomizer.Utility.Const;
using static ERBingoRandomizer.Utility.Config;
using static FSParam.Param;

namespace ERBingoRandomizer.Randomizer;

public partial class BingoRandomizer {
    private Task init() {
        if (!allCacheFilesExist()) {
            _bhd5Reader = new BHD5Reader(_path, CacheBHDs, _cancellationToken);
        }
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
    private bool allCacheFilesExist() {
        return File.Exists(ItemMsgBNDPath) && File.Exists(MenuMsgBNDPath);
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
        string[] defs = Util.GetResources("Params/Defs");
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
            if (!Enumerable.Range(81, 86).Contains(wep.wepType)) {
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

        _customWeaponDictionary = new();

        foreach (Row row in _equipParamCustomWeapon.Rows) {
            if (!_weaponDictionary.TryGetValue((int)row["baseWepId"].Value.Value, out EquipParamWeapon wep)) {
                continue;
            }

            _customWeaponDictionary.Add(row.ID, wep);

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
            case EquipParamCustomWeaponName:
                _equipParamCustomWeapon = Param.Read(file.Bytes);
                if (!_equipParamCustomWeapon.ApplyParamDefsCarefully(_paramDefs)) {
                    throw new NoParamDefException(_equipParamCustomWeapon.ParamType);
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
}
