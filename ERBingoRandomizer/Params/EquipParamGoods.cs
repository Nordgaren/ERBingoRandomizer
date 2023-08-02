using FSParam;

namespace ERBingoRandomizer.Params; 

public class EquipParamGoods {
    private Param.Cell _goodsType;


    public EquipParamGoods(Param.Row good) {
        _goodsType = good["goodsType"]!.Value;

    }
    public byte goodsType { get => (byte)_goodsType.Value; set => _goodsType.Value = value; }

}