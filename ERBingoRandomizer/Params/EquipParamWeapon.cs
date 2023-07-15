using static FSParam.Param;

namespace ERBingoRandomizer.Params; 

public class EquipParamWeapon {
    
    private Cell _wepType;
    
    private Cell _properStrength;
    private Cell _properAgility;
    private Cell _properMagic;
    private Cell _properFaith;
    private Cell _properLuck;
    
    public EquipParamWeapon(Row wep) {
        _wepType = wep["wepType"].Value;
        
        _properStrength = wep["properStrength"].Value;
        _properAgility = wep["properAgility"].Value;
        _properMagic = wep["properMagic"].Value;
        _properFaith = wep["properFaith"].Value;
        _properLuck = wep["properLuck"].Value;
    }
    
    public ushort wepType { get => (ushort)_wepType.Value; set => _wepType.Value = value; }

    public byte properStrength { get => (byte)_properStrength.Value; set => _properStrength.Value = value; }
    public byte properAgility { get => (byte)_properAgility.Value; set => _properAgility.Value = value; }
    public byte properMagic { get => (byte)_properMagic.Value; set => _properMagic.Value = value; }
    public byte properFaith { get => (byte)_properFaith.Value; set => _properFaith.Value = value; }
    public byte properLuck { get => (byte)_properLuck.Value; set => _properLuck.Value = value; }
    
}
