using Project.Params;
using Project.Settings;
using Project.Utility;
using FSParam;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Project.Tasks;

public partial class Randomizer
{
    private BND4 _regulationBnd;
    private string createSeed() { return Guid.NewGuid().ToString(); }
    private static int hashStringToInteger(string input)
    {
        byte[] array = Encoding.UTF8.GetBytes(input);
        byte[] hashData = SHA256.HashData(array);
        IEnumerable<byte[]> chunks = hashData.Chunk(4); // if we have a toggle for smithing cost, could choose different range of chunks, however what would the step be if there are more toggles?
        return chunks.Aggregate(0, (current, chunk) => current ^ BitConverter.ToInt32(chunk));
    }
    private void allocateStatsAndSpells(int rowId, CharaInitParam startingClass)
    {
        switch (rowId)
        {
            case 3000:
                setClassStats(startingClass);
                break;
            case 3001:
                setClassStats(startingClass);
                break;
            case 3002:
                setClassStats(startingClass);
                break;
            case 3003:
                setClassStats(startingClass);
                break;
            case 3004:
                setClassStats(startingClass);
                break;
            case 3005:
                setClassStats(startingClass);
                break;
            case 3006:
                setConfessorStats(startingClass);
                guaranteeIncantations(startingClass, Equipment.StartingIncantationIDs);
                break;
            case 3007:
                setClassStats(startingClass);
                break;
            case 3008:
                setPrisonerStats(startingClass);
                guaranteeSorceries(startingClass, Equipment.StartingSorceryIDs);
                break;
            case 3009:
                setClassStats(startingClass);
                break;
        }
    }
    private void guaranteeSorceries(CharaInitParam chr, IReadOnlyList<int> spells)
    {
        if (hasSpellOfType(chr, Const.SorceryType)) { return; }

        chr.equipSpell01 = Const.NoItem;
        chr.equipSpell02 = Const.NoItem;
        randomizeSorceries(chr, spells);
    }
    private void guaranteeIncantations(CharaInitParam chr, IReadOnlyList<int> spells)
    {
        if (hasSpellOfType(chr, Const.IncantationType)) { return; }

        chr.equipSpell01 = Const.NoItem;
        chr.equipSpell02 = Const.NoItem;
        randomizeIncantations(chr, spells);
    }
    private Dictionary<int, ItemLotEntry> getReplacementHashmap(IOrderedDictionary orderedDictionary)
    {
        Dictionary<int, ItemLotEntry> dict = new();

        // consolidate all ranged armamets
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

        for (int i = 0; i < orderedDictionary.Count; i++)
        {
            List<ItemLotEntry> value = (List<ItemLotEntry>)orderedDictionary[i]!;
            List<ItemLotEntry> itemLotEntries = new(value);
            itemLotEntries.Shuffle(_random);

            foreach (ItemLotEntry entry in itemLotEntries) { dict.Add(entry.Id, getNewId(entry.Id, value)); }
        }
        return dict;
    }
    private Dictionary<int, int> getShopReplacementHashmap(IOrderedDictionary orderedDictionary)
    {
        Dictionary<int, int> output = new();
        for (int i = 0; i < orderedDictionary.Count; i++)
        {
            List<int> value = (List<int>)orderedDictionary[i]!;
            List<int> itemLotEntries = new(value);
            itemLotEntries.Shuffle(_random); // TODO investigate if thise matters

            foreach (int entry in itemLotEntries) { output.Add(entry, getNewId(entry, value)); }
        }
        return output;
    }
    private void dedupeAndRandomizeVectors(IOrderedDictionary orderedDictionary)
    {
        for (int i = 0; i < orderedDictionary.Count; i++)
        {
            List<ItemLotEntry> values = (List<ItemLotEntry>)orderedDictionary[i]!;
            List<ItemLotEntry> distinct = values.Distinct().ToList();
            distinct.Shuffle(_random); // TODO investigate if thise matters
            orderedDictionary[i] = distinct;
        }
    }
    private void dedupeAndRandomizeShopVectors(IOrderedDictionary orderedDictionary)
    {
        for (int i = 0; i < orderedDictionary.Count; i++)
        {
            List<int> values = (List<int>)orderedDictionary[i]!;
            List<int> distinct = values.Distinct().ToList();
            distinct.Shuffle(_random); // TODO investigate if thise matters
            orderedDictionary[i] = distinct;
        }
    }

