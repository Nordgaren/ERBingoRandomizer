using static FSParam.Param;

namespace ERBingoRandomizer.Params; 

public class EquipParamGoods {
    private Cell _goodsType;


    public EquipParamGoods(Row good) {
        _goodsType = good["goodsType"]!.Value;

    }
    public byte goodsType { get => (byte)_goodsType.Value; set => _goodsType.Value = value; }

}