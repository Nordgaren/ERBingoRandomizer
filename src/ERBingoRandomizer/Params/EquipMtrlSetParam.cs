using FSParam;

namespace ERBingoRandomizer.Params; 

public class EquipMtrlSetParam {
    private Param.Cell _materialId01;
    private Param.Cell _materialCate01;
    private Param.Cell _itemNum01;

    public EquipMtrlSetParam(Param.Row mtrl) {
        _materialId01 = mtrl["materialId01"]!.Value;
        _materialCate01 = mtrl["materialCate01"]!.Value;
        _itemNum01 = mtrl["itemNum01"]!.Value;
    }
    public int materialId01 { get => (int)_materialId01.Value; set => _materialId01.Value = value; }
    public byte materialCate01 { get => (byte)_materialCate01.Value; set => _materialCate01.Value = value; }
    public sbyte itemNum01 { get => (sbyte)_itemNum01.Value; set => _itemNum01.Value = value; }
}
