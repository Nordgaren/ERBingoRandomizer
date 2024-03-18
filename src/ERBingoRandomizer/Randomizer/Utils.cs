using ERBingoRandomizer.Params;
using ERBingoRandomizer.Utility;
using FSParam;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ERBingoRandomizer.Randomizer;

public partial class BingoRandomizer {
    private Dictionary<int, ItemLotEntry> getReplacementHashmap(IOrderedDictionary orderedDictionary) {
        Dictionary<int, ItemLotEntry> dict = new();

        List<ItemLotEntry> bows = (List<ItemLotEntry>?)orderedDictionary[(object)Const.BowType] ?? new List<ItemLotEntry>();
        List<ItemLotEntry> lightbows = (List<ItemLotEntry>?)orderedDictionary[(object)Const.LightBowType] ?? new List<ItemLotEntry>();
        List<ItemLotEntry> greatbows = (List<ItemLotEntry>?)orderedDictionary[(object)Const.GreatbowType] ?? new List<ItemLotEntry>();
        List<ItemLotEntry> crossbows = (List<ItemLotEntry>?)orderedDictionary[(object)Const.CrossbowType] ?? new List<ItemLotEntry>();
        List<ItemLotEntry> ballista = (List<ItemLotEntry>?)orderedDictionary[(object)Const.BallistaType] ?? new List<ItemLotEntry>();

        bows.AddRange(lightbows);
        bows.AddRange(greatbows);
        bows.AddRange(crossbows);
        bows.AddRange(ballista);
        orderedDictionary[(object)Const.BowType] = bows;
        orderedDictionary.Remove(Const.LightBowType);
        orderedDictionary.Remove(Const.GreatbowType);
        orderedDictionary.Remove(Const.CrossbowType);
        orderedDictionary.Remove(Const.BallistaType);

        for (int i = 0; i < orderedDictionary.Count; i++) {
            List<ItemLotEntry> value = (List<ItemLotEntry>)orderedDictionary[i]!;
            List<ItemLotEntry> itemLotEntries = new(value);
            itemLotEntries.Shuffle(_resources.Random);
            foreach (ItemLotEntry entry in itemLotEntries) {
                dict.Add(entry.Id, getNewId(entry.Id, value));
            }
        }

        return dict;
    }
    private Dictionary<int, int> getShopReplacementHashmap(IOrderedDictionary orderedDictionary) {
        Dictionary<int, int> dict = new();
        for (int i = 0; i < orderedDictionary.Count; i++) {
            List<int> value = (List<int>)orderedDictionary[i]!;
            List<int> itemLotEntries = new(value);
            itemLotEntries.Shuffle(_resources.Random);
            foreach (int entry in itemLotEntries) {
                dict.Add(entry, getNewId(entry, value));
            }
        }

        return dict;
    }
    private void dedupeAndRandomizeVectors(IOrderedDictionary orderedDictionary) {
        for (int i = 0; i < orderedDictionary.Count; i++) {
            List<ItemLotEntry> value = (List<ItemLotEntry>)orderedDictionary[i]!;
            List<ItemLotEntry> distinct = value.Distinct().ToList();
            distinct.Shuffle(_resources.Random);
            orderedDictionary[i] = distinct;
        }
    }
    private void dedupeAndRandomizeShopVectors(IOrderedDictionary orderedDictionary) {
        for (int i = 0; i < orderedDictionary.Count; i++) {
            List<int> value = (List<int>)orderedDictionary[i]!;
            List<int> distinct = value.Distinct().ToList();
            distinct.Shuffle(_resources.Random);
            orderedDictionary[i] = distinct;
        }
    }
    private void replaceShopLineupParam(ShopLineupParam lot, IList<int> shopLineupParamDictionary, IList<ShopLineupParam> shopLineupParamRemembranceList) {
        if (lot.mtrlId == -1) {
            int newId = getNewId(lot.equipId, shopLineupParamDictionary);
            Logger.LogItem($"{_resources.WeaponNameDictionary[lot.equipId]} -> {_resources.WeaponNameDictionary[newId]}");
            lot.equipId = newId;
            return;
        }
        ShopLineupParam newRemembrance = getNewId(lot.equipId, shopLineupParamRemembranceList);
        Logger.LogItem($"{_resources.WeaponNameDictionary[lot.equipId]} -> {_resources.WeaponNameDictionary[newRemembrance.equipId]}");
        copyShopLineupParam(lot, newRemembrance);
    }
    private void replaceShopLineupParamMagic(ShopLineupParam lot, IReadOnlyDictionary<int, int> shopLineupParamDictionary, IList<ShopLineupParam> shopLineupParamRemembranceList) {
        if (lot.mtrlId == -1) {
            int newItem = shopLineupParamDictionary[lot.equipId];
            Logger.LogItem($"{_resources.GoodsFmg[lot.equipId]} -> {_resources.GoodsFmg[newItem]}");
            lot.equipId = newItem;
            return;
        }
        ShopLineupParam newRemembrance = getNewId(lot.equipId, shopLineupParamRemembranceList);
        Logger.LogItem($"{_resources.GoodsFmg[lot.equipId]} -> {_resources.GoodsFmg[newRemembrance.equipId]}");
        copyShopLineupParam(lot, newRemembrance);
    }
    private void writeFiles() {
        if (Directory.Exists(Const.BingoPath)) {
            Directory.Delete(Const.BingoPath, true);
        }
        Directory.CreateDirectory(Path.GetDirectoryName($"{Const.BingoPath}/{Const.RegulationName}") ?? throw new InvalidOperationException());
        setBndFile(_resources.RegulationBnd, Const.CharaInitParamName, _resources.CharaInitParam.Write());
        setBndFile(_resources.RegulationBnd, Const.ItemLotParam_mapName, _resources.ItemLotParamMap.Write());
        setBndFile(_resources.RegulationBnd, Const.ItemLotParam_enemyName, _resources.ItemLotParamEnemy.Write());
        setBndFile(_resources.RegulationBnd, Const.ShopLineupParamName, _resources.ShopLineupParam.Write());
        setBndFile(_resources.RegulationBnd, Const.EquipParamWeaponName, _resources.EquipParamWeapon.Write());
        setBndFile(_resources.RegulationBnd, Const.EquipMtrlSetParamName, _resources.EquipMtrlSetParam.Write());
        setBndFile(_resources.RegulationBnd, Const.AtkParamPcName, _resources.AtkParamPc.Write());
        SFUtil.EncryptERRegulation($"{Const.BingoPath}/{Const.RegulationName}", _resources.RegulationBnd);
        Directory.CreateDirectory(Path.GetDirectoryName($"{Const.BingoPath}/{Const.MenuMsgBNDPath}") ?? throw new InvalidOperationException());
        setBndFile(_resources.MenuMsgBnd, Const.GR_LineHelpName, _resources.LineHelpFmg.Write());
        File.WriteAllBytes($"{Const.BingoPath}/{Const.MenuMsgBNDPath}", _resources.MenuMsgBnd.Write());

    }
    private void logReplacementDictionary(Dictionary<int, ItemLotEntry> dict) {
        foreach (KeyValuePair<int, ItemLotEntry> pair in dict) {
            Logger.LogItem($"{_resources.WeaponNameDictionary[pair.Key]} -> {_resources.WeaponNameDictionary[pair.Value.Id]}");
        }
    }
    private void logReplacementDictionaryMagic(Dictionary<int, int> dict) {
        foreach (KeyValuePair<int, int> pair in dict) {
            Logger.LogItem($"{_resources.GoodsFmg[pair.Key]} -> {_resources.GoodsFmg[pair.Value]}");
        }
    }
    private void logShopId(int rowId) {
        switch (rowId) {
            case 100000:
                Logger.LogItem("\n> Gatekeeper Gostoc");
                break;
            case 100100:
                Logger.LogItem("\n> Patches");
                break;
            case 100325:
                Logger.LogItem("\n> Pidia Carian Servant");
                break;
            case 100500:
                Logger.LogItem("\n> Merchant Kale");
                break;
            case 100525:
                Logger.LogItem("\n> Merchant - North Limgrave");
                break;
            case 100550:
                Logger.LogItem("\n> Merchant - East Limgrave");
                break;
            case 100575:
                Logger.LogItem("\n> Merchant - Coastal Cave");
                break;
            case 100600:
                Logger.LogItem("\n> Merchant - East Weeping Peninsula");
                break;
            case 100625:
                Logger.LogItem("\n> Merchant - Liurnia of the Lakes");
                break;
            case 100650:
                Logger.LogItem("\n> Isolated Merchant - Weeping Peninsula");
                break;
            case 100700:
                Logger.LogItem("\n> Merchant - North Liurnia");
                break;
            case 100725:
                Logger.LogItem("\n> Hermit Merchant - Leyndell");
                break;
            case 100750:
                Logger.LogItem("\n> Merchant - Altus Plateau");
                break;
            case 100875:
                Logger.LogItem("\n> Isolated Merchant - Dragonbarrow");
                break;
            case 100925:
                Logger.LogItem("\n> Merchant - Siofra River");
                break;
            case 101800:
                Logger.LogItem("\n> Twin Maiden Husks");
                break;
            case 101900:
                Logger.LogItem("\n> Remembrances");
                break;
        }
    }
    private void logShopIdMagic(int rowId) {
        switch (rowId) {
            case 100050:
                Logger.LogItem("\n> Sorceress Sellen");
                break;
            case 100056:
                Logger.LogItem("\n> Sorceress Sellen - Quest");
                break;
            case 100057:
                Logger.LogItem("\n> Sorceress Sellen - Conspectus Scroll");
                break;
            case 100059:
                Logger.LogItem("\n> Sorceress Sellen -  Academy Scroll");
                break;
            case 100061:
                Logger.LogItem("\n> Sorceress Sellen");
                break;
            case 100126:
                Logger.LogItem("\n> D Hunter of The Dead");
                break;
            case 100175:
                Logger.LogItem("\n> Gowry");
                break;
            case 100250:
                Logger.LogItem("\n> Preceptor Seluvis");
                break;
            case 100300:
                Logger.LogItem("\n> Preceptor Seluvis - Ranni Quest");
                break;
            case 100310:
                Logger.LogItem("\n> Preceptor Seluvis - Dung Eater Quest");
                break;
            case 100350:
                Logger.LogItem("\n> Brother Corhyn");
                break;
            case 100358:
                Logger.LogItem("\n> Brother Corhyn - Altus Plateau");
                break;
            case 100360:
                Logger.LogItem("\n> Brother Corhyn - Goldmask");
                break;
            case 100361:
                Logger.LogItem("\n> Brother Corhyn - Erdtree Sanctuary");
                break;
            case 100362:
                Logger.LogItem("\n> Brother Corhyn - Fire Monks' Prayerbook");
                break;
            case 100364:
                Logger.LogItem("\n> Brother Corhyn - Giant's Prayerbook");
                break;
            case 100368:
                Logger.LogItem("\n> Brother Corhyn - Two Fingers' Prayerbook");
                break;
            case 100370:
                Logger.LogItem("\n> Brother Corhyn - Assassin's Prayerbook");
                break;
            case 100372:
                Logger.LogItem("\n> Brother Corhyn - Golden Order Principia");
                break;
            case 100374:
                Logger.LogItem("\n> Brother Corhyn - Dragon Cult Prayerbook");
                break;
            case 100377:
                Logger.LogItem("\n> Brother Corhyn - Ancient Dragon Prayerbook");
                break;
            case 100400:
                Logger.LogItem("\n> Miriel");
                break;
            case 100402:
                Logger.LogItem("\n> Miriel - Conspectus Scroll");
                break;
            case 100404:
                Logger.LogItem("\n> Miriel - Academy Scroll");
                break;
            case 100406:
                Logger.LogItem("\n> Miriel");
                break;
            case 100426:
                Logger.LogItem("\n> Miriel - Fire Monks' Prayerbook");
                break;
            case 100429:
                Logger.LogItem("\n> Miriel - Giant's Prayerbook");
                break;
            case 100433:
                Logger.LogItem("\n> Miriel - Two Fingers' Prayerbook");
                break;
            case 100435:
                Logger.LogItem("\n> Miriel - Assassin's Prayerbook");
                break;
            case 100437:
                Logger.LogItem("\n> Miriel - Golden Order Principia");
                break;
            case 100439:
                Logger.LogItem("\n> Miriel - Dragon Cult Prayerbook");
                break;
            case 100442:
                Logger.LogItem("\n> Miriel - Ancient Dragon Prayerbook");
                break;
            case 101905:
                Logger.LogItem("\n> Remembrance");
                break;
            case 101950:
                Logger.LogItem("\n> Dragon Communion");
                break;
        }
    }
    private static T getNewId<T>(int oldId, IList<T> vec) where T : IEquatable<int> {
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
    // ReSharper disable once SuggestBaseTypeForParameter
    private static void addToOrderedDict<T>(IOrderedDictionary orderedDict, object key, T type) {
        List<T>? ids = (List<T>?)orderedDict[key];
        if (ids != null) {
            ids.Add(type);
        }
        else {
            ids = new List<T> {
                type,
            };
            orderedDict.Add(key, ids);
        }
    }
    private static void setBndFile(IBinder binder, string fileName, byte[] bytes) {
        BinderFile file = binder.Files.First(file => file.Name.EndsWith(fileName)) ?? throw new BinderFileNotFoundException(fileName);
        file.Bytes = bytes;
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
    internal static int RemoveWeaponMetadata(int id) {
        return id / 10000 * 10000;
    }
    internal static int RemoveWeaponLevels(int id) {
        return id / 100 * 100;
    }
    private void calculateLevels() {
        for (int i = 0; i < 10; i++) {
            Param.Row? row = _resources.CharaInitParam[i + 3000];
            if (row == null) {
                continue;
            }
            CharaInitParam chr = new(row);

            Debug.WriteLine($"{_resources.MenuTextFmg[i + 288100]} {chr.soulLv} {addLevels(chr)}");
        }
    }
    private static int addLevels(CharaInitParam chr) {
        return chr.baseVit
            + chr.baseWil
            + chr.baseEnd
            + chr.baseStr
            + chr.baseDex
            + chr.baseMag
            + chr.baseFai
            + chr.baseLuc;
    }
}
