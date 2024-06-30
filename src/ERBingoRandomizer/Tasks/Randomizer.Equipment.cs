using Project.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using Project.Settings;
using FSParam;

namespace Project.Tasks;

public partial class Randomizer
{
    private int randomizeStartingWeapon(int id, IReadOnlyList<int> weapons)
    { // TODO Rely on weapons table
        int limit = weapons.Count;
        // int newID = weapons[_random.Next(limit)];
        // while (!_weaponDictionary.ContainsKey(newID))
        // { // TODO update _weaponDictionary for DLC weapons
        //     newID = weapons[_random.Next(limit)];
        // }
        int newID = Equipment.StartingWeaponIDs[_random.Next(Equipment.StartingWeaponIDs.Count)];
        return washWeaponMetadata(newID);
    }
    private int exchangeArmorPiece(int id, byte type)
    {
        return validateNoItem(id, Config.ArmorChance) ? Const.NoItem : getRandomArmor(id, type);
    }
    private int getRandomArmor(int id, byte type)
    {
        //IReadOnlyList<Param.Row> armors = _armorTypeDictionary[type];
        // return armors[_random.Next(armors.Count)].ID;
        IReadOnlyList<int> armors = Equipment.ArmorLists[type];
        return armors[_random.Next(armors.Count)];
    }
    private bool validateNoItem(int id, int chance)
    {   // If the entry is -1 (no item) validate for a small chance to becomes an item.
        int randomChance = _random.Next(chance);

        if (id == Const.NoItem)
        {
            return Config.Target < randomChance;
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
        assignUsableWeapon(chr, Const.StaffType);
    }
    private void randomizeIncantations(CharaInitParam chr, IReadOnlyList<int> spells)
    {
        chr.equipSpell02 = getRandomMagic(chr, Const.IncantationType, spells);
        if (chr.equipSpell01 == Const.NoItem)
        {
            chr.equipSpell01 = chanceRandomMagic(chr.equipSpell01, chr, Const.IncantationType, spells);
        }
        assignUsableWeapon(chr, Const.SealType);
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
        return validateNoItem(id, Config.SpellChance) ? Const.NoItem : getRandomMagic(chr, type, spells);
    }

    private void assignUsableWeapon(CharaInitParam chr, ushort type)
    {   // starting classes get two random weapons in slot1 left, right
        // this fills in the next open slot with the desired type.
        EquipParamWeapon? wep;

        if (_weaponDictionary.TryGetValue(chr.subWepLeft, out wep))
        {
            if (wep.wepType == type && chrCanUseWeapon(wep, chr))
            { return; }
        }
        else
        {
            chr.subWepLeft = getUsableWeapon(chr, type);
            return;
        }

        if (_weaponDictionary.TryGetValue(chr.subWepRight, out wep))
        {
            if (wep.wepType == type && chrCanUseWeapon(wep, chr))
            { return; }
        }
        else
        {
            chr.subWepRight = getUsableWeapon(chr, type);
            return;
        }

        if (_weaponDictionary.TryGetValue(chr.subWepLeft3, out wep))
        {
            if (wep.wepType == type && chrCanUseWeapon(wep, chr))
            { return; }
        }
        else
        {
            chr.subWepLeft3 = getUsableWeapon(chr, type);
            return;
        }

        chr.subWepRight3 = getUsableWeapon(chr, type);
    }
    private int getUsableWeapon(CharaInitParam chr, ushort type)
    {
        IReadOnlyList<Param.Row> table = _weaponTypeDictionary[type];
        int limit = table.Count;

        while (true)
        {
            int i = _random.Next() % limit;
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
