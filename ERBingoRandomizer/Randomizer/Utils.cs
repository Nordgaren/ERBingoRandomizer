using ERBingoRandomizer.Params;
using ERBingoRandomizer.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static ERBingoRandomizer.Utility.Config;

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
    private void shuffleVectors(OrderedDictionary orderedDictionary) {
        for (int i = 0; i < orderedDictionary.Count; i++) {
            List<ShopLineupParam> value = (List<ShopLineupParam>)orderedDictionary[i];
            orderedDictionary[i] = value.OrderBy(i => _random.Next()).ToList();
        }
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
}
