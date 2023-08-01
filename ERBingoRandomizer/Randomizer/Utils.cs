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
using static ERBingoRandomizer.Const;

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
    private static void copyShopLineupParam(ShopLineupParam lot, ShopLineupParam shopLineupParam) {
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
    private static int removeWeaponMetadata(int id) {
        return id / 10000 * 10000;
    }
    private static int removeWeaponLevels(int id) {
        return id / 100 * 100;
    }
    private Dictionary<int, ItemLotEntry> getReplacementHashmap(OrderedDictionary orderedDictionary) {
        Dictionary<int, ItemLotEntry> dict = new();
        for (int i = 0; i < orderedDictionary.Count; i++) {
            List<ItemLotEntry> value = (List<ItemLotEntry>)orderedDictionary[i]!;
            List<ItemLotEntry> itemLotEntries = new(value);
            foreach (ItemLotEntry entry in itemLotEntries) {
                dict.Add(entry.Id, getNewId(entry.Id, value));
            }
        }
        
        return dict;
    }
    private Dictionary<int, ShopLineupParam> getShopReplacementHashmap(OrderedDictionary orderedDictionary) {
        Dictionary<int, ShopLineupParam> dict = new();
        for (int i = 0; i < orderedDictionary.Count; i++) {
            List<ShopLineupParam> value = (List<ShopLineupParam>)orderedDictionary[i]!;
            List<ShopLineupParam> itemLotEntries = new(value);
            foreach (ShopLineupParam entry in itemLotEntries) {
                dict.Add(entry.equipId, getNewId(entry.equipId, value));
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
    private void dedupeAndRandomizeVectors(OrderedDictionary orderedDictionary) {
        for (int i = 0; i < orderedDictionary.Count; i++) {
            List<ItemLotEntry> value = (List<ItemLotEntry>)orderedDictionary[i]!;
            List<ItemLotEntry> distinct = value.Distinct().ToList();
            orderedDictionary[i] = distinct.OrderBy(_ => _random.Next()).ToList();
        }
    }
    private void dedupeAndRandomizeShopVectors(OrderedDictionary orderedDictionary) {
        for (int i = 0; i < orderedDictionary.Count; i++) {
            List<ShopLineupParam> value = (List<ShopLineupParam>)orderedDictionary[i]!;
            List<ShopLineupParam> distinct = value.Distinct().ToList();
            orderedDictionary[i] = distinct.OrderBy(_ => _random.Next()).ToList();
        }
    }
    private static void addToOrderedDict<T>(OrderedDictionary orderedDict, object key, T type) {
        List<T>? ids = (List<T>?)orderedDict[key];
        if (ids != null) {
            ids.Add(type);
        }
        else {
            ids = new List<T> {
                type
            };
            orderedDict.Add(key, ids);
        }
    }
    private static bool chrCanUseWeapon(EquipParamWeapon wep, CharaInitParam chr) {
        return wep.properStrength <= chr.baseStr
            && wep.properAgility <= chr.baseDex
            && wep.properMagic <= chr.baseMag
            && wep.properFaith <= chr.baseFai
            && wep.properLuck <= chr.baseLuc;
    }
    private static bool chrCanUseSpell(Magic spell, CharaInitParam chr) {
        return spell.requirementIntellect <= chr.baseMag
            && spell.requirementFaith <= chr.baseFai
            && spell.requirementLuck <= chr.baseLuc;
    }
    private void replaceShopLineupParam(ShopLineupParam lot, List<int> shopLineupParamDictionary, List<ShopLineupParam> shopLineupParamRemembranceList) {
        if (lot.mtrlId == -1) {
            int newId = getNewId(lot.equipId, shopLineupParamDictionary);
            logItem($"{_weaponNameDictionary[lot.equipId]} -> {_weaponNameDictionary[newId]}");
            lot.equipId = newId;
            return;
        }
        ShopLineupParam newRemembrance = getNewId(lot.equipId, shopLineupParamRemembranceList);
        logItem($"{_weaponNameDictionary[lot.equipId]} -> {_weaponNameDictionary[newRemembrance.equipId]}");
        copyShopLineupParam(lot, newRemembrance);
    }
    private void replaceShopLineupParamMagic(ShopLineupParam lot, Dictionary<int, ShopLineupParam> shopLineupParamDictionary, List<ShopLineupParam> shopLineupParamRemembranceList) {
        if (lot.mtrlId == -1) {
            ShopLineupParam newItem = shopLineupParamDictionary[lot.equipId];
            logItem($"{_goodsFmg[lot.equipId]} -> {_goodsFmg[newItem.equipId]}");
            copyShopLineupParam(lot, newItem);
            return;
        }
        ShopLineupParam newRemembrance = getNewId(lot.equipId, shopLineupParamRemembranceList);
        logItem($"{_goodsFmg[lot.equipId]} -> {_goodsFmg[newRemembrance.equipId]}");
        copyShopLineupParam(lot, newRemembrance);
    }
    private void addDescriptionString(CharaInitParam chr, int id) {
        List<string> str = new();
        str.Add($"{_weaponNameDictionary[chr.wepleft]}{getRequiredLevelsWeapon(chr, chr.wepleft)}");
        str.Add($"{_weaponNameDictionary[chr.wepRight]}{getRequiredLevelsWeapon(chr, chr.wepRight)}");
        if (chr.subWepLeft != -1) {
            str.Add($"{_weaponNameDictionary[chr.subWepLeft]}{getRequiredLevelsWeapon(chr, chr.subWepLeft)}");
        }
        if (chr.subWepRight != -1) {
            str.Add($"{_weaponNameDictionary[chr.subWepRight]}{getRequiredLevelsWeapon(chr, chr.subWepRight)}");
        }
        if (chr.subWepLeft3 != -1) {
            str.Add($"{_weaponNameDictionary[chr.subWepLeft3]}{getRequiredLevelsWeapon(chr, chr.subWepLeft3)}");
        }
        if (chr.subWepRight3 != -1) {
            str.Add($"{_weaponNameDictionary[chr.subWepRight3]}{getRequiredLevelsWeapon(chr, chr.subWepRight3)}");
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

        _lineHelpFmg[id] = string.Join(", ", str);
    }
    private static int getSeedFromHashData(byte[] hashData) {
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
        Directory.CreateDirectory(Path.GetDirectoryName($"{BingoPath}/{RegulationName}") ?? throw new InvalidOperationException());
        setBndFile(_regulationBnd, CharaInitParamName, _charaInitParam.Write());
        setBndFile(_regulationBnd, ItemLotParam_mapName, _itemLotParam_map.Write());
        setBndFile(_regulationBnd, ItemLotParam_enemyName, _itemLotParam_enemy.Write());
        setBndFile(_regulationBnd, ShopLineupParamName, _shopLineupParam.Write());
        setBndFile(_regulationBnd, EquipParamWeaponName, _equipParamWeapon.Write());
        setBndFile(_regulationBnd, AtkParamPcName, _atkParam_Pc.Write());
        SFUtil.EncryptERRegulation($"{BingoPath}/{RegulationName}", _regulationBnd);
        Directory.CreateDirectory(Path.GetDirectoryName($"{BingoPath}/{MenuMsgBNDPath}") ?? throw new InvalidOperationException());
        setBndFile(_menuMsgBND, GR_LineHelpName, _lineHelpFmg.Write());
        File.WriteAllBytes($"{BingoPath}/{MenuMsgBNDPath}", _menuMsgBND.Write());

    }
    private static void setBndFile(BND4 files, string fileName, byte[] bytes) {
        foreach (BinderFile file in files.Files) {
            if (file.Name.EndsWith(fileName)) {
                file.Bytes = bytes;
            }
        }
    }
    private void logReplacementDictionary(Dictionary<int, ItemLotEntry> dict) {
        foreach (KeyValuePair<int, ItemLotEntry> pair in dict) {
            logItem($"{_weaponNameDictionary[pair.Key]} -> {_weaponNameDictionary[pair.Value.Id]}");
        }
    }
    private void logReplacementDictionaryMagic(Dictionary<int, ItemLotEntry> dict) {
        foreach (KeyValuePair<int, ItemLotEntry> pair in dict) {
            logItem($"{_goodsFmg[pair.Key]} -> {_goodsFmg[pair.Value.Id]}");
        }
    }
    private void logCharaInitEntry(CharaInitParam chr, int i) {
        logItem($"\n> {_menuTextFmg[i]}");
        logItem("> Weapons");
        if (chr.wepleft != -1) {
            logItem($"Left: {_weaponFmg[chr.wepleft]}{getRequiredLevelsWeapon(chr, chr.wepleft)}");
        }
        if (chr.wepRight != -1) {
            logItem($"Right: {_weaponFmg[chr.wepRight]}{getRequiredLevelsWeapon(chr, chr.wepRight)}");
        }
        if (chr.subWepLeft != -1) {
            logItem($"Left 2: {_weaponFmg[chr.subWepLeft]}{getRequiredLevelsWeapon(chr, chr.subWepLeft)}");
        }
        if (chr.subWepRight != -1) {
            logItem($"Right 2: {_weaponFmg[chr.subWepRight]}{getRequiredLevelsWeapon(chr, chr.subWepRight)}");
        }
        if (chr.subWepLeft3 != -1) {
            logItem($"Left 3: {_weaponFmg[chr.subWepLeft3]}{getRequiredLevelsWeapon(chr, chr.subWepLeft3)}");
        }
        if (chr.subWepRight3 != -1) {
            logItem($"Right 3: {_weaponFmg[chr.subWepRight3]}{getRequiredLevelsWeapon(chr, chr.subWepRight3)}");
        }
        
        logItem("\n> Armor");
        logItem($"Helm: {_protectorFmg[chr.equipHelm]}");
        logItem($"Body: {_protectorFmg[chr.equipArmer]}");
        logItem($"Arms: {_protectorFmg[chr.equipGaunt]}");
        logItem($"Legs: {_protectorFmg[chr.equipLeg]}");

        logItem("\n> Levels");
        logItem($"Vigor: {chr.baseVit}");
        logItem($"Attunement: {chr.baseWil}");
        logItem($"Endurance: {chr.baseEnd}");
        logItem($"Strength: {chr.baseStr}");
        logItem($"Dexterity: {chr.baseDex}");
        logItem($"Intelligence: {chr.baseMag}");
        logItem($"Faith: {chr.baseFai}");
        logItem($"Arcane: {chr.baseLuc}");

        if (chr.equipArrow != -1 || chr.equipSubArrow != -1 || chr.equipBolt != -1 || chr.equipSubBolt != -1) {
            logItem("\n> Ammo");
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
        }

        if (chr.equipSpell01 != -1 || chr.equipSpell02 != -1) {
            logItem("\n> Spells");
            if (chr.equipSpell01 != -1) {
                logItem($"{_goodsFmg[chr.equipSpell01]}{getRequiredLevelsSpell(chr, chr.equipSpell01)}");
            }
            if (chr.equipSpell02 != -1) {
                logItem($"{_goodsFmg[chr.equipSpell02]}{getRequiredLevelsSpell(chr, chr.equipSpell02)}");
            }
        }

        logItem("");
    }
    private string getRequiredLevelsWeapon(CharaInitParam chr, int id) {
        EquipParamWeapon wep = _weaponDictionary[id];
        int reqLevels = 0;
        if (wep.properStrength > chr.baseStr) {
            reqLevels += wep.properStrength - chr.baseStr;
        }
        if (wep.properAgility > chr.baseDex) {
            reqLevels += wep.properAgility - chr.baseDex;
        }
        if (wep.properMagic > chr.baseMag) {
            reqLevels += wep.properMagic - chr.baseMag;
        }
        if (wep.properFaith > chr.baseFai) {
            reqLevels += wep.properFaith - chr.baseFai;
        }
        if (wep.properLuck > chr.baseLuc) {
            reqLevels += wep.properLuck - chr.baseLuc;
        }

        return reqLevels > 0 ? $" (-{reqLevels})" : "";

    }
    private string getRequiredLevelsSpell(CharaInitParam chr, int id) {
        Magic spell = _magicDictionary[id];
        int reqLevels = 0;
        if (spell.requirementIntellect > chr.baseMag) {
            reqLevels += spell.requirementIntellect - chr.baseMag;
        }
        if (spell.requirementFaith > chr.baseFai) {
            reqLevels += spell.requirementFaith - chr.baseFai;
        }
        if (spell.requirementLuck > chr.baseLuc) {
            reqLevels += spell.requirementLuck - chr.baseLuc;
        }

        if (reqLevels > 0) {
            return $" (-{reqLevels})";
        }

        return "";
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
    private void logShopIdMagic(int rowId) {
        switch (rowId) {
            case 100050:
                logItem("\n> Sorceress Sellen");
                break;
            case 100056:
                logItem("\n> Sorceress Sellen - Quest");
                break;
            case 100057:
                logItem("\n> Sorceress Sellen - Conspectus Scroll");
                break;
            case 100059:
                logItem("\n> Sorceress Sellen -  Academy Scroll");
                break;
            case 100061:
                logItem("\n> Sorceress Sellen");
                break;
            case 100126:
                logItem("\n> D Hunter of The Dead");
                break;
            case 100175:
                logItem("\n> Gowry");
                break;
            case 100250:
                logItem("\n> Preceptor Seluvis");
                break;
            case 100300:
                logItem("\n> Preceptor Seluvis - Ranni Quest");
                break;
            case 100310:
                logItem("\n> Preceptor Seluvis - Dung Eater Quest");
                break;
            case 100350:
                logItem("\n> Brother Corhyn");
                break;
            case 100358:
                logItem("\n> Brother Corhyn - Altus Plateau");
                break;
            case 100360:
                logItem("\n> Brother Corhyn - Goldmask");
                break;
            case 100361:
                logItem("\n> Brother Corhyn - Erdtree Sanctuary");
                break;
            case 100362:
                logItem("\n> Brother Corhyn - Fire Monks' Prayerbook");
                break;
            case 100364:
                logItem("\n> Brother Corhyn - Giant's Prayerbook");
                break;
            case 100368:
                logItem("\n> Brother Corhyn - Two Fingers' Prayerbook");
                break;
            case 100370:
                logItem("\n> Brother Corhyn - Assassin's Prayerbook");
                break;
            case 100372:
                logItem("\n> Brother Corhyn - Golden Order Principia");
                break;
            case 100374:
                logItem("\n> Brother Corhyn - Dragon Cult Prayerbook");
                break;
            case 100377:
                logItem("\n> Brother Corhyn - Ancient Dragon Prayerbook");
                break;
            case 100400:
                logItem("\n> Miriel");
                break;
            case 100402:
                logItem("\n> Miriel - Conspectus Scroll");
                break;
            case 100404:
                logItem("\n> Miriel - Academy Scroll");
                break;
            case 100406:
                logItem("\n> Miriel");
                break;
            case 100426:
                logItem("\n> Miriel - Fire Monks' Prayerbook");
                break;
            case 100429:
                logItem("\n> Miriel - Giant's Prayerbook");
                break;
            case 100433:
                logItem("\n> Miriel - Two Fingers' Prayerbook");
                break;
            case 100435:
                logItem("\n> Miriel - Assassin's Prayerbook");
                break;
            case 100437:
                logItem("\n> Miriel - Golden Order Principia");
                break;
            case 100439:
                logItem("\n> Miriel - Dragon Cult Prayerbook");
                break;
            case 100442:
                logItem("\n> Miriel - Ancient Dragon Prayerbook");
                break;
            case 101905:
                logItem("\n> Remembrance");
                break;
            case 101950:
                logItem("\n> Dragon Communion");
                break;
        }
    }
    private static void patchSpEffectAtkPowerCorrectRate(AtkParam atkParam) {
        atkParam.spEffectAtkPowerCorrectRate_byPoint = 100;
        atkParam.spEffectAtkPowerCorrectRate_byRate = 100;
        atkParam.spEffectAtkPowerCorrectRate_byDmg = 100;
    }
}
