using Project.Params;
using Project.Settings;
using FSParam;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace Project.Tasks;

public partial class Randomizer
{
    List<string> _randomizerLog;
    void logItem(string item)
    { _randomizerLog.Add(item); }
    void writeLog()
    {
        Directory.CreateDirectory(Config.SpoilerPath);
        File.WriteAllLines($"{Config.SpoilerPath}/Starting_Classes.log", _randomizerLog);
    }
    void logReplacementDictionary(Dictionary<int, ItemLotEntry> dict)
    {
        foreach (KeyValuePair<int, ItemLotEntry> pair in dict)
        { logItem($"* {_weaponNameDictionary[pair.Key]} -> {_weaponNameDictionary[pair.Value.Id]}"); }
    }
    void logReplacementDictionaryMagic(Dictionary<int, int> dict)
    {
        foreach (KeyValuePair<int, int> pair in dict)
        { logItem($"*~ {_goodsFmg[pair.Key]} -> {_goodsFmg[pair.Value]}"); }
    }
    void logCharaInitEntry(CharaInitParam chr, int i)
    {
        logItem($"\n## {_menuTextFmg[i]}");
        List<string> str = new() {
            $"{_weaponNameDictionary[chr.wepleft]}{getRequiredLevelsWeapon(chr, chr.wepleft)}",
            $"{_weaponNameDictionary[chr.wepRight]}{getRequiredLevelsWeapon(chr, chr.wepRight)}",
        };
        if (chr.subWepLeft != -1)
        { str.Add($"{_weaponNameDictionary[chr.subWepLeft]}{getRequiredLevelsWeapon(chr, chr.subWepLeft)}"); }

        if (chr.subWepRight != -1)
        { str.Add($"{_weaponNameDictionary[chr.subWepRight]}{getRequiredLevelsWeapon(chr, chr.subWepRight)}"); }

        if (chr.subWepLeft3 != -1)
        { str.Add($"{_weaponNameDictionary[chr.subWepLeft3]}{getRequiredLevelsWeapon(chr, chr.subWepLeft3)}"); }

        if (chr.subWepRight3 != -1)
        { str.Add($"{_weaponNameDictionary[chr.subWepRight3]}{getRequiredLevelsWeapon(chr, chr.subWepRight3)}"); }

        if (chr.equipArrow != -1)
        { str.Add($"{_weaponNameDictionary[chr.equipArrow]}[{chr.arrowNum}]"); }

        if (chr.equipSubArrow != -1)
        { str.Add($"{_weaponNameDictionary[chr.equipSubArrow]}[{chr.subArrowNum}]"); }

        if (chr.equipBolt != -1)
        { str.Add($"{_weaponNameDictionary[chr.equipBolt]}[{chr.boltNum}]"); }

        if (chr.equipSubBolt != -1)
        { str.Add($"{_weaponNameDictionary[chr.equipSubBolt]}[{chr.subBoltNum}]"); }

        if (chr.equipSpell01 != -1)
        { str.Add($"{_goodsFmg[chr.equipSpell01]}"); }

        if (chr.equipSpell02 != -1)
        { str.Add($"{_goodsFmg[chr.equipSpell02]}"); }

        logItem(string.Join(", ", str));
        // logItem("__ Weapons");

        // if (chr.wepleft != -1)
        // { logItem($"Left: {_weaponNameDictionary[chr.wepleft]}{getRequiredLevelsWeapon(chr, chr.wepleft)}"); }

        // if (chr.wepRight != -1)
        // { logItem($"Right: {_weaponNameDictionary[chr.wepRight]}{getRequiredLevelsWeapon(chr, chr.wepRight)}"); }

        // if (chr.subWepLeft != -1)
        // { logItem($"Left 2: {_weaponNameDictionary[chr.subWepLeft]}{getRequiredLevelsWeapon(chr, chr.subWepLeft)}"); }

        // if (chr.subWepRight != -1)
        // { logItem($"Right 2: {_weaponNameDictionary[chr.subWepRight]}{getRequiredLevelsWeapon(chr, chr.subWepRight)}"); }

        // if (chr.subWepLeft3 != -1)
        // { logItem($"Left 3: {_weaponNameDictionary[chr.subWepLeft3]}{getRequiredLevelsWeapon(chr, chr.subWepLeft3)}"); }

        // if (chr.subWepRight3 != -1)
        // { logItem($"Right 3: {_weaponNameDictionary[chr.subWepRight3]}{getRequiredLevelsWeapon(chr, chr.subWepRight3)}"); }

        // logItem("\n__ Armor");
        // logItem($"Head: {_protectorFmg[chr.equipHelm]} : {chr.equipHelm}");
        // logItem($"Body: {_protectorFmg[chr.equipArmer]} : {chr.equipArmer}");
        // logItem($"Arms: {_protectorFmg[chr.equipGaunt]} : {chr.equipGaunt}");
        // logItem($"Legs: {_protectorFmg[chr.equipLeg]} : {chr.equipLeg}");

        // logItem("\n__ Starting Stats");
        logItem($"Vigor: {chr.baseVit}");
        logItem($"Mind: {chr.baseWil}");
        logItem($"Endurance: {chr.baseEnd}");
        logItem($"Strength: {chr.baseStr}");
        logItem($"Dexterity: {chr.baseDex}");
        logItem($"Intelligence: {chr.baseMag}");
        logItem($"Faith: {chr.baseFai}");
        logItem($"Arcane: {chr.baseLuc}");

        // if (chr.equipArrow != -1 || chr.equipSubArrow != -1 || chr.equipBolt != -1 || chr.equipSubBolt != -1)
        // {
        //     logItem("\n__ Ammo");

        //     if (chr.equipArrow != -1)
        //     { logItem($"{_weaponFmg[chr.equipArrow]}[{chr.arrowNum}]"); }

        //     if (chr.equipSubArrow != -1)
        //     { logItem($"{_weaponFmg[chr.equipSubArrow]}[{chr.subArrowNum}]"); }

        //     if (chr.equipBolt != -1)
        //     { logItem($"{_weaponFmg[chr.equipBolt]}[{chr.boltNum}]"); }

        //     if (chr.equipSubBolt != -1)
        //     { logItem($"{_weaponFmg[chr.equipSubBolt]}[{chr.subBoltNum}]"); }
        // }

        // if (chr.equipSpell01 != -1 || chr.equipSpell02 != -1)
        // {
        //     logItem("\n__ Spells");

        //     if (chr.equipSpell01 != -1)
        //     { logItem($"{_goodsFmg[chr.equipSpell01]}{getRequiredLevelsSpell(chr, chr.equipSpell01)}"); }

        //     if (chr.equipSpell02 != -1)
        //     { logItem($"{_goodsFmg[chr.equipSpell02]}{getRequiredLevelsSpell(chr, chr.equipSpell02)}"); }
        // }
        // logItem("");
    }
    void logShopId(int rowId)
    {
        switch (rowId)
        {
            case 100000:
                logItem("\n<> Gatekeeper Gostoc");
                break;
            case 100100:
                logItem("\n<> Patches");
                break;
            case 100325:
                logItem("\n<> Pidia Carian Servant");
                break;
            case 100500:
                logItem("\n<> Merchant Kale");
                break;
            case 100525:
                logItem("\n<> Merchant - North Limgrave (Saintsbridge)");
                break;
            case 100550:
                logItem("\n<> Merchant - East Limgrave (Fort Haight)");
                break;
            case 100575:
                logItem("\n<> Merchant - West Limgrave (Coastal Cave)");
                break;
            case 100600:
                logItem("\n<> Merchant - Castle Morne Rampart");
                break;
            case 100625:
                logItem("\n<> Merchant - Liurnia of the Lakes");
                break;
            case 100650:
                logItem("\n<> Isolated Merchant - Weeping Peninsula");
                break;
            case 100700:
                logItem("\n<> Merchant - Bellum Church");
                break;
            case 100725:
                logItem("\n<> Hermit Merchant - Leyndell");
                break;
            case 100750:
                logItem("\n<> Merchant - Altus Plateau");
                break;
            case 100875:
                logItem("\n<> Isolated Merchant - Dragonbarrow");
                break;
            case 100925:
                logItem("\n<> Merchant - Siofra River");
                break;
            case 101800:
                logItem("\n<> Twin Maiden Husks");
                break;
            case 101900:
                logItem("\n<> Remembrances");
                break;
        }
    }
    void logShopIdMagic(int rowId)
    {
        switch (rowId)
        {
            case 100050:
                logItem("\n~* Sorceress Sellen");
                break;
            case 100056:
                logItem("\n~* Sorceress Sellen - Quest");
                break;
            case 100057:
                logItem("\n~* Sorceress Sellen - Conspectus Scroll");
                break;
            case 100059:
                logItem("\n~* Sorceress Sellen -  Academy Scroll");
                break;
            case 100061:
                logItem("\n~* Sorceress Sellen");
                break;
            case 100126:
                logItem("\n~* D Hunter of The Dead");
                break;
            case 100175:
                logItem("\n~* Gowry");
                break;
            case 100250:
                logItem("\n~* Preceptor Seluvis");
                break;
            case 100300:
                logItem("\n~* Preceptor Seluvis - Ranni Quest");
                break;
            case 100310:
                logItem("\n~* Preceptor Seluvis - Dung Eater Quest");
                break;
            case 100350:
                logItem("\n~* Brother Corhyn");
                break;
            case 100358:
                logItem("\n~* Brother Corhyn - Altus Plateau");
                break;
            case 100360:
                logItem("\n~* Brother Corhyn - Goldmask");
                break;
            case 100361:
                logItem("\n~* Brother Corhyn - Erdtree Sanctuary");
                break;
            case 100362:
                logItem("\n~* Brother Corhyn - Fire Monks' Prayerbook");
                break;
            case 100364:
                logItem("\n~* Brother Corhyn - Giant's Prayerbook");
                break;
            case 100368:
                logItem("\n~* Brother Corhyn - Two Fingers' Prayerbook");
                break;
            case 100370:
                logItem("\n~* Brother Corhyn - Assassin's Prayerbook");
                break;
            case 100372:
                logItem("\n~* Brother Corhyn - Golden Order Principia");
                break;
            case 100374:
                logItem("\n~* Brother Corhyn - Dragon Cult Prayerbook");
                break;
            case 100377:
                logItem("\n~* Brother Corhyn - Ancient Dragon Prayerbook");
                break;
            case 100400:
                logItem("\n~* Miriel");
                break;
            case 100402:
                logItem("\n~* Miriel - Conspectus Scroll");
                break;
            case 100404:
                logItem("\n~* Miriel - Academy Scroll");
                break;
            case 100406:
                logItem("\n~* Miriel");
                break;
            case 100426:
                logItem("\n~* Miriel - Fire Monks' Prayerbook");
                break;
            case 100429:
                logItem("\n~* Miriel - Giant's Prayerbook");
                break;
            case 100433:
                logItem("\n~* Miriel - Two Fingers' Prayerbook");
                break;
            case 100435:
                logItem("\n~* Miriel - Assassin's Prayerbook");
                break;
            case 100437:
                logItem("\n~* Miriel - Golden Order Principia");
                break;
            case 100439:
                logItem("\n~* Miriel - Dragon Cult Prayerbook");
                break;
            case 100442:
                logItem("\n~* Miriel - Ancient Dragon Prayerbook");
                break;
            case 101905:
                logItem("\n~* Remembrance");
                break;
            case 101950:
                logItem("\n~* Dragon Communion");
                break;
        }
    }
}