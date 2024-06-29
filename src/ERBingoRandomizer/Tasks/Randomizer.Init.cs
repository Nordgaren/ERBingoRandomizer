using Project.FileHandler;
using Project.Params;
using Project.Settings;
using Project.Utility;
using FSParam;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CS8618

namespace Project.Tasks;

public partial class Randomizer
{
    //static async method that behaves like a constructor    
    public static async Task<Randomizer> BuildRandomizerAsync(string path, string seed, CancellationToken cancellationToken)
    {
        Randomizer rando = new(path, seed, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        await Task.Run(() => rando.init());
        return rando;
    }
    // Cancellation Token
    private readonly CancellationToken _cancellationToken;
    private Randomizer(string path, string seed, CancellationToken cancellationToken)
    {
        _path = Path.GetDirectoryName(path) ?? throw new InvalidOperationException("Path.GetDirectoryName(path) was null. Incorrect path provided.");
        _regulationPath = $"{_path}/{Const.RegulationName}";
        _seed = string.IsNullOrWhiteSpace(seed) ? createSeed() : seed.Trim();
        byte[] hashData = SHA256.HashData(Encoding.UTF8.GetBytes(_seed));
        _seedInt = getSeedFromHashData(hashData);
        _random = new Random(_seedInt);
        _cancellationToken = cancellationToken;
    }
    private Task init()
    {
        if (!allCacheFilesExist())
        {
            _bhd5Reader = new BHD5Reader(_path, Config.CacheBHDs, _cancellationToken);
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
        Kernel32.FreeLibrary(_oodlePtr);
        _oodlePtr = IntPtr.Zero;
        return Task.CompletedTask;
    }
    private static bool allCacheFilesExist()
    {
        return File.Exists(Const.ItemMsgBNDPath) && File.Exists(Const.MenuMsgBNDPath);
    }
    private void getDefs()
    {
        _paramDefs = new List<PARAMDEF>();
        string[] defs = Util.GetResourcesInFolder("Params/Defs");
        foreach (string def in defs)
        {
            _paramDefs.Add(Util.XmlDeserialize(def));
        }
    }
    private void getParams()
    {
        _regulationBnd = SFUtil.DecryptERRegulation(_regulationPath);
        foreach (BinderFile file in _regulationBnd.Files)
        {
            getParams(file);
        }
    }
    private void getFmgs()
    {
        byte[] itemMsgBndBytes = getOrOpenFile(Const.ItemMsgBNDPath);
        if (itemMsgBndBytes == null)
        {
            throw new InvalidFileException(Const.ItemMsgBNDPath);
        }
        BND4 itemBnd = BND4.Read(itemMsgBndBytes);
        foreach (BinderFile file in itemBnd.Files)
        {
            getFmgs(file);
        }

        _cancellationToken.ThrowIfCancellationRequested();

        byte[] menuMsgBndBytes = getOrOpenFile(Const.MenuMsgBNDPath);
        if (itemMsgBndBytes == null)
        {
            throw new InvalidFileException(Const.MenuMsgBNDPath);
        }
        _menuMsgBND = BND4.Read(menuMsgBndBytes);
        foreach (BinderFile file in _menuMsgBND.Files)
        {
            getFmgs(file);
        }
    }
    private byte[] getOrOpenFile(string path)
    {
        if (File.Exists($"{Config.CachePath}/{path}"))
        {
            return File.ReadAllBytes($"{Config.CachePath}/{path}");
        }

        byte[] file = _bhd5Reader.GetFile(path) ?? throw new InvalidOperationException($"Could not find file {Config.CachePath}/{path}");
        Directory.CreateDirectory(Path.GetDirectoryName($"{Config.CachePath}/{path}") ?? throw new InvalidOperationException($"Could not get directory name for file {Config.CachePath}/{path}"));
        File.WriteAllBytes($"{Config.CachePath}/{path}", file);
        return file;
    }
    private void buildDictionaries()
    {
        _weaponDictionary = new Dictionary<int, EquipParamWeapon>(); // TODO update for DLC weapons
        _weaponTypeDictionary = new Dictionary<ushort, List<Param.Row>>(); // TODO update for DLC weapons
        _weaponNameDictionary = new Dictionary<int, string>(); // TODO update for DLC weapons

        foreach (Param.Row row in _equipParamWeapon.Rows)
        {
            string rowString = _weaponFmg[row.ID];
            if ((int)row["sortId"]!.Value.Value == 9999999 || string.IsNullOrWhiteSpace(rowString) || rowString.ToLower().Contains("[error]"))
            {
                continue;
            }
            EquipParamWeapon wep = new(row);
            if (row.ID == 17030000)
            {
                wep.materialSetId = 0;
                wep.reinforceTypeId = 3000;
                wep.reinforceShopCategory = 0;
                continue;
            }

            _weaponNameDictionary[row.ID] = rowString;
            if (!Enumerable.Range(81, 86).Contains(wep.wepType))
            {
                _weaponDictionary.Add(row.ID, new EquipParamWeapon(row)); // TODO visit
            }

            if (_weaponTypeDictionary.TryGetValue(wep.wepType, out List<Param.Row>? rows))
            {
                rows.Add(row);
            }
            else
            {
                rows = new List<Param.Row> {
                    row,
                };
                _weaponTypeDictionary.Add(wep.wepType, rows);
            }
        }

        _customWeaponDictionary = new Dictionary<int, EquipParamWeapon>();

        foreach (Param.Row row in _equipParamCustomWeapon.Rows)
        { // TODO visit why is this necessary
            if (!_weaponDictionary.TryGetValue((int)row["baseWepId"]!.Value.Value, out EquipParamWeapon? wep))
            {
                continue;
            }
            EquipParamCustomWeapon customWep = new(row);
            _weaponNameDictionary[row.ID] = $"{_weaponNameDictionary[customWep.baseWepId]} +{customWep.reinforceLv}";
            _customWeaponDictionary.Add(row.ID, wep);
        }

        _armorTypeDictionary = new Dictionary<byte, List<Param.Row>>();
        foreach (Param.Row row in _equipParamProtector.Rows)
        {
            int sortId = (int)row["sortId"]!.Value.Value;
            string rowString = _protectorFmg[row.ID];
            if (sortId == 9999999 || sortId == 99999 || string.IsNullOrWhiteSpace(rowString) || rowString.ToLower().Contains("[error]"))
            {
                continue;
            }

            byte protectorCategory = (byte)row["protectorCategory"]!.Value.Value;
            if (_armorTypeDictionary.TryGetValue(protectorCategory, out List<Param.Row>? rows))
            {
                rows.Add(row);
            }
            else
            {
                rows = new List<Param.Row> {
                    row,
                };
                _armorTypeDictionary.Add(protectorCategory, rows);
            }
        }

        _goodsDictionary = new Dictionary<int, EquipParamGoods>();
        foreach (Param.Row row in _equipParamGoods.Rows)
        {
            int sortId = (int)row["sortId"]!.Value.Value;
            string rowString = _goodsFmg[row.ID];
            if (sortId == 9999999 || sortId == 0 || string.IsNullOrWhiteSpace(rowString) || rowString.ToLower().Contains("[error]"))
            {
                continue;
            }

            EquipParamGoods good = new(row);
            _goodsDictionary.Add(row.ID, good);
        }

        _magicDictionary = new Dictionary<int, Magic>();
        _magicTypeDictionary = new Dictionary<byte, List<Param.Row>>();
        foreach (Param.Row row in _goodsParam.Rows)
        {
            string rowString = _goodsFmg[row.ID];
            if (!_goodsDictionary.TryGetValue(row.ID, out EquipParamGoods? good) || string.IsNullOrWhiteSpace(rowString) || rowString.ToLower().Contains("[error]"))
            {
                continue;
            }

            if (!isSpellGoods(good))
            {
                continue;
            }

            Magic magic = new(row);
            _magicDictionary.Add(row.ID, magic);
            if (_magicTypeDictionary.TryGetValue(magic.ezStateBehaviorType, out List<Param.Row>? rows))
            {
                rows.Add(row);
            }
            else
            {
                rows = new List<Param.Row> {
                    row,
                };
                _magicTypeDictionary.Add(magic.ezStateBehaviorType, rows);
            }
        }
    }
    private static bool isSpellGoods(EquipParamGoods good)
    {
        switch (good.goodsType)
        {
            case Const.GoodsSorceryType:
            case Const.GoodsIncantationType:
            case Const.GoodsSelfSorceryType:
            case Const.GoodsSelfIncantationType:
                return true;
        }

        return false;
    }
    private void getParams(BinderFile file)
    {
        string fileName = Path.GetFileName(file.Name);
        switch (fileName)
        {
            case Const.EquipParamWeaponName:
                {
                    _equipParamWeapon = Param.Read(file.Bytes); // TODO visit
                    if (!_equipParamWeapon.ApplyParamDefsCarefully(_paramDefs))
                    {
                        throw new InvalidParamDefException(_equipParamWeapon.ParamType);
                    }
                    break;
                }
            case Const.EquipParamCustomWeaponName:
                {
                    _equipParamCustomWeapon = Param.Read(file.Bytes);
                    if (!_equipParamCustomWeapon.ApplyParamDefsCarefully(_paramDefs))
                    {
                        throw new InvalidParamDefException(_equipParamCustomWeapon.ParamType);
                    }
                    break;
                }
            case Const.EquipParamGoodsName:
                {
                    _equipParamGoods = Param.Read(file.Bytes);
                    if (!_equipParamGoods.ApplyParamDefsCarefully(_paramDefs))
                    {
                        throw new InvalidParamDefException(_equipParamGoods.ParamType);
                    }
                    break;
                }
            case Const.EquipParamProtectorName:
                {
                    _equipParamProtector = Param.Read(file.Bytes);
                    if (!_equipParamProtector.ApplyParamDefsCarefully(_paramDefs))
                    {
                        throw new InvalidParamDefException(_equipParamProtector.ParamType);
                    }
                    break;
                }
            case Const.CharaInitParamName:
                {
                    _charaInitParam = Param.Read(file.Bytes);
                    if (!_charaInitParam.ApplyParamDefsCarefully(_paramDefs))
                    {
                        throw new InvalidParamDefException(_charaInitParam.ParamType);
                    }
                    break;
                }
            case Const.MagicName:
                {
                    _goodsParam = Param.Read(file.Bytes);
                    if (!_goodsParam.ApplyParamDefsCarefully(_paramDefs))
                    {
                        throw new InvalidParamDefException(_goodsParam.ParamType);
                    }
                    break;
                }
            case Const.ItemLotParam_mapName:
                {
                    _itemLotParam_map = Param.Read(file.Bytes);
                    if (!_itemLotParam_map.ApplyParamDefsCarefully(_paramDefs))
                    {
                        throw new InvalidParamDefException(_itemLotParam_map.ParamType);
                    }
                    break;
                }
            case Const.ItemLotParam_enemyName:
                {
                    _itemLotParam_enemy = Param.Read(file.Bytes);
                    if (!_itemLotParam_enemy.ApplyParamDefsCarefully(_paramDefs))
                    {
                        throw new InvalidParamDefException(_itemLotParam_enemy.ParamType);
                    }
                    break;
                }
            case Const.ShopLineupParamName:
                {
                    _shopLineupParam = Param.Read(file.Bytes);
                    if (!_shopLineupParam.ApplyParamDefsCarefully(_paramDefs))
                    {
                        throw new InvalidParamDefException(_shopLineupParam.ParamType);
                    }
                    break;
                }
            case Const.AtkParamPcName:
                {
                    _atkParam_Pc = Param.Read(file.Bytes);
                    if (!_atkParam_Pc.ApplyParamDefsCarefully(_paramDefs))
                    {
                        throw new InvalidParamDefException(_atkParam_Pc.ParamType);
                    }
                    break;
                }
        }
    }
    private void getFmgs(BinderFile file)
    {
        string fileName = Path.GetFileName(file.Name);
        switch (fileName)
        {
            case Const.WeaponNameName:
                _weaponFmg = FMG.Read(file.Bytes);
                break;
            case Const.ProtectorNameName:
                _protectorFmg = FMG.Read(file.Bytes);
                break;
            case Const.GoodsNameName:
                _goodsFmg = FMG.Read(file.Bytes);
                break;
            case Const.GR_LineHelpName:
                _lineHelpFmg = FMG.Read(file.Bytes);
                break;
            case Const.GR_MenuTextName:
                _menuTextFmg = FMG.Read(file.Bytes);
                break;
        }
    }
}
