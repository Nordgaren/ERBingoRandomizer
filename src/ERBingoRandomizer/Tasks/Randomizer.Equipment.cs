using Project.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using Project.Settings;
using FSParam;

namespace Project.Tasks;

public partial class Randomizer
{
    HashSet<int> allocatedIDs = new HashSet<int>()
    {
        14050000, 7040000, 7020000, 7100000, 15120000, 18060000,
        16010000, 16070000, 16020000, 16160000, 16150000,
        2150000, 5020000, 20000000,
    };
    private int randomizeStartingWeapon(int id, List<int> weapons)
    {
        int limit = weapons.Count;
        int newID = 0;
        do
        {
            _cancellationToken.ThrowIfCancellationRequested();
            int index = _random.Next(limit);
            newID = washWeaponMetadata(weapons[index]);
        } while (allocatedIDs.Contains(newID));

        allocatedIDs.Add(newID);  //  allocatedIDs.Add(washWeaponMetadata(newID));
        return newID;
    }
    private int getRandomArmor(int id, byte type, IReadOnlyDictionary<byte, List<int>> gearLists)
    {
        List<int> armors = gearLists[type];
        int limit = armors.Count;
        int newID = 0;
        int index = 0;
        do
        {
            _cancellationToken.ThrowIfCancellationRequested();
            index = _random.Next(limit);
            newID = armors[index];
        } while (allocatedIDs.Contains(newID));
        allocatedIDs.Add(newID);
        return newID;
    }
    private int exchangeArmorPiece(int id, byte type) { return getRandomArmor(id, type, Equipment.ArmorLists); }
    private bool validateNoItem(int id, int chance)
    {   // currently never used, but could be handy in a chaos mode where classes start with potentially nothing
        int randomChance = _random.Next(chance);

        if (id == Const.NoItem)
        { return Config.Target < randomChance; }

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
        if (id == Const.NoItem) { return false; }

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
        if (id == Const.NoItem) { return false; }

        return _magicDictionary.TryGetValue(id, out Magic? magic) && types.Contains(magic.ezStateBehaviorType);
    }
    private void giveArrows(CharaInitParam chr)
    {
        chr.equipArrow = getRandomAmmo(Const.ArrowType);
        chr.arrowNum = (ushort)(_random.Next(Config.MaxArrows));
    }
    private void giveGreatArrows(CharaInitParam chr)
    {
        chr.equipSubArrow = getRandomAmmo(Const.GreatArrowType);
        chr.subArrowNum = (ushort)(_random.Next(Config.MaxGreatArrows));
    }
    private void giveBolts(CharaInitParam chr)
    {
        chr.equipBolt = getRandomAmmo(Const.BoltType);
        chr.boltNum = (ushort)(_random.Next(Config.MaxBolts));
    }
    private void giveBallistaBolts(CharaInitParam chr)
    {
        chr.equipSubBolt = getRandomAmmo(Const.BallistaBoltType);
        chr.subBoltNum = (ushort)(_random.Next(Config.MaxBallistaBolts));
    }
    private int getRandomAmmo(ushort type)
    {
        IList<Param.Row> arrows = _weaponTypeDictionary[type];
        return arrows[_random.Next(arrows.Count)].ID;
    }
    private void randomizeSorceries(CharaInitParam chr, IReadOnlyList<int> spells)
    {
        assignUsableWeapon(chr, Const.StaffType);
        chr.equipSpell01 = assignStartingSpell(chr, Const.SorceryType, Equipment.StartingSorceryIDs);
    }
    private void randomizeIncantations(CharaInitParam chr, IReadOnlyList<int> spells)
    {
        assignUsableWeapon(chr, Const.SealType);
        chr.equipSpell02 = assignStartingSpell(chr, Const.IncantationType, Equipment.StartingIncantationIDs);
    }
    private int assignStartingSpell(CharaInitParam chr, byte type, IReadOnlyList<int> spells)
    {
        int index = _random.Next(spells.Count);
        return spells[index];
    }

    private void assignUsableWeapon(CharaInitParam chr, ushort type)
    {   // starting classes get two random weapons in slot1 left, right this fills in the next open slot with the desired type.
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
    { // currently only used for starting classes (staves and seals)
        if (type == Const.StaffType)
        { return 33000000; } // Glintstone Staff

        if (type == Const.SealType)
        { return 34000000; } // Finger Seal

        IReadOnlyList<Param.Row> table = _weaponTypeDictionary[type];
        int limit = table.Count;
        int i = _random.Next(limit);
        EquipParamWeapon entry = _weaponDictionary[table[i].ID];

        while (!chrCanUseWeapon(entry, chr))
        {
            _cancellationToken.ThrowIfCancellationRequested();
            i = _random.Next(limit);
            entry = _weaponDictionary[table[i].ID];
        }
        return table[i].ID;
    }
    private void randomizeEquipment(CharaInitParam chr, List<int> main, List<int> side)
    {
        chr.wepleft = randomizeStartingWeapon(chr.wepleft, side);
        chr.wepRight = randomizeStartingWeapon(chr.wepRight, main);
        chr.subWepLeft = Const.NoItem;
        chr.subWepRight = Const.NoItem;
        chr.subWepLeft3 = Const.NoItem;
        chr.subWepRight3 = Const.NoItem;

        chr.equipHelm = Config.Helmet; // assures no class is behind on armor
        chr.equipArmer = Config.Armor;
        chr.equipGaunt = Config.Gauntlet;
        chr.equipLeg = Config.Greaves;

        chr.equipHelm = exchangeArmorPiece(chr.equipHelm, Const.HelmType);
        chr.equipArmer = exchangeArmorPiece(chr.equipArmer, Const.BodyType);
        chr.equipGaunt = exchangeArmorPiece(chr.equipGaunt, Const.ArmType);
        chr.equipLeg = exchangeArmorPiece(chr.equipLeg, Const.LegType);

        chr.equipArrow = Const.NoItem;
        chr.arrowNum = ushort.MaxValue;
        if (hasWeaponOfType(chr, Const.BowType, Const.LightBowType))
        { giveArrows(chr); }

        chr.equipSubArrow = Const.NoItem;
        chr.subArrowNum = ushort.MaxValue;
        if (hasWeaponOfType(chr, Const.GreatbowType))
        { giveGreatArrows(chr); }

        chr.equipBolt = Const.NoItem;
        chr.boltNum = ushort.MaxValue;
        if (hasWeaponOfType(chr, Const.CrossbowType))
        { giveBolts(chr); }

        chr.equipSubBolt = Const.NoItem;
        chr.subBoltNum = ushort.MaxValue;
        if (hasWeaponOfType(chr, Const.BallistaType))
        { giveBallistaBolts(chr); }

        chr.equipSpell01 = -1;
        chr.equipSpell02 = -1;
    }
}
