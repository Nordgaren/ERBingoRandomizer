using ERBingoRandomizer.FileHandler;
using ERBingoRandomizer.Params;
using ERBingoRandomizer.Utility;
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

namespace ERBingoRandomizer.Randomizer; 

public class RandoResource {
    private readonly CancellationToken _cancellationToken;

    internal readonly Random Random;
    internal readonly string Path;
    internal readonly string RegulationPath;
    internal BND4 RegulationBnd;
    internal readonly string Seed;
    internal int SeedInt;
    internal BHD5Reader Bhd5Reader;
    internal IntPtr PodlePtr;
    // FMGs
    internal BND4 MenuMsgBnd;
    internal FMG LineHelpFmg;
    internal FMG MenuTextFmg;
    internal FMG WeaponFmg;
    internal FMG ProtectorFmg;
    internal FMG GoodsFmg;
    // Params
    internal List<PARAMDEF> ParamDefs;
    internal Param EquipParamWeapon;
    internal Param EquipParamCustomWeapon;
    internal Param EquipParamGoods;
    internal Param EquipParamProtector;
    internal Param CharaInitParam;
    internal Param GoodsParam;
    internal Param ItemLotParamMap;
    internal Param ItemLotParamEnemy;
    internal Param ShopLineupParam;
    internal Param AtkParamPc;
    // Dictionaries
    internal Dictionary<int, EquipParamWeapon> WeaponDictionary;
    internal Dictionary<int, EquipParamWeapon> CustomWeaponDictionary;
    internal Dictionary<int, string> WeaponNameDictionary;
    internal Dictionary<int, EquipParamGoods> GoodsDictionary;
    internal Dictionary<int, Magic> MagicDictionary;
    internal Dictionary<ushort, List<Param.Row>> WeaponTypeDictionary;
    internal Dictionary<byte, List<Param.Row>> ArmorTypeDictionary;
    internal Dictionary<byte, List<Param.Row>> MagicTypeDictionary;

    public RandoResource(string path, string seed, CancellationToken cancellationToken) {
        _cancellationToken = cancellationToken;
        Path = System.IO.Path.GetDirectoryName(path) ?? throw new InvalidOperationException("Path.GetDirectoryName(path) was null. Incorrect path provided.");
        RegulationPath = $"{Path}/{Const.RegulationName}";
        Seed = string.IsNullOrWhiteSpace(seed) ? Random.Shared.NextInt64().ToString() : seed.Trim();
        byte[] hashData = SHA256.HashData(Encoding.UTF8.GetBytes(Seed));
        SeedInt = getSeedFromHashData(hashData);
        Random = new Random(SeedInt);
    }
    
    private static int getSeedFromHashData(IEnumerable<byte> hashData) {
        IEnumerable<byte[]> chunks = hashData.Chunk(4);
        return chunks.Aggregate(0, (current, chunk) => current ^ BitConverter.ToInt32(chunk));
    }