    private void replaceShopLineupParamMagic(ShopLineupParam lot, IReadOnlyDictionary<int, int> shopLineupParamDictionary, IList<ShopLineupParam> shopLineupParamRemembranceList)
    {
        if (lot.mtrlId == -1)
        {
            int newItem = shopLineupParamDictionary[lot.equipId];
            // logItem($"{_goodsFmg[lot.equipId]} --> {_goodsFmg[newItem]}");
            lot.equipId = newItem;
            return;
        }
        ShopLineupParam newRemembrance = getNewId(lot.equipId, shopLineupParamRemembranceList);
        // logItem($"{_goodsFmg[lot.equipId]} --> {_goodsFmg[newRemembrance.equipId]}");
        copyShopLineupParam(lot, newRemembrance);
    }
    private void addDescriptionString(CharaInitParam chr, int id)
    {
        List<string> str = new() {
            $"{Equipment.EquipmentNameList[chr.wepleft]}{getRequiredLevelsWeapon(chr, chr.wepleft)}",
            $"{Equipment.EquipmentNameList[chr.wepRight]}{getRequiredLevelsWeapon(chr, chr.wepRight)}",
        };
        if (chr.subWepLeft != Const.NoItem)
        { str.Add($"{Equipment.EquipmentNameList[chr.subWepLeft]}{getRequiredLevelsWeapon(chr, chr.subWepLeft)}"); }

        if (chr.subWepRight != Const.NoItem)
        { str.Add($"{Equipment.EquipmentNameList[chr.subWepRight]}{getRequiredLevelsWeapon(chr, chr.subWepRight)}"); }

        if (chr.subWepLeft3 != Const.NoItem)
        { str.Add($"{Equipment.EquipmentNameList[chr.subWepLeft3]}{getRequiredLevelsWeapon(chr, chr.subWepLeft3)}"); }

        if (chr.subWepRight3 != Const.NoItem)
        { str.Add($"{Equipment.EquipmentNameList[chr.subWepRight3]}{getRequiredLevelsWeapon(chr, chr.subWepRight3)}"); }

        if (chr.equipArrow != Const.NoItem)
        { str.Add($"{_weaponNameDictionary[chr.equipArrow]}[{chr.arrowNum}]"); }

        if (chr.equipSubArrow != Const.NoItem)
        { str.Add($"{_weaponNameDictionary[chr.equipSubArrow]}[{chr.subArrowNum}]"); }

        if (chr.equipBolt != Const.NoItem)
        { str.Add($"{_weaponNameDictionary[chr.equipBolt]}[{chr.boltNum}]"); }

        if (chr.equipSubBolt != Const.NoItem)
        { str.Add($"{_weaponNameDictionary[chr.equipSubBolt]}[{chr.subBoltNum}]"); }

        if (chr.equipSpell01 != Const.NoItem)
        { str.Add($"{_goodsFmg[chr.equipSpell01]}"); }

        if (chr.equipSpell02 != Const.NoItem)
        { str.Add($"{_goodsFmg[chr.equipSpell02]}"); }

        _lineHelpFmg[id] = string.Join(", ", str);
    }
    private void writeFiles()
    {
        if (Directory.Exists(Const.BingoPath))
        { Directory.Delete(Const.BingoPath, true); }

        Directory.CreateDirectory(Path.GetDirectoryName($"{Const.BingoPath}/{Const.RegulationName}") ?? throw new InvalidOperationException());
        setBndFile(_regulationBnd, Const.CharaInitParamName, _charaInitParam.Write());
        setBndFile(_regulationBnd, Const.ItemLotParam_mapName, _itemLotParam_map.Write());
        setBndFile(_regulationBnd, Const.ItemLotParam_enemyName, _itemLotParam_enemy.Write());
        setBndFile(_regulationBnd, Const.ShopLineupParamName, _shopLineupParam.Write());
        setBndFile(_regulationBnd, Const.EquipParamWeaponName, _equipParamWeapon.Write());
        setBndFile(_regulationBnd, Const.AtkParamPcName, _atkParam_Pc.Write());
        setBndFile(_regulationBnd, Const.EquipMtrlSetParam, _equipMtrlSetParam.Write());
        SFUtil.EncryptERRegulation($"{Const.BingoPath}/{Const.RegulationName}", _regulationBnd);
        // create menu message for starting classes
        Directory.CreateDirectory(Path.GetDirectoryName($"{Const.BingoPath}/{Const.MenuMsgBNDPath}") ?? throw new InvalidOperationException());
        setBndFile(_menuMsgBND, Const.GR_LineHelpName, _lineHelpFmg.Write()); // TODO why isn't this updating starting classes ?
        File.WriteAllBytes($"{Const.BingoPath}/{Const.MenuMsgBNDPath}", _menuMsgBND.Write());
    }

