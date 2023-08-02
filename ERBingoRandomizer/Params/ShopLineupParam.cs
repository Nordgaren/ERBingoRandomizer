using System;
using System.Collections.Generic;
using static FSParam.Param;

namespace ERBingoRandomizer.Params;

public class ShopLineupParam : IEquatable<int>, IEquatable<ShopLineupParam> {
    private Cell _equipId;
    private Cell _costType;
    private Cell _mtrlId;
    private Cell _sellQuantity;
    private Cell _setNum;
    private Cell _value;
    private Cell _value_Add;
    private Cell _value_Magnification;
    private Cell _eventFlag_forStock;
    private Cell _eventFlag_forRelease;
    private Cell _iconId;
    private Cell _nameMsgId;
    private Cell _menuIconId;
    private Cell _menuTitleMsgId;

    public ShopLineupParam(Row lot) {
        _equipId = lot["equipId"]!.Value;
        _costType = lot["costType"]!.Value;
        _mtrlId = lot["mtrlId"]!.Value;
        _sellQuantity = lot["sellQuantity"]!.Value;
        _setNum = lot["setNum"]!.Value;
        _value = lot["value"]!.Value;
        _value_Add = lot["value_Add"]!.Value;
        _value_Magnification = lot["value_Magnification"]!.Value;
        _eventFlag_forStock = lot["eventFlag_forStock"]!.Value;
        _eventFlag_forRelease = lot["eventFlag_forRelease"]!.Value;
        _iconId = lot["iconId"]!.Value;
        _nameMsgId = lot["nameMsgId"]!.Value;
        _menuIconId = lot["menuIconId"]!.Value;
        _menuTitleMsgId = lot["menuTitleMsgId"]!.Value;

    }
    public int equipId { get => (int)_equipId.Value; set => _equipId.Value = value; }
    public byte costType { get => (byte)_costType.Value; set => _costType.Value = value; }
    public int mtrlId { get => (int)_mtrlId.Value; set => _mtrlId.Value = value; }
    public short sellQuantity { get => (short)_sellQuantity.Value; set => _sellQuantity.Value = value; }
    public ushort setNum { get => (ushort)_setNum.Value; set => _setNum.Value = value; }
    public int value { get => (int)_value.Value; set => _value.Value = value; }
    public int value_Add { get => (int)_value_Add.Value; set => _value_Add.Value = value; }
    public float value_Magnification { get => (float)_value_Magnification.Value; set => _value_Magnification.Value = value; }
    public uint eventFlag_forStock { get => (uint)_eventFlag_forStock.Value; set => _eventFlag_forStock.Value = value; }
    public uint eventFlag_forRelease { get => (uint)_eventFlag_forRelease.Value; set => _eventFlag_forRelease.Value = value; }
    public int iconId { get => (int)_iconId.Value; set => _iconId.Value = value; }
    public int nameMsgId { get => (int)_nameMsgId.Value; set => _nameMsgId.Value = value; }
    public short menuIconId { get => (short)_menuIconId.Value; set => _menuIconId.Value = value; }
    public int menuTitleMsgId { get => (int)_menuTitleMsgId.Value; set => _menuTitleMsgId.Value = value; }
    public bool Equals(int other) {
        return other == equipId;
    }
    public bool Equals(ShopLineupParam? other) {
        return other?.equipId == equipId;
    }
    public override int GetHashCode() {
        return equipId.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ShopLineupParam);
    }
}
