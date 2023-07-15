using ERBingoRandomizer.Params;
using ERBingoRandomizer.Utility;
using System.Collections.Generic;
using System.Linq;
using static ERBingoRandomizer.Utility.Const;
using static ERBingoRandomizer.Utility.Config;
using static FSParam.Param;

namespace ERBingoRandomizer.Randomizer;

public partial class BingoRandomizer {
    private int getRandomWeapon(int id) {
        while (true) {
            int newWeapon = _weaponDictionary.Keys.ElementAt(_random.Next(_weaponDictionary.Keys.Count));
            if (_weaponDictionary.ContainsKey(newWeapon) && newWeapon != id) {
                return newWeapon;
            }
        }
    }
    private int chanceGetRandomWeapon(int id) {
        if (ReturnNoItem(id)) {
            return NoItem;
        }

        return getRandomWeapon(id);
    }
    private int chanceGetRandomHelm(int id) {
        if (ReturnNoItem(id)) {
            return NoItem;
        }

        return getRandomArmor(id, HelmType);
    }
    private int chanceGetRandomBody(int id) {
        if (ReturnNoItem(id)) {
            return NoItem;
        }

        return getRandomArmor(id, BodyType);
    }
    private int chanceGetRandomArms(int id) {
        if (ReturnNoItem(id)) {
            return NoItem;
        }

        return getRandomArmor(id, ArmType);
    }
    private int chanceGetRandomLegs(int id) {
        if (ReturnNoItem(id)) {
            return NoItem;
        }

        return getRandomArmor(id, LegType);
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
            if (target > Chance) {
                return true;
            }
        }
        else {
            if (target < Chance) {
                return true;
            }
        }

