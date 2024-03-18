using FSParam;

namespace ERBingoRandomizer.Params;

public class EquipParamWeapon {
    private Param.Cell _bothHandEquipable;
    private Param.Cell _originEquipWep10;
    private Param.Cell _originEquipWep25;
    private Param.Cell _properAgility;
    private Param.Cell _properFaith;
    private Param.Cell _properLuck;
    private Param.Cell _properMagic;
    private Param.Cell _properStrength;
    private Param.Cell _wepType;
    private Param.Cell _isDualBlade;
    private Param.Cell _materialSetId;
    private Param.Cell _reinforceTypeId;
    private Param.Cell _reinforceShopCategory;

    public EquipParamWeapon(Param.Row wep) {
        _bothHandEquipable = wep["bothHandEquipable"]!.Value;
        _isDualBlade = wep["isDualBlade"]!.Value;
        _wepType = wep["wepType"]!.Value;
        _materialSetId = wep["materialSetId"]!.Value;
        _reinforceTypeId = wep["reinforceTypeId"]!.Value;
        _reinforceShopCategory = wep["reinforceShopCategory"]!.Value;
        //reinforceShopCategory

        _originEquipWep10 = wep["originEquipWep10"]!.Value;
        _originEquipWep25 = wep["originEquipWep25"]!.Value;
        _properStrength = wep["properStrength"]!.Value;
        _properAgility = wep["properAgility"]!.Value;
        _properMagic = wep["properMagic"]!.Value;
        _properFaith = wep["properFaith"]!.Value;
        _properLuck = wep["properLuck"]!.Value;
    }

    public byte bothHandEquipable { get => (byte)_bothHandEquipable.Value; set => _bothHandEquipable.Value = value; }
    public byte isDualBlade { get => (byte)_isDualBlade.Value; set => _isDualBlade.Value = value; }
    public WeaponType wepType { get => (WeaponType)_wepType.Value; set => _wepType.Value = value; }
    public int materialSetId { get => (int)_materialSetId.Value; set => _materialSetId.Value = value; }
    public short reinforceTypeId { get => (short)_reinforceTypeId.Value; set => _reinforceTypeId.Value = value; }
    public byte reinforceShopCategory { get => (byte)_reinforceShopCategory.Value; set => _reinforceShopCategory.Value = value; }
    public int originEquipWep10 { get => (int)_originEquipWep10.Value; set => _originEquipWep10.Value = value; }
    public int originEquipWep25 { get => (int)_originEquipWep25.Value; set => _originEquipWep25.Value = value; }
    public byte properStrength { get => (byte)_properStrength.Value; set => _properStrength.Value = value; }
    public byte properAgility { get => (byte)_properAgility.Value; set => _properAgility.Value = value; }
    public byte properMagic { get => (byte)_properMagic.Value; set => _properMagic.Value = value; }
    public byte properFaith { get => (byte)_properFaith.Value; set => _properFaith.Value = value; }
    public byte properLuck { get => (byte)_properLuck.Value; set => _properLuck.Value = value; }
    
    public enum WeaponType : ushort
    {
        Dagger = 1, //Dagger
        StraightSword = 3, //SwordNormal
        Greatsword = 5, //SwordLarge
        ColossalSword = 7, //SwordGigantic
        CurvedSword = 9, //SabreNormal
        CurvedGreatsword = 11, //SabreLarge
        Katana = 13, //katana
        Twinblade = 14, //SwordDoulbeEdge
        ThrustingSword = 15, //SwordPierce
        HeavyThrustingSword = 16, //RapierHeavy
        Axe = 17, //AxeNormal
        Greataxe = 19, //AxeLarge
        Hammer = 21, //HammerNormal
        GreatHammer = 23, //HammerLarge
        Flail = 24, //Flail
        Spear = 25, //SpearNormal
        SpearLarge = 26, //SpearLarge UNUSED
        GreatSpear = 28, //SpearHeavy
        Halberd = 29, //SpearAxe
        Reaper = 31, //Sickle
        Fist = 35, //Knuckle
        Claws = 37, //Claw
        Whip = 39, //Whip
        ColossalWeapon = 41, //Axhammerlarge
        LightBow = 50, //BowSmall
        Bow = 51, //BowNormal
        Greatbow = 53, //BowLarge
        Crossbow = 55, //Clossbow
        Ballista = 56, //Ballista
        GlintstoneStaff = 57, //Staff
        Sorcery = 58, //Sorcery UNUSED
        FingerSeal = 61, //Talisman
        SmallShield = 65, //ShieldSmall
        MediumShield = 67, //ShieldNormal
        Greatshield = 69, //SheildLarge
        Arrow = 81,
        GreatArrow = 83,
        Bolt = 85,
        BallistaBolt = 86,
        Torch = 87 //Torch
    }
}
