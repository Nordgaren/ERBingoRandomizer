using ERBingoRandomizer.Params;
using ERBingoRandomizer.Utility;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static ERBingoRandomizer.Utility.Config;
using static ERBingoRandomizer.Utility.Const;

namespace ERBingoRandomizer.Randomizer;

public partial class BingoRandomizer {
    private List<string> _randomizerLog;
    private void logItem(string item) {
        _randomizerLog.Add(item);
    }
    private void writeLog() {
        Directory.CreateDirectory(SpoilerPath);
        File.WriteAllLines($"{SpoilerPath}/spoiler-{_seed}.log", _randomizerLog);
    }
    private void copyShopLineupParam(ShopLineupParam lot, ShopLineupParam shopLineupParam) {
        lot.equipId = shopLineupParam.equipId;
        lot.costType = shopLineupParam.costType;
        lot.sellQuantity = shopLineupParam.sellQuantity;
        lot.setNum = shopLineupParam.setNum;
        lot.value = shopLineupParam.value;
        lot.value_Add = shopLineupParam.value_Add;
        lot.value_Magnification = shopLineupParam.value_Magnification;
        lot.iconId = shopLineupParam.iconId;
        lot.nameMsgId = shopLineupParam.nameMsgId;
        lot.menuIconId = shopLineupParam.menuIconId;
        lot.menuTitleMsgId = shopLineupParam.menuTitleMsgId;
    }
    private int removeWeaponMetadata(int id) {
        return id / 10000 * 10000;
    }
    private int removeWeaponLevels(int id) {
        return id / 100 * 100;
    }
    private Dictionary<int, int> getReplacementHashmap(OrderedDictionary orderedDictionary) {
        Dictionary<int, int> dict = new();
        for (int i = 0; i < orderedDictionary.Count; i++) {
            List<int> value = (List<int>)orderedDictionary[i];
            List<int> ids = new(value);
            foreach (int id in ids) {
                dict.Add(id, getNewId(id, value));
            }
        }

        return dict;
    }
    private T getNewId<T>(int oldId, List<T> vec) where T : IEquatable<int> {
        if (vec.All(i => i.Equals(oldId))) {
            Debug.WriteLine($"No New Ids for {oldId}");
            return vec.Pop();
        }

        T newId = vec.Pop();
        while (newId.Equals(oldId)) {
            vec.Insert(0, newId);
            newId = vec.Pop();
        }

        return newId;
    }
    private void dedupVectors(OrderedDictionary orderedDictionary) {
        for (int i = 0; i < orderedDictionary.Count; i++) {
            List<int> value = (List<int>)orderedDictionary[i];
            List<int> distinct = value.Distinct().ToList();
            orderedDictionary[i] = distinct.OrderBy(i => _random.Next()).ToList();
        }
    }
    private static void addToOrderedDict<T>(OrderedDictionary orderedDict, EquipParamWeapon wep, T type) {
        List<T>? ids = (List<T>?)orderedDict[(object)wep.wepType];
        if (ids != null) {
            ids.Add(type);
        }
        else {
            ids = new() {
                type
            };
            orderedDict.Add(wep.wepType, ids);
        }
    }
    public static bool ChrCanUseWeapon(EquipParamWeapon wep, CharaInitParam chr) {
        return wep.properStrength <= chr.baseStr
            && wep.properAgility <= chr.baseDex
            && wep.properMagic <= chr.baseMag
            && wep.properFaith <= chr.baseFai
            && wep.properLuck <= chr.baseLuc;
    }
    public static bool ChrCanUseSpell(Magic spell, CharaInitParam chr) {
        return spell.requirementIntellect <= chr.baseMag
            && spell.requirementFaith <= chr.baseFai
            && spell.requirementLuck <= chr.baseLuc;
    }
    private void replaceShopLineupParam(ShopLineupParam lot, List<int> shopLineupParamDictionary, EquipParamWeapon wep, List<ShopLineupParam> shopLineupParamRememberanceList) {
        if (lot.mtrlId == -1) {
            int newId = getNewId(lot.equipId, shopLineupParamDictionary);
            logItem($"{_weaponNameDictionary[lot.equipId]} -> {_weaponNameDictionary[newId]}");
            lot.equipId = newId;
            return;
        }
        ShopLineupParam newRemembrance = getNewId(lot.equipId, shopLineupParamRememberanceList);
        logItem($"{_weaponNameDictionary[lot.equipId]} -> {_weaponNameDictionary[newRemembrance.equipId]}");
        copyShopLineupParam(lot, newRemembrance);
    }
    private void addDescriptionString(CharaInitParam chr, int id) {
        List<string> str = new();
        str.Add(_weaponFmg[chr.wepRight]);

        if (chr.wepleft != -1) {
            str.Add($"{_weaponNameDictionary[chr.wepleft]}");
        }
        if (chr.subWepLeft != -1) {
            str.Add($"{_weaponNameDictionary[chr.subWepLeft]}");
        }
        if (chr.subWepRight != -1) {
            str.Add($"{_weaponNameDictionary[chr.subWepRight]}");
        }
        if (chr.subWepLeft3 != -1) {
            str.Add($"{_weaponNameDictionary[chr.subWepLeft3]}");
        }
        if (chr.subWepRight3 != -1) {
            str.Add($"{_weaponNameDictionary[chr.subWepRight3]}");
        }
        if (chr.equipArrow != -1) {
            str.Add($"{_weaponNameDictionary[chr.equipArrow]}[{chr.arrowNum}]");
        }
        if (chr.equipSubArrow != -1) {
            str.Add($"{_weaponNameDictionary[chr.equipSubArrow]}[{chr.subArrowNum}]");
        }
        if (chr.equipBolt != -1) {
            str.Add($"{_weaponNameDictionary[chr.equipBolt]}[{chr.boltNum}]");
        }
        if (chr.equipSubBolt != -1) {
            str.Add($"{_weaponNameDictionary[chr.equipSubBolt]}[{chr.subBoltNum}]");
        }
        if (chr.equipSpell01 != -1) {
            str.Add($"{_goodsFmg[chr.equipSpell01]}");
        }
        if (chr.equipSpell02 != -1) {
            str.Add($"{_goodsFmg[chr.equipSpell02]}");
        }

        _lineHelpFmg[id] = string.Join(", ", str); //Util.SplitCharacterText(true, str);
    }
    private int getSeedFromHashData(byte[] hashData) {
        IEnumerable<byte[]> chunks = hashData.Chunk(4);
        int num = 0;
        foreach (byte[] chunk in chunks) {
            num ^= BitConverter.ToInt32(chunk);
        }

        return num;
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
        setBndFile(_regulationBnd, EquipParamWeaponName, _equipParamWeapon.Write());
        SFUtil.EncryptERRegulation($"{BingoPath}/{RegulationName}", _regulationBnd);
        Directory.CreateDirectory(Path.GetDirectoryName($"{BingoPath}/{MenuMsgBNDPath}"));
        setBndFile(_menuMsgBND, GR_LineHelpName, _lineHelpFmg.Write());
        File.WriteAllBytes($"{BingoPath}/{MenuMsgBNDPath}", _menuMsgBND.Write());

    }
    private void setBndFile(BND4 files, string fileName, byte[] bytes) {
        foreach (BinderFile file in files.Files) {
            if (file.Name.EndsWith(fileName)) {
                file.Bytes = bytes;
            }
        }
    }
    private void logReplacementDictionary(Dictionary<int, int> dict) {
        foreach (KeyValuePair<int, int> pair in dict) {
            logItem($"{_weaponNameDictionary[pair.Key]} -> {_weaponNameDictionary[pair.Value]}");
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
        logItem("\nArmor");
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
        logItem("\nLevels");
        logItem($"Vigor: {chr.baseVit}");
        logItem($"Attunement: {chr.baseWil}");
        logItem($"Endurance: {chr.baseEnd}");
        logItem($"Strength: {chr.baseStr}");
        logItem($"Dexterity: {chr.baseDex}");
        logItem($"Intelligence: {chr.baseMag}");
        logItem($"Faith: {chr.baseFai}");
        logItem($"Arcane: {chr.baseLuc}");
        logItem("\nAmmo");
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
        logItem("\nSpells");
        if (chr.equipSpell01 != -1) {
            logItem($"{_goodsFmg[chr.equipSpell01]}");
        }
        if (chr.equipSpell02 != -1) {
            logItem($"{_goodsFmg[chr.equipSpell02]}");
        }
        logItem("");
    }
    private void logShopId(int rowId) {
        switch (rowId) {
            case 100000:
                logItem("\n> Gatekeeper Gostoc");
                break;
            case 100100:
                logItem("\n> Patches");
                break;
            case 100325:
                logItem("\n> Pidia Carian Servant");
                break;
            case 100500:
                logItem("\n> Merchant Kale");
                break;
            case 100525:
                logItem("\n> Merchant - North Limgrave");
                break;
            case 100550:
                logItem("\n> Merchant - East Limgrave");
                break;
            case 100575:
                logItem("\n> Merchant - Coastal Cave");
                break;
            case 100600:
                logItem("\n> Merchant - East Weeping Peninsula");
                break;
            case 100625:
                logItem("\n> Merchant - Liurnia of the Lakes");
                break;
            case 100650:
                logItem("\n> Isolated Merchant - Weeping Peninsula");
                break;
            case 100700:
                logItem("\n> Merchant - North Liurnia");
                break;
            case 100725:
                logItem("\n> Hermit Merchant - Leyndell");
                break;
            case 100750:
                logItem("\n> Merchant - Altus Plateau");
                break;
            case 100875:
                logItem("\n> Isolated Merchant - Dragonbarrow");
                break;
            case 100925:
                logItem("\n> Merchant - Siofra River");
                break;
            case 101800:
                logItem("\n> Twin Maiden Husks");
                break;
            case 101900:
                logItem("\n> Remembrances");
                break;
        }
    }
}