        return false;
    }
    private bool hasBow(CharaInitParam chr) {
        if (checkWeaponType(chr.wepRight, BowType, LightBowType)) {
            return true;
        }
        if (checkWeaponType(chr.wepleft, BowType, LightBowType)) {
            return true;
        }
        if (checkWeaponType(chr.subWepLeft, BowType, LightBowType)) {
            return true;
        }
        if (checkWeaponType(chr.subWepRight, BowType, LightBowType)) {
            return true;
        }
        if (checkWeaponType(chr.subWepLeft3, BowType, LightBowType)) {
            return true;
        }
        if (checkWeaponType(chr.subWepRight3, BowType, LightBowType)) {
            return true;
        }

        return false;
    }
    private bool hasGreatbow(CharaInitParam chr) {
        if (checkWeaponType(chr.wepRight, GreatbowType)) {
            return true;
        }
        if (checkWeaponType(chr.wepleft, GreatbowType)) {
            return true;
        }
        if (checkWeaponType(chr.subWepLeft, GreatbowType)) {
            return true;
        }
        if (checkWeaponType(chr.subWepRight, GreatbowType)) {
            return true;
        }
        if (checkWeaponType(chr.subWepLeft3, GreatbowType)) {
            return true;
        }
        if (checkWeaponType(chr.subWepRight3, GreatbowType)) {
            return true;
        }

        return false;
    }
    private bool hasCrossbow(CharaInitParam chr) {
        if (checkWeaponType(chr.wepRight, CrossbowType)) {
            return true;
        }
        if (checkWeaponType(chr.wepleft, CrossbowType)) {
            return true;
        }
        if (checkWeaponType(chr.subWepLeft, CrossbowType)) {
            return true;
        }
        if (checkWeaponType(chr.subWepRight, CrossbowType)) {
            return true;
        }
        if (checkWeaponType(chr.subWepLeft3, CrossbowType)) {
            return true;
        }
        if (checkWeaponType(chr.subWepRight3, CrossbowType)) {
            return true;
        }

        return false;
    }
    private bool hasBallista(CharaInitParam chr) {
        if (checkWeaponType(chr.wepRight, BallistaType)) {
            return true;
        }
        if (checkWeaponType(chr.wepleft, BallistaType)) {
            return true;
        }
        if (checkWeaponType(chr.subWepLeft, BallistaType)) {
            return true;
        }
        if (checkWeaponType(chr.subWepRight, BallistaType)) {
            return true;
        }
        if (checkWeaponType(chr.subWepLeft3, BallistaType)) {
            return true;
        }
        if (checkWeaponType(chr.subWepRight3, BallistaType)) {
            return true;
        }

        return false;
    }
    private bool checkWeaponType(int id, params ushort[] types) {
        if (id != -1) {
            return false;
        }
        if (_weaponDictionary.TryGetValue(id, out EquipParamWeapon wep)) {
            return types.Contains(wep.wepType);
        }

        return false;
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
        chr.subBoltNum = (ushort)(_random.Next() % MaxBolts);
    }
    private int getRandomAmmo(ushort type) {
        var arrows = _weaponTypeDictionary[type];
        return arrows[_random.Next() % arrows.Count].ID;
    }
    private void randomizeSorceries(CharaInitParam chr) {
        chr.equipSpell01 = getRandomMagic(chr, SorceryType);
        if (chr.equipSpell02 == -1) {
            chr.equipSpell02 = chanceRandomMagic(chr.equipSpell02, chr, SorceryType);
        }
        giveRandomWeapon(chr, StaffType);
    }
    private void randomizeIncantations(CharaInitParam chr) {
        chr.equipSpell02 = getRandomMagic(chr, IncantationType);
        if (chr.equipSpell01 == -1) {
            chr.equipSpell01 = chanceRandomMagic(chr.equipSpell01, chr, IncantationType);
        }
        giveRandomWeapon(chr, SealType);
    }
    private int getRandomMagic(CharaInitParam chr, byte type) {
        List<Row> table = _magicTypeDictionary[type];
        while (true) {
            var i = _random.Next() % table.Count;
            var entry = _magicDictionary[table[i].ID];
            if (RandoUtil.ChrCanUseSpell(entry, chr)) {
                return table[i].ID;
            }
        }
    }
    private int chanceRandomMagic(int id, CharaInitParam chr, byte type) {
        if (ReturnNoItem(id)) {
            return -1;
        }

        return getRandomMagic(chr, type);
    }
    private void giveRandomWeapon(CharaInitParam chr, ushort type) {
        EquipParamWeapon wep;
        if (_weaponDictionary.TryGetValue(chr.wepleft, out wep)) {
            if (wep.wepType == type && RandoUtil.ChrCanUseWeapon(wep, chr)) {
                return;
            }
        }
        else {
            chr.wepleft = getRandomWeapon(chr, type);
            return;
        }
        if (_weaponDictionary.TryGetValue(chr.wepRight, out wep)) {
            if (wep.wepType == type && RandoUtil.ChrCanUseWeapon(wep, chr)) {
                return;
            }
        }
        else {
            chr.wepRight = getRandomWeapon(chr, type);
            return;
        }
        if (_weaponDictionary.TryGetValue(chr.subWepLeft, out wep)) {
            if (wep.wepType == type && RandoUtil.ChrCanUseWeapon(wep, chr)) {
                return;
            }
        }
        else {
            chr.subWepLeft = getRandomWeapon(chr, type);
            return;
        }
        if (_weaponDictionary.TryGetValue(chr.subWepRight, out wep)) {
            if (wep.wepType == type && RandoUtil.ChrCanUseWeapon(wep, chr)) {
                return;
            }
        }
        else {
            chr.subWepRight = getRandomWeapon(chr, type);
            return;
        }
        if (_weaponDictionary.TryGetValue(chr.subWepLeft3, out wep)) {
            if (wep.wepType == type && RandoUtil.ChrCanUseWeapon(wep, chr)) {
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
            var i = _random.Next() % table.Count;
            var entry = _weaponDictionary[table[i].ID];
            if (RandoUtil.ChrCanUseWeapon(entry, chr)) {
                return table[i].ID;
            }
        }
    }
}