    private string getRequiredLevelsWeapon(CharaInitParam chr, int id)
    {   // TODO reimplement to account for DLC gear
        string response = "";
        EquipParamWeapon wep = _weaponDictionary[id];
        int reqLevels = 0;

        if (wep.properStrength > (chr.baseStr * 3 / 2))
        { reqLevels += wep.properStrength - (chr.baseStr * 3 / 2); }

        if (wep.properAgility > chr.baseDex)
        { reqLevels += wep.properAgility - chr.baseDex; }

        if (wep.properMagic > chr.baseMag)
        { reqLevels += wep.properMagic - chr.baseMag; }

        if (wep.properFaith > chr.baseFai)
        { reqLevels += wep.properFaith - chr.baseFai; }

        if (wep.properLuck > chr.baseLuc)
        { reqLevels += wep.properLuck - chr.baseLuc; }

        return reqLevels > 0 ? $"({reqLevels})" : response;
    }
    private string getRequiredLevelsSpell(CharaInitParam chr, int id)
    {
        Magic spell = _magicDictionary[id];
        int reqLevels = 0;

        if (spell.requirementIntellect > chr.baseMag)
        { reqLevels += spell.requirementIntellect - chr.baseMag; }

        if (spell.requirementFaith > chr.baseFai)
        { reqLevels += spell.requirementFaith - chr.baseFai; }

        if (spell.requirementLuck > chr.baseLuc)
        { reqLevels += spell.requirementLuck - chr.baseLuc; }

        return reqLevels > 0 ? $" (-{reqLevels})" : "";
    }