    internal Task Init() {
        if (!allCacheFilesExist()) {
            Bhd5Reader = new BHD5Reader(Path, Config.CacheBHDs, _cancellationToken);
        }
        _cancellationToken.ThrowIfCancellationRequested();
        PodlePtr = Kernel32.LoadLibrary($"{Path}/oo2core_6_win64.dll");
        _cancellationToken.ThrowIfCancellationRequested();
        getDefs();
        _cancellationToken.ThrowIfCancellationRequested();
        getParams();
        _cancellationToken.ThrowIfCancellationRequested();
        getFmgs();
        _cancellationToken.ThrowIfCancellationRequested();
        buildDictionaries();
        _cancellationToken.ThrowIfCancellationRequested();
        Kernel32.FreeLibrary(PodlePtr);
        PodlePtr = IntPtr.Zero;
        return Task.CompletedTask;
    }
    private static bool allCacheFilesExist() {
        return File.Exists(Const.ItemMsgBNDPath) && File.Exists(Const.MenuMsgBNDPath);
    }
    private void getDefs() {
        ParamDefs = new List<PARAMDEF>();
        string[] defs = Util.GetResourcesInFolder("Params/Defs");
        foreach (string def in defs) {
            ParamDefs.Add(Util.XmlDeserialize(def));
        }
    }
    private void getParams() {
        RegulationBnd = SFUtil.DecryptERRegulation(RegulationPath);
        foreach (BinderFile file in RegulationBnd.Files) {
            getParams(file);
        }
    }
    private void getFmgs() {
        byte[] itemMsgBndBytes = getOrOpenFile(Const.ItemMsgBNDPath);
        if (itemMsgBndBytes == null) {
            throw new InvalidFileException(Const.ItemMsgBNDPath);
        }
        BND4 itemBnd = BND4.Read(itemMsgBndBytes);
        foreach (BinderFile file in itemBnd.Files) {
            getFmgs(file);
        }

        _cancellationToken.ThrowIfCancellationRequested();

        byte[] menuMsgBndBytes = getOrOpenFile(Const.MenuMsgBNDPath);
        if (itemMsgBndBytes == null) {
            throw new InvalidFileException(Const.MenuMsgBNDPath);
        }
        MenuMsgBnd = BND4.Read(menuMsgBndBytes);
        foreach (BinderFile file in MenuMsgBnd.Files) {
            getFmgs(file);
        }
    }
    private byte[] getOrOpenFile(string path) {
        if (File.Exists($"{Config.CachePath}/{path}")) {
            return File.ReadAllBytes($"{Config.CachePath}/{path}");
        }

        byte[] file = Bhd5Reader.GetFile(path) ?? throw new InvalidOperationException($"Could not find file {Config.CachePath}/{path}");
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName($"{Config.CachePath}/{path}") ?? throw new InvalidOperationException($"Could not get directory name for file {Config.CachePath}/{path}"));
        File.WriteAllBytes($"{Config.CachePath}/{path}", file);
        return file;
    }
    private void buildDictionaries() {
        WeaponDictionary = new Dictionary<int, EquipParamWeapon>();
        WeaponTypeDictionary = new Dictionary<ushort, List<Param.Row>>();
        WeaponNameDictionary = new Dictionary<int, string>();

        foreach (Param.Row row in EquipParamWeapon.Rows) {
            string rowString = WeaponFmg[row.ID];
            if ((int)row["sortId"]!.Value.Value == 9999999 || string.IsNullOrWhiteSpace(rowString) || rowString.ToLower().Contains("[error]")) {
                continue;
            }
            EquipParamWeapon wep = new(row);
            if (row.ID == 17030000) {
                wep.materialSetId = 0;
                wep.reinforceTypeId = 3000;
                wep.reinforceShopCategory = 0;
                continue;
            }

            WeaponNameDictionary[row.ID] = rowString;
            if (!Enumerable.Range(81, 86).Contains(wep.wepType)) {
                WeaponDictionary.Add(row.ID, new EquipParamWeapon(row));
            }

            if (WeaponTypeDictionary.TryGetValue(wep.wepType, out List<Param.Row>? rows)) {
                rows.Add(row);
            }
            else {
                rows = new List<Param.Row> {
                    row,
                };
                WeaponTypeDictionary.Add(wep.wepType, rows);
            }
        }

        CustomWeaponDictionary = new Dictionary<int, EquipParamWeapon>();

        foreach (Param.Row row in EquipParamCustomWeapon.Rows) {
            if (!WeaponDictionary.TryGetValue((int)row["baseWepId"]!.Value.Value, out EquipParamWeapon? wep)) {
                continue;
            }
            EquipParamCustomWeapon customWep = new(row);
            string levelString = customWep.reinforceLv != 0 ?  $" +{customWep.reinforceLv}" : String.Empty; 
            WeaponNameDictionary[row.ID] = $"{WeaponNameDictionary[customWep.baseWepId]}{levelString}";
            CustomWeaponDictionary.Add(row.ID, wep);
        }

        ArmorTypeDictionary = new Dictionary<byte, List<Param.Row>>();
        foreach (Param.Row row in EquipParamProtector.Rows) {
            int sortId = (int)row["sortId"]!.Value.Value;
            string rowString = ProtectorFmg[row.ID];
            if (sortId == 9999999 || sortId == 99999 || string.IsNullOrWhiteSpace(rowString) || rowString.ToLower().Contains("[error]")) {
                continue;
            }

            byte protectorCategory = (byte)row["protectorCategory"]!.Value.Value;
            if (ArmorTypeDictionary.TryGetValue(protectorCategory, out List<Param.Row>? rows)) {
                rows.Add(row);
            }
            else {
                rows = new List<Param.Row> {
                    row,
                };
                ArmorTypeDictionary.Add(protectorCategory, rows);
            }
        }

        GoodsDictionary = new Dictionary<int, EquipParamGoods>();
        foreach (Param.Row row in EquipParamGoods.Rows) {
            int sortId = (int)row["sortId"]!.Value.Value;
            string rowString = GoodsFmg[row.ID];
            if (sortId == 9999999 || sortId == 0 || string.IsNullOrWhiteSpace(rowString) || rowString.ToLower().Contains("[error]")) {
                continue;
            }

            EquipParamGoods good = new(row);
            GoodsDictionary.Add(row.ID, good);
        }

        MagicDictionary = new Dictionary<int, Magic>();
        MagicTypeDictionary = new Dictionary<byte, List<Param.Row>>();
        foreach (Param.Row row in GoodsParam.Rows) {
            string rowString = GoodsFmg[row.ID];
            if (!GoodsDictionary.TryGetValue(row.ID, out EquipParamGoods? good) || string.IsNullOrWhiteSpace(rowString) || rowString.ToLower().Contains("[error]")) {
                continue;
            }

            if (!isSpellGoods(good)) {
                continue;
            }

            Magic magic = new(row);
            MagicDictionary.Add(row.ID, magic);
            if (MagicTypeDictionary.TryGetValue(magic.ezStateBehaviorType, out List<Param.Row>? rows)) {
                rows.Add(row);
            }
            else {
                rows = new List<Param.Row> {
                    row,
                };
                MagicTypeDictionary.Add(magic.ezStateBehaviorType, rows);
            }
        }
    }
    private static bool isSpellGoods(EquipParamGoods good) {
        switch (good.goodsType) {
            case Const.GoodsSorceryType:
            case Const.GoodsIncantationType:
            case Const.GoodsSelfSorceryType:
            case Const.GoodsSelfIncantationType:
                return true;
        }

        return false;
    }
    private void getParams(BinderFile file) {
        string fileName = System.IO.Path.GetFileName(file.Name);
        switch (fileName) {
            case Const.EquipParamWeaponName: {
                EquipParamWeapon = Param.Read(file.Bytes);
                if (!EquipParamWeapon.ApplyParamDefsCarefully(ParamDefs)) {
                    throw new InvalidParamDefException(EquipParamWeapon.ParamType);
                }
                break;
            }
            case Const.EquipParamCustomWeaponName: {
                EquipParamCustomWeapon = Param.Read(file.Bytes);
                if (!EquipParamCustomWeapon.ApplyParamDefsCarefully(ParamDefs)) {
                    throw new InvalidParamDefException(EquipParamCustomWeapon.ParamType);
                }
                break;
            }
            case Const.EquipParamGoodsName: {
                EquipParamGoods = Param.Read(file.Bytes);
                if (!EquipParamGoods.ApplyParamDefsCarefully(ParamDefs)) {
                    throw new InvalidParamDefException(EquipParamGoods.ParamType);
                }
                break;
            }
            case Const.EquipParamProtectorName: {
                EquipParamProtector = Param.Read(file.Bytes);
                if (!EquipParamProtector.ApplyParamDefsCarefully(ParamDefs)) {
                    throw new InvalidParamDefException(EquipParamProtector.ParamType);
                }
                break;
            }
            case Const.CharaInitParamName: {
                CharaInitParam = Param.Read(file.Bytes);
                if (!CharaInitParam.ApplyParamDefsCarefully(ParamDefs)) {
                    throw new InvalidParamDefException(CharaInitParam.ParamType);
                }
                break;
            }
            case Const.MagicName: {
                GoodsParam = Param.Read(file.Bytes);
                if (!GoodsParam.ApplyParamDefsCarefully(ParamDefs)) {
                    throw new InvalidParamDefException(GoodsParam.ParamType);
                }
                break;
            }
            case Const.ItemLotParam_mapName: {
                ItemLotParamMap = Param.Read(file.Bytes);
                if (!ItemLotParamMap.ApplyParamDefsCarefully(ParamDefs)) {
                    throw new InvalidParamDefException(ItemLotParamMap.ParamType);
                }
                break;
            }
            case Const.ItemLotParam_enemyName: {
                ItemLotParamEnemy = Param.Read(file.Bytes);
                if (!ItemLotParamEnemy.ApplyParamDefsCarefully(ParamDefs)) {
                    throw new InvalidParamDefException(ItemLotParamEnemy.ParamType);
                }
                break;
            }
            case Const.ShopLineupParamName: {
                ShopLineupParam = Param.Read(file.Bytes);
                if (!ShopLineupParam.ApplyParamDefsCarefully(ParamDefs)) {
                    throw new InvalidParamDefException(ShopLineupParam.ParamType);
                }
                break;
            }
            case Const.AtkParamPcName: {
                AtkParamPc = Param.Read(file.Bytes);
                if (!AtkParamPc.ApplyParamDefsCarefully(ParamDefs)) {
                    throw new InvalidParamDefException(AtkParamPc.ParamType);
                }
                break;
            }
        }
    }
    private void getFmgs(BinderFile file) {
        string fileName = System.IO.Path.GetFileName(file.Name);
        switch (fileName) {
            case Const.WeaponNameName:
                WeaponFmg = FMG.Read(file.Bytes);
                break;
            case Const.ProtectorNameName:
                ProtectorFmg = FMG.Read(file.Bytes);
                break;
            case Const.GoodsNameName:
                GoodsFmg = FMG.Read(file.Bytes);
                break;
            case Const.GR_LineHelpName:
                LineHelpFmg = FMG.Read(file.Bytes);
                break;
            case Const.GR_MenuTextName:
                MenuTextFmg = FMG.Read(file.Bytes);
                break;
        }
    }
    
}
