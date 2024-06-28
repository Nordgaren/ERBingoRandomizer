using FSParam;

namespace ERBingoRandomizer.Params;

public class EquipParamWeapon
{
    private Param.Cell _properAgility;
    private Param.Cell _properFaith;
    private Param.Cell _properLuck;
    private Param.Cell _properMagic;
    private Param.Cell _properStrength;
    private Param.Cell _wepType;
    private Param.Cell _materialSetId;
    private Param.Cell _reinforceTypeId;
    private Param.Cell _reinforceShopCategory;

    public EquipParamWeapon(Param.Row wep)
    {
        _wepType = wep["wepType"]!.Value;
        _materialSetId = wep["materialSetId"]!.Value;
        _reinforceTypeId = wep["reinforceTypeId"]!.Value;
        _reinforceShopCategory = wep["reinforceShopCategory"]!.Value;
        //reinforceShopCategory

        _properStrength = wep["properStrength"]!.Value;
        _properAgility = wep["properAgility"]!.Value;
        _properMagic = wep["properMagic"]!.Value;
        _properFaith = wep["properFaith"]!.Value;
        _properLuck = wep["properLuck"]!.Value;
    }

    public ushort wepType { get => (ushort)_wepType.Value; set => _wepType.Value = value; }
    public int materialSetId { get => (int)_materialSetId.Value; set => _materialSetId.Value = value; }
    public short reinforceTypeId { get => (short)_reinforceTypeId.Value; set => _reinforceTypeId.Value = value; }
    public byte reinforceShopCategory { get => (byte)_reinforceShopCategory.Value; set => _reinforceShopCategory.Value = value; }
    public byte properStrength { get => (byte)_properStrength.Value; set => _properStrength.Value = value; }
    public byte properAgility { get => (byte)_properAgility.Value; set => _properAgility.Value = value; }
    public byte properMagic { get => (byte)_properMagic.Value; set => _properMagic.Value = value; }
    public byte properFaith { get => (byte)_properFaith.Value; set => _properFaith.Value = value; }
    public byte properLuck { get => (byte)_properLuck.Value; set => _properLuck.Value = value; }
}
