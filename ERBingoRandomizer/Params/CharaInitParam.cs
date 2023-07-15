using static FSParam.Param;

namespace ERBingoRandomizer.Params;

public class CharaInitParam {
    private Cell _arrowNum;
    private Cell _baseDex;
    private Cell _baseEnd;
    private Cell _baseFai;
    private Cell _baseLuc;
    private Cell _baseMag;
    private Cell _baseStr;
    private Cell _baseVit;
    private Cell _baseWil;
    private Cell _boltNum;
    private Cell _equipArmer;
    private Cell _equipArrow;
    private Cell _equipBolt;
    private Cell _equipGaunt;
    private Cell _equipHelm;
    private Cell _equipLeg;
    private Cell _equipSpell01;
    private Cell _equipSpell02;
    private Cell _equipSubArrow;
    private Cell _equipSubBolt;
    private Cell _subArrowNum;
    private Cell _subBoltNum;
    private Cell _subWepLeft;
    private Cell _subWepLeft3;
    private Cell _subWepRight;
    private Cell _subWepRight3;

    private Cell _wepLeft;
    private Cell _wepRight;

    public CharaInitParam(Row chr) {
        _wepLeft = chr["equip_Wep_Left"].Value;
        _wepRight = chr["equip_Wep_Right"].Value;
        _subWepLeft = chr["equip_Subwep_Left"].Value;
        _subWepRight = chr["equip_Subwep_Right"].Value;
        _subWepLeft3 = chr["equip_Subwep_Left3"].Value;
        _subWepRight3 = chr["equip_Subwep_Right3"].Value;
        _equipHelm = chr["equip_Helm"].Value;
        _equipArmer = chr["equip_Armer"].Value;
        _equipGaunt = chr["equip_Gaunt"].Value;
        _equipLeg = chr["equip_Leg"].Value;
        _equipArrow = chr["equip_Arrow"].Value;
        _arrowNum = chr["arrowNum"].Value;
        _equipSubArrow = chr["equip_SubArrow"].Value;
        _subArrowNum = chr["subArrowNum"].Value;
        _equipBolt = chr["equip_Bolt"].Value;
        _boltNum = chr["boltNum"].Value;
        _equipSubBolt = chr["equip_SubBolt"].Value;
        _subBoltNum = chr["subBoltNum"].Value;
        _equipSpell01 = chr["equip_Spell_01"].Value;
        _equipSpell02 = chr["equip_Spell_02"].Value;

        _baseVit = chr["baseVit"].Value;
        _baseWil = chr["baseWil"].Value;
        _baseEnd = chr["baseEnd"].Value;
        _baseStr = chr["baseStr"].Value;
        _baseDex = chr["baseDex"].Value;
        _baseMag = chr["baseMag"].Value;
        _baseFai = chr["baseFai"].Value;
        _baseLuc = chr["baseLuc"].Value;

    }
    public int wepleft { get => (int)_wepLeft.Value; set => _wepLeft.Value = value; }
    public int wepRight { get => (int)_wepRight.Value; set => _wepRight.Value = value; }
    public int subWepLeft { get => (int)_subWepLeft.Value; set => _subWepLeft.Value = value; }
    public int subWepRight { get => (int)_subWepRight.Value; set => _subWepRight.Value = value; }
    public int subWepLeft3 { get => (int)_subWepLeft3.Value; set => _subWepLeft3.Value = value; }
    public int subWepRight3 { get => (int)_subWepRight3.Value; set => _subWepRight3.Value = value; }
    public int equipHelm { get => (int)_equipHelm.Value; set => _equipHelm.Value = value; }
    public int equipArmer { get => (int)_equipArmer.Value; set => _equipArmer.Value = value; }
    public int equipGaunt { get => (int)_equipGaunt.Value; set => _equipGaunt.Value = value; }
    public int equipLeg { get => (int)_equipLeg.Value; set => _equipLeg.Value = value; }
    public int equipArrow { get => (int)_equipArrow.Value; set => _equipArrow.Value = value; }
    public ushort arrowNum { get => (ushort)_arrowNum.Value; set => _arrowNum.Value = value; }
    public int equipSubArrow { get => (int)_equipSubArrow.Value; set => _equipSubArrow.Value = value; }
    public ushort subArrowNum { get => (ushort)_subArrowNum.Value; set => _subArrowNum.Value = value; }
    public int equipBolt { get => (int)_equipBolt.Value; set => _equipBolt.Value = value; }
    public ushort boltNum { get => (ushort)_boltNum.Value; set => _boltNum.Value = value; }
    public int equipSubBolt { get => (int)_equipSubBolt.Value; set => _equipSubBolt.Value = value; }
    public ushort subBoltNum { get => (ushort)_subBoltNum.Value; set => _subBoltNum.Value = value; }
    public int equipSpell01 { get => (int)_equipSpell01.Value; set => _equipSpell01.Value = value; }
    public int equipSpell02 { get => (int)_equipSpell02.Value; set => _equipSpell02.Value = value; }
    public byte baseVit { get => (byte)_baseVit.Value; set => _baseVit.Value = value; }
    public byte baseWil { get => (byte)_baseWil.Value; set => _baseWil.Value = value; }
    public byte baseEnd { get => (byte)_baseEnd.Value; set => _baseEnd.Value = value; }
    public byte baseStr { get => (byte)_baseStr.Value; set => _baseStr.Value = value; }
    public byte baseDex { get => (byte)_baseDex.Value; set => _baseDex.Value = value; }
    public byte baseMag { get => (byte)_baseMag.Value; set => _baseMag.Value = value; }
    public byte baseFai { get => (byte)_baseFai.Value; set => _baseFai.Value = value; }
    public byte baseLuc { get => (byte)_baseLuc.Value; set => _baseLuc.Value = value; }

    public Cell GetVit() {
        return _baseVit;
    }
    public Cell GetWil() {
        return _baseWil;
    }
    public Cell GetEnd() {
        return _baseEnd;
    }
    public Cell GetStr() {
        return _baseStr;
    }
    public Cell GetDex() {
        return _baseDex;
    }
    public Cell GetMag() {
        return _baseMag;
    }
    public Cell GetFai() {
        return _baseFai;
    }
    public Cell GetLuc() {
        return _baseLuc;
    }
}
