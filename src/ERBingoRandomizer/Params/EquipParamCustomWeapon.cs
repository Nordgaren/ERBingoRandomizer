using FSParam;

namespace Project.Params;

public class EquipParamCustomWeapon
{
    private Param.Cell _baseWepId;
    private Param.Cell _gemId;
    private Param.Cell _reinforceLv;

    public EquipParamCustomWeapon(Param.Row wep)
    {
        _baseWepId = wep["baseWepId"]!.Value;
        _gemId = wep["gemId"]!.Value;
        _reinforceLv = wep["reinforceLv"]!.Value;
    }

    public int baseWepId { get => (int)_baseWepId.Value; set => _baseWepId.Value = value; }
    public int gemId { get => (int)_gemId.Value; set => _gemId.Value = value; }
    public byte reinforceLv { get => (byte)_reinforceLv.Value; set => _reinforceLv.Value = value; }
}
