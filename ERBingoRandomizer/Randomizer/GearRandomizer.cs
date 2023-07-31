using ERBingoRandomizer.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using static ERBingoRandomizer.Utility.Const;
using static ERBingoRandomizer.Utility.Config;
using static FSParam.Param;

namespace ERBingoRandomizer.Randomizer;

public partial class BingoRandomizer {
    private int getRandomWeapon(int id, List<int> weapons) {
        while (true) {
            int newWeapon = weapons[_random.Next(weapons.Count)];
            if (_weaponDictionary.ContainsKey(newWeapon) && newWeapon != id) {
                return removeWeaponMetadata(newWeapon);
            }
        }
    }
    private int chanceGetRandomWeapon(int id, List<int> weapons) {
        if (ReturnNoItem(id)) {
            return NoItem;
        }

        return getRandomWeapon(id, weapons);
    }
    private int chanceGetRandomArmor(int id, byte type) {
        if (ReturnNoItem(id)) {
            return NoItem;
        }

        return getRandomArmor(id, type);
    }
    private int getRandomArmor(int id, byte type) {
        while (true) {
            List<Row> legs = _armorTypeDictionary[type];
            int newLegs = legs[_random.Next(legs.Count)].ID;
            if (newLegs != id) {
                return newLegs;
            }
        }
    }
    private bool ReturnNoItem(int id) {
        float target = _random.NextSingle();

        // If the entry is -1, return -1 99.99% of the time. If it's not, return -1 0.01% of the time
        // This makes it a small chance for a no item to become an item, and a small chance for an item to become no item.
        if (id == -1) {
            if (target > AddRemoveWeaponChance) {
                return true;
            }
        }
        else {
            if (target < AddRemoveWeaponChance) {
                return true;
            }
        }

        return false;
    }
    private bool hasWeaponOfType(CharaInitParam chr, params ushort[] types) {
        if (types == null || types.Length < 1) {
            throw new ArgumentException("types cannot be null, and must contain 1 or more values. Please pass in a valid weapon type." , nameof(types));
        }
        
        if (checkWeaponType(chr.wepRight, types)) {
            return true;
        }
        if (checkWeaponType(chr.wepleft, types)) {
            return true;
        }
        if (checkWeaponType(chr.subWepLeft, types)) {
            return true;
        }
        if (checkWeaponType(chr.subWepRight, types)) {
            return true;
        }
        if (checkWeaponType(chr.subWepLeft3, types)) {
            return true;
        }
        if (checkWeaponType(chr.subWepRight3, types)) {
            return true;
        }

        return false;
    }
    private bool checkWeaponType(int id, params ushort[] types) {
        if (id == -1) {
            return false;
        }
        if (_weaponDictionary.TryGetValue(id, out EquipParamWeapon? wep)) {
            return types.Contains(wep.wepType);
        }

        return false;
    }
    private bool hasSpellOfType(CharaInitParam chr, params byte[] types) {
        if (types == null || types.Length < 1) {
            throw new ArgumentException("types cannot be null, and must contain 1 or more values. Please pass in a valid weapon type." , nameof(types));
        }
        
        if (checkSpellType(chr.equipSpell01, types)) {
            return true;
        }
        if (checkSpellType(chr.equipSpell02, types)) {
            return true;
        }

        return false;
    }
    private bool checkSpellType(int id, params byte[] types) {
        if (id == -1) {
            return false;
        }
        return _magicDictionary.TryGetValue(id, out Magic? magic) && types.Contains(magic.ezStateBehaviorType);

    }
    private void giveArrows(CharaInitParam chr) {
        chr.equipArrow = getRandomAmmo(ArrowType);
        chr.arrowNum = (ushort)(_random.Next() % MaxArrows);
    }
    private void giveGreatArrows(CharaInitParam chr) {
        chr.equipSubArrow = getRandomAmmo(GreatArrowType);
        chr.subArrowNum = (ushort)(_random.Next() % MaxGreatArrows);
    }
    private void giveBolts(CharaInitParam chr) {
        chr.equipBolt = getRandomAmmo(BoltType);
        chr.boltNum = (ushort)(_random.Next() % MaxBolts);
    }
    private void giveBallistaBolts(CharaInitParam chr) {
        chr.equipSubBolt = getRandomAmmo(BallistaBoltType);
        chr.subBoltNum = (ushort)(_random.Next() % MaxBallistaBolts);
    }
    private int getRandomAmmo(ushort type) {
        List<Row> arrows = _weaponTypeDictionary[type];
        return arrows[_random.Next() % arrows.Count].ID;
    }
    private void randomizeSorceries(CharaInitParam chr, List<int> spells) {
        chr.equipSpell01 = getRandomMagic(chr, SorceryType, spells);
        if (chr.equipSpell02 == -1) {
            chr.equipSpell02 = chanceRandomMagic(chr.equipSpell02, chr, SorceryType, spells);
        }
        giveRandomWeapon(chr, StaffType);
    }
    private void randomizeIncantations(CharaInitParam chr, List<int> spells) {
        chr.equipSpell02 = getRandomMagic(chr, IncantationType, spells);
        if (chr.equipSpell01 == -1) {
            chr.equipSpell01 = chanceRandomMagic(chr.equipSpell01, chr, IncantationType, spells);
        }
        giveRandomWeapon(chr, SealType);
    }
    private int getRandomMagic(CharaInitParam chr, byte type, List<int> spells) {
        List<Row> table = _magicTypeDictionary[type];
        while (true) {
            int i = _random.Next() % table.Count;
            Magic entry = _magicDictionary[table[i].ID];
            if (chrCanUseSpell(entry, chr) && spells.Contains(table[i].ID)) {
                return table[i].ID;
            }
        }
    }
    private int chanceRandomMagic(int id, CharaInitParam chr, byte type, List<int> spells) {
        if (ReturnNoItem(id)) {
            return -1;
        }

        return getRandomMagic(chr, type, spells);
    }
    private void giveRandomWeapon(CharaInitParam chr, ushort type) {
        EquipParamWeapon? wep;
        if (_weaponDictionary.TryGetValue(chr.wepleft, out wep)) {
            if (wep.wepType == type && chrCanUseWeapon(wep, chr)) {
                return;
            }
        }
        else {
            chr.wepleft = getRandomWeapon(chr, type);
            return;
        }
        
        if (_weaponDictionary.TryGetValue(chr.wepRight, out wep)) {
            if (wep.wepType == type && chrCanUseWeapon(wep, chr)) {
                return;
            }
        }
        else {
            chr.wepRight = getRandomWeapon(chr, type);
            return;
        }
        
        if (_weaponDictionary.TryGetValue(chr.subWepLeft, out wep)) {
            if (wep.wepType == type && chrCanUseWeapon(wep, chr)) {
                return;
            }
        }
        else {
            chr.subWepLeft = getRandomWeapon(chr, type);
            return;
        }
        
        if (_weaponDictionary.TryGetValue(chr.subWepRight, out wep)) {
            if (wep.wepType == type && chrCanUseWeapon(wep, chr)) {
                return;
            }
        }
        else {
            chr.subWepRight = getRandomWeapon(chr, type);
            return;
        }
        
        if (_weaponDictionary.TryGetValue(chr.subWepLeft3, out wep)) {
            if (wep.wepType == type && chrCanUseWeapon(wep, chr)) {
                return;
            }
        }
        else {
            chr.subWepLeft3 = getRandomWeapon(chr, type);
            return;
        }
        
        chr.subWepRight3 = getRandomWeapon(chr, type);
    }
    private int getRandomWeapon(CharaInitParam chr, ushort type) {
        List<Row> table = _weaponTypeDictionary[type];
        while (true) {
            int i = _random.Next() % table.Count;
            EquipParamWeapon? entry;
            if (_weaponDictionary.TryGetValue(table[i].ID, out entry)) {
                if (chrCanUseWeapon(entry, chr)) {
                    return table[i].ID;
                }
                continue;
            }

            entry = _customWeaponDictionary[table[i].ID];
            if (chrCanUseWeapon(entry, chr)) {
                return table[i].ID;
            }
        }
    }
}