    private static T getNewId<T>(int oldId, IList<T> queue) where T : IEquatable<int>
    {   // used to allocate shop items
        if (queue.All(i => i.Equals(oldId)))
        {
            Debug.WriteLine($"No New Ids for {oldId}");
            return queue.Pop();
        }

        T newId = queue.Pop();
        while (newId.Equals(oldId))
        {   // does not allow original weapon at shop slot
            queue.Insert(0, newId);
            newId = queue.Pop();
        }
        return newId;
    }
    // ReSharper disable once SuggestBaseTypeForParameter
    private static void addToOrderedDict<T>(IOrderedDictionary orderedDict, object key, T type)
    {
        List<T>? ids = (List<T>?)orderedDict[key];
        if (ids != null)
        { ids.Add(type); }
        else
        {
            ids = new List<T> { type, };
            orderedDict.Add(key, ids);
        }
    }
    private static bool chrCanUseWeapon(EquipParamWeapon wep, CharaInitParam chr)
    {
        return wep.properStrength <= chr.baseStr
            && wep.properAgility <= chr.baseDex
            && wep.properMagic <= chr.baseMag
            && wep.properFaith <= chr.baseFai
            && wep.properLuck <= chr.baseLuc;
    }
    private static bool chrCanUseSpell(Magic spell, CharaInitParam chr)
    {
        return spell.requirementIntellect <= chr.baseMag
            && spell.requirementFaith <= chr.baseFai
            && spell.requirementLuck <= chr.baseLuc;
    }
    private static void setBndFile(IBinder binder, string fileName, byte[] bytes)
    {
        BinderFile file = binder.Files.First(file => file.Name.EndsWith(fileName)) ?? throw new BinderFileNotFoundException(fileName);
        file.Bytes = bytes;
    }
    private static void patchSpEffectAtkPowerCorrectRate(AtkParam atkParam)
    {
        atkParam.spEffectAtkPowerCorrectRate_byPoint = 100;
        atkParam.spEffectAtkPowerCorrectRate_byRate = 100;
        atkParam.spEffectAtkPowerCorrectRate_byDmg = 100;
    }
    private static void copyShopLineupParam(ShopLineupParam lot, ShopLineupParam shopLineupParam)
    {
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
    private static int washWeaponMetadata(int id) { return id / 10000 * 10000; }
    private static int washWeaponLevels(int id) { return id / 100 * 100; }

    private void injectAdditionalWeaponNames()
    {
        // no affinity
        _weaponFmg[14510000] = "Death Knight's Twin Axes";
        _weaponFmg[14540000] = "Forked-Tongue Hatchet";
        _weaponFmg[64520000] = "Curseblade's Cirque";
        _weaponFmg[8520000] = "Horned Warrior's Greatsword";
        _weaponFmg[22500000] = "Claws of Night";
        _weaponFmg[68510000] = "Red Bear's Claw";
        _weaponFmg[4500000] = "Ancient Meteoric Ore Greatsword";
        _weaponFmg[4540000] = "Moonrithyll's Knight Sword";
        _weaponFmg[23500000] = "Devonia's Hammer";
        _weaponFmg[12510000] = "Anvil Hammer";
        _weaponFmg[12530000] = "Bloodfiend's Arm";
        _weaponFmg[7500000] = "Spirit Sword";
        _weaponFmg[7510000] = "Falx";
        _weaponFmg[7520000] = "Dancing Blade of Ranah";
        _weaponFmg[7530000] = "Horned Warrior's Sword";
        _weaponFmg[21520000] = "Poisoned Hand";
        _weaponFmg[66510000] = "Dragon-Hunter's Great Katana";
        _weaponFmg[66520000] = "Rakshasa's Great Katana";
        _weaponFmg[15500000] = "Death Knight's Longhaft Axe";
        _weaponFmg[16550000] = "Bloodfiend's Sacred Spear";
        _weaponFmg[17520000] = "Barbed Staff-Spear";
        _weaponFmg[3550000] = "Greatsword of Solitude";
        _weaponFmg[18500000] = "Spirit Glaive";
        _weaponFmg[11500000] = "Flowerstone Gavel";
        _weaponFmg[60500000] = "Dryleaf Arts";
        _weaponFmg[60510000] = "Dane's Footwork";
        _weaponFmg[2520000] = "Star-Lined Sword";
        _weaponFmg[9500000] = "Sword of Night";
        _weaponFmg[19500000] = "Obsidian Lamina";
        _weaponFmg[67510000] = "Leda's Sword";
        _weaponFmg[16520000] = "Swift Spear";
        _weaponFmg[16540000] = "Bloodfiend's Fork";
        _weaponFmg[2540000] = "Stone-Sheathed Sword";
        _weaponFmg[2550000] = "Sword of Light";
        _weaponFmg[2560000] = "Sword of Darkness";
        _weaponFmg[2530000] = "Carian Sorcery Sword";
        _weaponFmg[10500000] = "Euporia";
        _weaponFmg[31530000] = "Golden Lion Shield";
        _weaponFmg[62500000] = "Dueling Shield";
        _weaponFmg[62510000] = "Carian Thrusting Shield";
        _weaponFmg[13500000] = "Serpent Flail";
        _weaponFmg[41510000] = "Ansbach's Bow";
        _weaponFmg[43500000] = "Repeating Crossbow";
        _weaponFmg[43510000] = "Spread Crossbow";
        _weaponFmg[44500000] = "Rabbath's Cannon";
        _weaponFmg[42500000] = "Igon's Greatbow";
        _weaponFmg[40500000] = "Bone Bow";

        // affinities
        for (int i = 0; i < 1200; i += 100)
        {
            _weaponFmg[i + 64500000] = "Backhand Blade";
            _weaponFmg[i + 8510000] = "Freyja's Greatsword";
            _weaponFmg[i + 4520000] = "Fire Knight's Greatsword";
            _weaponFmg[i + 1510000] = "Fire Knight Shortsword";
            _weaponFmg[i + 1500000] = "Main-gauche";
            _weaponFmg[i + 21510000] = "Pata";
            _weaponFmg[i + 21540000] = "Golem Fist";
            _weaponFmg[i + 66500000] = "Great Katana";
            _weaponFmg[i + 3520000] = "Lizard Greatsword";
            _weaponFmg[i + 6500000] = "Queelign's Greatsword";
            _weaponFmg[i + 67500000] = "Milady";
            _weaponFmg[i + 12520000] = "Black Steel Greathammer";
            _weaponFmg[i + 10510000] = "Black Steel Twinblade";
            _weaponFmg[i + 14520000] = "Messmer Soldier's Axe";
            _weaponFmg[i + 16520000] = "Swift Spear";
            _weaponFmg[i + 16540000] = "Bloodfiend's Fork";
            _weaponFmg[i + 68500000] = "Beast Claw";
        }
    }
}