using ERBingoRandomizer.Params;
using ERBingoRandomizer.Utility;
using FSParam;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using static ERBingoRandomizer.Params.EquipParamWeapon;

namespace ERBingoRandomizer.Randomizer.Strategies.CharaInitParam;

public class Season2ClassRandomizer : IBingoClassStrategy {
    private readonly IBingoLevelStrategy _levelRandomizer;
    private RandoResource _resources;
    
    public Season2ClassRandomizer(IBingoLevelStrategy levelRandomizer, RandoResource resources) {
        _levelRandomizer = levelRandomizer;
        _resources = resources;
    }
    
    public void RandomizeCharaInitParam() {
        Logger.LogItem(">> Class Randomization - All items are randomized, with each class having a .001% chance to gain or lose and item. Spells given class meets min stat requirements");
        Logger.LogItem("> Ammo is give if you get a ranged weapon. Catalyst is give if you have spells.\n");
        IEnumerable<int> remembranceItems = _resources.ShopLineupParam.Rows.Where(r => r.ID is >= 101900 and <= 101929).Select(r => new ShopLineupParam(r).equipId);
        List<Param.Row> staves = _resources.WeaponTypeDictionary[WeaponType.GlintstoneStaff];
        List<Param.Row> seals = _resources.WeaponTypeDictionary[WeaponType.FingerSeal];
        List<int> weapons = _resources.WeaponDictionary.Keys.Select(BingoRandomizer.RemoveWeaponMetadata).Distinct()
            .Where(id => remembranceItems.All(i => i != id))
            .Where(id => staves.All(s => s.ID != id) && seals.All(s => s.ID != id))
            .ToList();
        weapons.Shuffle(_resources.Random);

        List<int> spells = _resources.MagicDictionary.Keys.Select(id => id).Distinct()
            .Where(id => remembranceItems.All(r => r != id))
            .Where(id => staves.All(s => s.ID != id) && seals.All(s => s.ID != id)).ToList();
        spells.Shuffle(_resources.Random);

        for (int i = 0; i < 10; i++) {
            Param.Row? row = _resources.CharaInitParam[i + 3000];
            if (row == null) {
                continue;
            }
            Params.CharaInitParam param = new(row);
            randomizeCharaInitEntry(param, weapons);
            guaranteeSpellcasters(row.ID, param, spells);
            logCharaInitEntry(param, i + 288100);
            addDescriptionString(param, Const.ChrInfoMapping[i]);
        }
        
    }
    
    private void randomizeCharaInitEntry(Params.CharaInitParam chr, IReadOnlyList<int> weapons) {
        chr.wepleft = getRandomWeapon(chr.wepleft, weapons);
        chr.wepRight = getRandomWeapon(chr.wepRight, weapons);
        chr.subWepLeft = -1;
        chr.subWepRight = -1;
        chr.subWepLeft3 = -1;
        chr.subWepRight3 = -1;

        chr.equipHelm = chanceGetRandomArmor(chr.equipHelm, Const.HelmType);
        chr.equipArmer = chanceGetRandomArmor(chr.equipArmer, Const.BodyType);
        chr.equipGaunt = chanceGetRandomArmor(chr.equipGaunt, Const.ArmType);
        chr.equipLeg = chanceGetRandomArmor(chr.equipLeg, Const.LegType);

        _levelRandomizer.RandomizeLevels(chr);

        chr.equipArrow = Const.NoItem;
        chr.arrowNum = ushort.MaxValue;
        if (hasWeaponOfType(chr, WeaponType.Bow, WeaponType.LightBow)) {
            giveArrows(chr);
        }
        chr.equipSubArrow = Const.NoItem;
        chr.subArrowNum = ushort.MaxValue;
        if (hasWeaponOfType(chr, WeaponType.Greatbow)) {
            giveGreatArrows(chr);
        }
        chr.equipBolt = Const.NoItem;
        chr.boltNum = ushort.MaxValue;
        if (hasWeaponOfType(chr, WeaponType.Crossbow)) {
            giveBolts(chr);
        }
        chr.equipSubBolt = Const.NoItem;
        chr.subBoltNum = ushort.MaxValue;
        if (hasWeaponOfType(chr, WeaponType.Ballista)) {
            giveBallistaBolts(chr);
        }

        chr.equipSpell01 = -1;
        chr.equipSpell02 = -1;
    }

