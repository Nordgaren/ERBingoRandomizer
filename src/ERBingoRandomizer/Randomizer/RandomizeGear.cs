using ERBingoRandomizer.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using ERBingoRandomizer.Utility;
using FSParam;

namespace ERBingoRandomizer.Randomizer;

public partial class BingoRandomizer
{
    private int getRandomWeapon(int id, IReadOnlyList<int> weapons)
    {
        while (true)
        {
            int newWeapon = weapons[_random.Next(weapons.Count)];
            if (_weaponDictionary.ContainsKey(newWeapon) && newWeapon != id)
            {
                return washWeaponMetadata(newWeapon);
            }
        }
    }
    private int chanceGetRandomWeapon(int id, IReadOnlyList<int> weapons)
    {
        return ReturnNoItem(id) ? Const.NoItem : getRandomWeapon(id, weapons);

    }
    private int chanceGetRandomArmor(int id, byte type)
    {
        return ReturnNoItem(id) ? Const.NoItem : getRandomArmor(id, type);

    }
    private int getRandomArmor(int id, byte type)
    {
        while (true)
        {
            IReadOnlyList<Param.Row> legs = _armorTypeDictionary[type];
            int newLegs = legs[_random.Next(legs.Count)].ID;
            if (newLegs != id)
            {
                return newLegs;
            }
        }
    }
    private bool ReturnNoItem(int id)
    {
        float target = _random.NextSingle();

        // If the entry is -1, return -1 99.99% of the time. If it's not, return -1 0.01% of the time
        // This makes it a small chance for a no item to become an item, and a small chance for an item to become no item.
        if (id == Const.NoItem)
        {
            if (target > Config.AddRemoveWeaponChance)
            {
                return true;
            }
        }
        else
        {
            if (target < Config.AddRemoveWeaponChance)
            {
                return true;
            }
        }

        return false;
    }
    private bool hasWeaponOfType(CharaInitParam chr, params ushort[] types)
    {
        if (types == null || types.Length < 1)
        {
            throw new ArgumentException("types cannot be null, and must contain 1 or more values. Please pass in a valid weapon type.", nameof(types));
        }

        return checkWeaponType(chr.wepRight, types) || checkWeaponType(chr.wepleft, types)
            || checkWeaponType(chr.subWepLeft, types) || checkWeaponType(chr.subWepRight, types)
            || checkWeaponType(chr.subWepLeft3, types) || checkWeaponType(chr.subWepRight3, types);

    }
    private bool checkWeaponType(int id, params ushort[] types)
    {
        if (id == Const.NoItem)
        {
            return false;
        }
        return _weaponDictionary.TryGetValue(id, out EquipParamWeapon? wep) && types.Contains(wep.wepType);

    }
    private bool hasSpellOfType(CharaInitParam chr, params byte[] types)
    {
        if (types == null || types.Length < 1)
        {
            throw new ArgumentException("types cannot be null, and must contain 1 or more values. Please pass in a valid weapon type.", nameof(types));
        }

        return checkSpellType(chr.equipSpell01, types) || checkSpellType(chr.equipSpell02, types);

    }
    private bool checkSpellType(int id, params byte[] types)
    {
        if (id == Const.NoItem)
        {
            return false;
        }
        return _magicDictionary.TryGetValue(id, out Magic? magic) && types.Contains(magic.ezStateBehaviorType);

    }
    private void giveArrows(CharaInitParam chr)
    {
        chr.equipArrow = getRandomAmmo(Const.ArrowType);
        chr.arrowNum = (ushort)(_random.Next() % Config.MaxArrows);
    }
    private void giveGreatArrows(CharaInitParam chr)
    {
        chr.equipSubArrow = getRandomAmmo(Const.GreatArrowType);
        chr.subArrowNum = (ushort)(_random.Next() % Config.MaxGreatArrows);
    }
    private void giveBolts(CharaInitParam chr)
    {
        chr.equipBolt = getRandomAmmo(Const.BoltType);
        chr.boltNum = (ushort)(_random.Next() % Config.MaxBolts);
    }
    private void giveBallistaBolts(CharaInitParam chr)
    {
        chr.equipSubBolt = getRandomAmmo(Const.BallistaBoltType);
        chr.subBoltNum = (ushort)(_random.Next() % Config.MaxBallistaBolts);
    }
    private int getRandomAmmo(ushort type)
    {
        IList<Param.Row> arrows = _weaponTypeDictionary[type];
        return arrows[_random.Next() % arrows.Count].ID;
    }
    private void randomizeSorceries(CharaInitParam chr, IReadOnlyList<int> spells)
    {
        chr.equipSpell01 = getRandomMagic(chr, Const.SorceryType, spells);
        if (chr.equipSpell02 == Const.NoItem)
        {
            chr.equipSpell02 = chanceRandomMagic(chr.equipSpell02, chr, Const.SorceryType, spells);
        }
        giveRandomWeapon(chr, Const.StaffType);
    }
    private void randomizeIncantations(CharaInitParam chr, IReadOnlyList<int> spells)
    {
        chr.equipSpell02 = getRandomMagic(chr, Const.IncantationType, spells);
        if (chr.equipSpell01 == Const.NoItem)
        {
            chr.equipSpell01 = chanceRandomMagic(chr.equipSpell01, chr, Const.IncantationType, spells);
        }
        giveRandomWeapon(chr, Const.SealType);
    }
    private int getRandomMagic(CharaInitParam chr, byte type, IReadOnlyList<int> spells)
    {
        IReadOnlyList<Param.Row> table = _magicTypeDictionary[type];
        while (true)
        {
            int i = _random.Next() % table.Count;
            Magic entry = _magicDictionary[table[i].ID];
            if (chrCanUseSpell(entry, chr) && spells.Contains(table[i].ID))
            {
                return table[i].ID;
            }
        }
    }
    private int chanceRandomMagic(int id, CharaInitParam chr, byte type, IReadOnlyList<int> spells)
    {
        return ReturnNoItem(id) ? Const.NoItem : getRandomMagic(chr, type, spells);

    }
    private void giveRandomWeapon(CharaInitParam chr, ushort type)
    {
        EquipParamWeapon? wep;
        if (_weaponDictionary.TryGetValue(chr.wepleft, out wep))
        {
            if (wep.wepType == type && chrCanUseWeapon(wep, chr))
            {
                return;
            }
        }
        else
        {
            chr.wepleft = getRandomWeapon(chr, type);
            return;
        }

        if (_weaponDictionary.TryGetValue(chr.wepRight, out wep))
        {
            if (wep.wepType == type && chrCanUseWeapon(wep, chr))
            {
                return;
            }
        }
        else
        {
            chr.wepRight = getRandomWeapon(chr, type);
            return;
        }

        if (_weaponDictionary.TryGetValue(chr.subWepLeft, out wep))
        {
            if (wep.wepType == type && chrCanUseWeapon(wep, chr))
            {
                return;
            }
        }
        else
        {
            chr.subWepLeft = getRandomWeapon(chr, type);
            return;
        }

        if (_weaponDictionary.TryGetValue(chr.subWepRight, out wep))
        {
            if (wep.wepType == type && chrCanUseWeapon(wep, chr))
            {
                return;
            }
        }
        else
        {
            chr.subWepRight = getRandomWeapon(chr, type);
            return;
        }

        if (_weaponDictionary.TryGetValue(chr.subWepLeft3, out wep))
        {
            if (wep.wepType == type && chrCanUseWeapon(wep, chr))
            {
                return;
            }
        }
        else
        {
            chr.subWepLeft3 = getRandomWeapon(chr, type);
            return;
        }

        chr.subWepRight3 = getRandomWeapon(chr, type);
    }
    private int getRandomWeapon(CharaInitParam chr, ushort type)
    {
        IReadOnlyList<Param.Row> table = _weaponTypeDictionary[type];
        while (true)
        {
            int i = _random.Next() % table.Count;
            if (_weaponDictionary.TryGetValue(table[i].ID, out EquipParamWeapon? entry))
            {
                if (chrCanUseWeapon(entry, chr))
                {
                    return table[i].ID;
                }
                continue;
            }

            entry = _customWeaponDictionary[table[i].ID];
            if (chrCanUseWeapon(entry, chr))
            {
                return table[i].ID;
            }
        }
    }
}
