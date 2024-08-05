using FSParam;

namespace Project.Params;

public class EquipMtrlSetParam
{
    private Param.Cell _materialId01;
    private Param.Cell _materialId02;
    private Param.Cell _materialId03;
    private Param.Cell _materialId04;
    private Param.Cell _materialId05;
    private Param.Cell _materialId06;

    private Param.Cell _itemNum01;
    private Param.Cell _itemNum02;
    private Param.Cell _itemNum03;
    private Param.Cell _itemNum04;
    private Param.Cell _itemNum05;
    private Param.Cell _itemNum06;

    private Param.Cell _materialCate01;
    private Param.Cell _materialCate02;
    private Param.Cell _materialCate03;
    private Param.Cell _materialCate04;
    private Param.Cell _materialCate05;
    private Param.Cell _materialCate06;

    public EquipMtrlSetParam(Param.Row material)
    {
        _materialId01 = material["materialId01"]!.Value;
        _materialId02 = material["materialId02"]!.Value;
        _materialId03 = material["materialId03"]!.Value;
        _materialId04 = material["materialId04"]!.Value;
        _materialId05 = material["materialId05"]!.Value;
        _materialId06 = material["materialId06"]!.Value;

        _itemNum01 = material["itemNum01"]!.Value;
        _itemNum02 = material["itemNum02"]!.Value;
        _itemNum03 = material["itemNum03"]!.Value;
        _itemNum04 = material["itemNum04"]!.Value;
        _itemNum05 = material["itemNum05"]!.Value;
        _itemNum06 = material["itemNum06"]!.Value;

        _materialCate01 = material["materialCate01"]!.Value;
        _materialCate02 = material["materialCate02"]!.Value;
        _materialCate03 = material["materialCate03"]!.Value;
        _materialCate04 = material["materialCate04"]!.Value;
        _materialCate05 = material["materialCate05"]!.Value;
        _materialCate06 = material["materialCate06"]!.Value;
    }

}