    private void guaranteeSpellcasters(int rowId, Params.CharaInitParam chr, IReadOnlyList<int> spells) {
        switch (rowId) {
            case 3008:
                guaranteePrisonerHasSpells(chr, spells);
                break;
            case 3006:
                guaranteeConfessorHasIncantation(chr, spells);
                break;
        }
    }
    private void guaranteePrisonerHasSpells(Params.CharaInitParam chr, IReadOnlyList<int> spells) {
        if (hasSpellOfType(chr, Const.SorceryType)) {
            return;
        }
        // Get a new random chr until it has the required stats.
        while (chr.baseMag < Config.MinInt) {
            _levelRandomizer.RandomizeLevels(chr);
        }

        chr.equipSpell01 = -1;
        chr.equipSpell02 = -1;
        randomizeSorceries(chr, spells);
    }
    private void guaranteeConfessorHasIncantation(Params.CharaInitParam chr, IReadOnlyList<int> spells) {
        if (hasSpellOfType(chr, Const.IncantationType)) {
            return;
        }
        // Get a new random chr until it has the required stats.
        while (chr.baseFai < Config.MinFai) {
            _levelRandomizer.RandomizeLevels(chr);
        }

        chr.equipSpell01 = -1;
        chr.equipSpell02 = -1;
        randomizeIncantations(chr, spells);
    }
    private int getRandomWeapon(int id, IReadOnlyList<int> weapons) {
        while (true) {
            int newWeapon = weapons[_resources.Random.Next(weapons.Count)];
            if (_resources.WeaponDictionary.ContainsKey(newWeapon) && newWeapon != id) {
                return BingoRandomizer.RemoveWeaponMetadata(newWeapon);
            }
        }
    }
    private int chanceGetRandomWeapon(int id, IReadOnlyList<int> weapons) {
        return ReturnNoItem(id) ? Const.NoItem : getRandomWeapon(id, weapons);

    }
    private int chanceGetRandomArmor(int id, byte type) {
        return ReturnNoItem(id) ? Const.NoItem : getRandomArmor(id, type);

    }
    private int getRandomArmor(int id, byte type) {
        while (true) {
            IReadOnlyList<Param.Row> legs = _resources.ArmorTypeDictionary[type];
            int newLegs = legs[_resources.Random.Next(legs.Count)].ID;
            if (newLegs != id) {
                return newLegs;
            }
        }
    }
    private bool ReturnNoItem(int id) {
        float target = _resources.Random.NextSingle();

        // If the entry is -1, return -1 99.99% of the time. If it's not, return -1 0.01% of the time
        // This makes it a small chance for a no item to become an item, and a small chance for an item to become no item.
        if (id == Const.NoItem) {
            if (target > Config.AddRemoveWeaponChance) {
                return true;
            }
        }
        else {
            if (target < Config.AddRemoveWeaponChance) {
                return true;
            }
        }

        return false;
    }
    private bool hasWeaponOfType(Params.CharaInitParam chr, params WeaponType[] types) {
        if (types == null || types.Length < 1) {
            throw new ArgumentException("types cannot be null, and must contain 1 or more values. Please pass in a valid weapon type." , nameof(types));
        }

        return checkWeaponType(chr.wepRight, types) || checkWeaponType(chr.wepleft, types) 
            || checkWeaponType(chr.subWepLeft, types) || checkWeaponType(chr.subWepRight, types)
            || checkWeaponType(chr.subWepLeft3, types) || checkWeaponType(chr.subWepRight3, types);

    }
    private bool checkWeaponType(int id, params WeaponType[] types) {
        if (id == Const.NoItem) {
            return false;
        }
        return _resources.WeaponDictionary.TryGetValue(id, out EquipParamWeapon? wep) && types.Contains(wep.wepType);

    }
    private bool hasSpellOfType(Params.CharaInitParam chr, params byte[] types) {
        if (types == null || types.Length < 1) {
            throw new ArgumentException("types cannot be null, and must contain 1 or more values. Please pass in a valid weapon type." , nameof(types));
        }
        
        return checkSpellType(chr.equipSpell01, types) || checkSpellType(chr.equipSpell02, types);

    }
    private bool checkSpellType(int id, params byte[] types) {
        if (id == Const.NoItem) {
            return false;
        }
        return _resources.MagicDictionary.TryGetValue(id, out Magic? magic) && types.Contains(magic.ezStateBehaviorType);

    }
    private void giveArrows(Params.CharaInitParam chr) {
        chr.equipArrow = getRandomAmmo(WeaponType.Arrow);
        chr.arrowNum = (ushort)(_resources.Random.Next() % Config.MaxArrows);
    }
    private void giveGreatArrows(Params.CharaInitParam chr) {
        chr.equipSubArrow = getRandomAmmo(WeaponType.GreatArrow);
        chr.subArrowNum = (ushort)(_resources.Random.Next() % Config.MaxGreatArrows);
    }
    private void giveBolts(Params.CharaInitParam chr) {
        chr.equipBolt = getRandomAmmo(WeaponType.Bolt);
        chr.boltNum = (ushort)(_resources.Random.Next() % Config.MaxBolts);
    }
    private void giveBallistaBolts(Params.CharaInitParam chr) {
        chr.equipSubBolt = getRandomAmmo(WeaponType.BallistaBolt);
        chr.subBoltNum = (ushort)(_resources.Random.Next() % Config.MaxBallistaBolts);
    }
    private int getRandomAmmo(WeaponType type) {
        IList<Param.Row> arrows = _resources.WeaponTypeDictionary[type];
        return arrows[_resources.Random.Next() % arrows.Count].ID;
    }
    private void randomizeSorceries(Params.CharaInitParam chr, IReadOnlyList<int> spells) {
        chr.equipSpell01 = getRandomMagic(chr, Const.SorceryType, spells);
        if (chr.equipSpell02 == Const.NoItem) {
            chr.equipSpell02 = chanceRandomMagic(chr.equipSpell02, chr, Const.SorceryType, spells);
        }
        giveRandomUsableWeapon(chr, WeaponType.GlintstoneStaff);
    }
    private void randomizeIncantations(Params.CharaInitParam chr, IReadOnlyList<int> spells) {
        chr.equipSpell02 = getRandomMagic(chr, Const.IncantationType, spells);
        if (chr.equipSpell01 == Const.NoItem) {
            chr.equipSpell01 = chanceRandomMagic(chr.equipSpell01, chr, Const.IncantationType, spells);
        }
        giveRandomUsableWeapon(chr, WeaponType.FingerSeal);
    }
    private int getRandomMagic(Params.CharaInitParam chr, byte type, IReadOnlyList<int> spells) {
        IReadOnlyList<Param.Row> table = _resources.MagicTypeDictionary[type];
        while (true) {
            int i = _resources.Random.Next() % table.Count;
            Magic entry = _resources.MagicDictionary[table[i].ID];
            if (chrCanUseSpell(entry, chr) && spells.Contains(table[i].ID)) {
                return table[i].ID;
            }
        }
    }
    private int chanceRandomMagic(int id, Params.CharaInitParam chr, byte type, IReadOnlyList<int> spells) {
        return ReturnNoItem(id) ? Const.NoItem : getRandomMagic(chr, type, spells);

    }
    private void giveRandomUsableWeapon(Params.CharaInitParam chr, WeaponType type) {
        EquipParamWeapon? wep;
        if (_resources.WeaponDictionary.TryGetValue(chr.wepleft, out wep)) {
            if (wep.wepType == type && chrCanUseWeapon(wep, chr)) {
                return;
            }
        }
        else {
            chr.wepleft = getRandomUsableWeapon(chr, type);
            return;
        }
        
        if (_resources.WeaponDictionary.TryGetValue(chr.wepRight, out wep)) {
            if (wep.wepType == type && chrCanUseWeapon(wep, chr)) {
                return;
            }
        }
        else {
            chr.wepRight = getRandomUsableWeapon(chr, type);
            return;
        }
        
        if (_resources.WeaponDictionary.TryGetValue(chr.subWepLeft, out wep)) {
            if (wep.wepType == type && chrCanUseWeapon(wep, chr)) {
                return;
            }
        }
        else {
            chr.subWepLeft = getRandomUsableWeapon(chr, type);
            return;
        }
        
        if (_resources.WeaponDictionary.TryGetValue(chr.subWepRight, out wep)) {
            if (wep.wepType == type && chrCanUseWeapon(wep, chr)) {
                return;
            }
        }
        else {
            chr.subWepRight = getRandomUsableWeapon(chr, type);
            return;
        }
        
        if (_resources.WeaponDictionary.TryGetValue(chr.subWepLeft3, out wep)) {
            if (wep.wepType == type && chrCanUseWeapon(wep, chr)) {
                return;
            }
        }
        else {
            chr.subWepLeft3 = getRandomUsableWeapon(chr, type);
            return;
        }
        
        chr.subWepRight3 = getRandomUsableWeapon(chr, type);
    }
    private int getRandomUsableWeapon(Params.CharaInitParam chr, WeaponType type) {
        IReadOnlyList<Param.Row> table = _resources.WeaponTypeDictionary[type];
        while (true) {
            int i = _resources.Random.Next() % table.Count;
            if (_resources.WeaponDictionary.TryGetValue(table[i].ID, out EquipParamWeapon? entry)) {
                if (chrCanUseWeapon(entry, chr)) {
                    return table[i].ID;
                }
                continue;
            }

            entry = _resources.CustomWeaponDictionary[table[i].ID];
            if (chrCanUseWeapon(entry, chr)) {
                return table[i].ID;
            }
        }
    }
    private static bool chrCanUseWeapon(EquipParamWeapon wep, Params.CharaInitParam chr) {
        return wep.properStrength <= chr.baseStr
            && wep.properAgility <= chr.baseDex
            && wep.properMagic <= chr.baseMag
            && wep.properFaith <= chr.baseFai
            && wep.properLuck <= chr.baseLuc;
    }
    private static bool chrCanUseSpell(Magic spell, Params.CharaInitParam chr) {
        return spell.requirementIntellect <= chr.baseMag
            && spell.requirementFaith <= chr.baseFai
            && spell.requirementLuck <= chr.baseLuc;
    }

