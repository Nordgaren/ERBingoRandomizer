using static FSParam.Param;

namespace ERBingoRandomizer.Params; 

public class EquipParamCustomWeapon {
    private Cell _baseWepId;
    private Cell _gemId;
    private Cell _reinforceLv;

    public EquipParamCustomWeapon(Row wep) {
        _baseWepId = wep["baseWepId"].Value;
        _gemId = wep["gemId"].Value;
        _reinforceLv = wep["reinforceLv"].Value;
    }
    
    public int baseWepId { get => (int)_baseWepId.Value; set => _baseWepId.Value = value; }
    public int gemId { get => (int)_gemId.Value; set => _gemId.Value = value; }
    public byte reinforceLv { get => (byte)_reinforceLv.Value; set => _reinforceLv.Value = value; }
}