    private void logCharaInitEntry(Params.CharaInitParam chr, int i) {
        Logger.LogItem($"\n> {_resources.MenuTextFmg[i]}");
        Logger.LogItem("> Weapons");
        if (chr.wepleft != -1) {
            Logger.LogItem($"Left: {_resources.WeaponFmg[chr.wepleft]}{getRequiredLevelsWeapon(chr, chr.wepleft)}");
        }
        if (chr.wepRight != -1) {
            Logger.LogItem($"Right: {_resources.WeaponFmg[chr.wepRight]}{getRequiredLevelsWeapon(chr, chr.wepRight)}");
        }
        if (chr.subWepLeft != -1) {
            Logger.LogItem($"Left 2: {_resources.WeaponFmg[chr.subWepLeft]}{getRequiredLevelsWeapon(chr, chr.subWepLeft)}");
        }
        if (chr.subWepRight != -1) {
            Logger.LogItem($"Right 2: {_resources.WeaponFmg[chr.subWepRight]}{getRequiredLevelsWeapon(chr, chr.subWepRight)}");
        }
        if (chr.subWepLeft3 != -1) {
            Logger.LogItem($"Left 3: {_resources.WeaponFmg[chr.subWepLeft3]}{getRequiredLevelsWeapon(chr, chr.subWepLeft3)}");
        }
        if (chr.subWepRight3 != -1) {
            Logger.LogItem($"Right 3: {_resources.WeaponFmg[chr.subWepRight3]}{getRequiredLevelsWeapon(chr, chr.subWepRight3)}");
        }

        Logger.LogItem("\n> Armor");
        Logger.LogItem($"Helm: {_resources.ProtectorFmg[chr.equipHelm]}");
        Logger.LogItem($"Body: {_resources.ProtectorFmg[chr.equipArmer]}");
        Logger.LogItem($"Arms: {_resources.ProtectorFmg[chr.equipGaunt]}");
        Logger.LogItem($"Legs: {_resources.ProtectorFmg[chr.equipLeg]}");

        Logger.LogItem("\n> Levels");
        Logger.LogItem($"Vigor: {chr.baseVit}");
        Logger.LogItem($"Attunement: {chr.baseWil}");
        Logger.LogItem($"Endurance: {chr.baseEnd}");
        Logger.LogItem($"Strength: {chr.baseStr}");
        Logger.LogItem($"Dexterity: {chr.baseDex}");
        Logger.LogItem($"Intelligence: {chr.baseMag}");
        Logger.LogItem($"Faith: {chr.baseFai}");
        Logger.LogItem($"Arcane: {chr.baseLuc}");

        if (chr.equipArrow != -1 || chr.equipSubArrow != -1 || chr.equipBolt != -1 || chr.equipSubBolt != -1) {
            Logger.LogItem("\n> Ammo");
            if (chr.equipArrow != -1) {
                Logger.LogItem($"{_resources.WeaponFmg[chr.equipArrow]}[{chr.arrowNum}]");
            }
            if (chr.equipSubArrow != -1) {
                Logger.LogItem($"{_resources.WeaponFmg[chr.equipSubArrow]}[{chr.subArrowNum}]");
            }
            if (chr.equipBolt != -1) {
                Logger.LogItem($"{_resources.WeaponFmg[chr.equipBolt]}[{chr.boltNum}]");
            }
            if (chr.equipSubBolt != -1) {
                Logger.LogItem($"{_resources.WeaponFmg[chr.equipSubBolt]}[{chr.subBoltNum}]");
            }
        }

        if (chr.equipSpell01 != -1 || chr.equipSpell02 != -1) {
            Logger.LogItem("\n> Spells");
            if (chr.equipSpell01 != -1) {
                Logger.LogItem($"{_resources.GoodsFmg[chr.equipSpell01]}{getRequiredLevelsSpell(chr, chr.equipSpell01)}");
            }
            if (chr.equipSpell02 != -1) {
                Logger.LogItem($"{_resources.GoodsFmg[chr.equipSpell02]}{getRequiredLevelsSpell(chr, chr.equipSpell02)}");
            }
        }

        Logger.LogItem("");
    }
    private void addDescriptionString(Params.CharaInitParam chr, int id) {
        List<string> str = new() {
            $"{_resources.WeaponNameDictionary[chr.wepleft]}{getRequiredLevelsWeapon(chr, chr.wepleft)}",
            $"{_resources.WeaponNameDictionary[chr.wepRight]}{getRequiredLevelsWeapon(chr, chr.wepRight)}",
        };
        if (chr.subWepLeft != -1) {
            str.Add($"{_resources.WeaponNameDictionary[chr.subWepLeft]}{getRequiredLevelsWeapon(chr, chr.subWepLeft)}");
        }
        if (chr.subWepRight != -1) {
            str.Add($"{_resources.WeaponNameDictionary[chr.subWepRight]}{getRequiredLevelsWeapon(chr, chr.subWepRight)}");
        }
        if (chr.subWepLeft3 != -1) {
            str.Add($"{_resources.WeaponNameDictionary[chr.subWepLeft3]}{getRequiredLevelsWeapon(chr, chr.subWepLeft3)}");
        }
        if (chr.subWepRight3 != -1) {
            str.Add($"{_resources.WeaponNameDictionary[chr.subWepRight3]}{getRequiredLevelsWeapon(chr, chr.subWepRight3)}");
        }
        if (chr.equipArrow != -1) {
            str.Add($"{_resources.WeaponNameDictionary[chr.equipArrow]}[{chr.arrowNum}]");
        }
        if (chr.equipSubArrow != -1) {
            str.Add($"{_resources.WeaponNameDictionary[chr.equipSubArrow]}[{chr.subArrowNum}]");
        }
        if (chr.equipBolt != -1) {
            str.Add($"{_resources.WeaponNameDictionary[chr.equipBolt]}[{chr.boltNum}]");
        }
        if (chr.equipSubBolt != -1) {
            str.Add($"{_resources.WeaponNameDictionary[chr.equipSubBolt]}[{chr.subBoltNum}]");
        }
        if (chr.equipSpell01 != -1) {
            str.Add($"{_resources.GoodsFmg[chr.equipSpell01]}");
        }
        if (chr.equipSpell02 != -1) {
            str.Add($"{_resources.GoodsFmg[chr.equipSpell02]}");
        }

        _resources.LineHelpFmg[id] = string.Join(", ", str);
    }
    private string getRequiredLevelsWeapon(Params.CharaInitParam chr, int id) {
        EquipParamWeapon wep = _resources.WeaponDictionary[id];
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
    private string getRequiredLevelsSpell(Params.CharaInitParam chr, int id) {
        Magic spell = _resources.MagicDictionary[id];
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

        return reqLevels > 0 ? $" (-{reqLevels})" : "";

    }
}
