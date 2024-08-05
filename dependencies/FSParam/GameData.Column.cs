using SoulsFormats;

namespace FSParam;

public partial class Param : SoulsFile<Param>
{
    /// <summary>
    /// Represents a Column (param field) in the param. Unlike the Soulsformats Cell, this one is stored
    /// completely separately, and reading/writing a value requires the Row to read/write from.
    /// </summary>
    public class Column
    {
        public PARAMDEF.Field Def { get; }

        public Type ValueType { get; private set; }

        private uint _byteOffset;
        private uint _arrayLength;
        private int _bitSize;
        private uint _bitOffset;

        internal Column(PARAMDEF.Field def, uint byteOffset, uint arrayLength = 1)
        {
            Def = def;
            _byteOffset = byteOffset;
            _arrayLength = arrayLength;
            _bitSize = -1;
            _bitOffset = 0;
            ValueType = TypeForParamDefType(def.DisplayType, arrayLength > 1);
        }

        internal Column(PARAMDEF.Field def, uint byteOffset, int bitSize, uint bitOffset)
        {
            Def = def;
            _byteOffset = byteOffset;
            _arrayLength = 1;
            _bitSize = bitSize;
            _bitOffset = bitOffset;
            ValueType = TypeForParamDefType(def.DisplayType, false);
        }

        private static Type TypeForParamDefType(PARAMDEF.DefType type, bool isArray)
        {
            switch (type)
            {
                case PARAMDEF.DefType.s8:
                    return typeof(sbyte);
                case PARAMDEF.DefType.u8:
                    return typeof(byte);
                case PARAMDEF.DefType.s16:
                    return typeof(short);
                case PARAMDEF.DefType.u16:
                    return typeof(ushort);
                case PARAMDEF.DefType.s32:
                case PARAMDEF.DefType.b32:
                    return typeof(int);
                case PARAMDEF.DefType.u32:
                    return typeof(uint);
                case PARAMDEF.DefType.f32:
                case PARAMDEF.DefType.angle32:
                    return typeof(float);
                case PARAMDEF.DefType.f64:
                    return typeof(double);
                case PARAMDEF.DefType.dummy8:
                    return isArray ? typeof(byte[]) : typeof(byte);
                case PARAMDEF.DefType.fixstr:
                case PARAMDEF.DefType.fixstrW:
                    return typeof(string);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public object GetValue(Row row)
        {
            var data = row.Parent._paramData;
            switch (Def.DisplayType)
            {
                case PARAMDEF.DefType.s8:
                    return data.ReadValueAtElementOffset<sbyte>(row.DataIndex, _byteOffset);
                case PARAMDEF.DefType.s16:
                    return data.ReadValueAtElementOffset<short>(row.DataIndex, _byteOffset);
                case PARAMDEF.DefType.s32:
                case PARAMDEF.DefType.b32:
                    return data.ReadValueAtElementOffset<int>(row.DataIndex, _byteOffset);
                case PARAMDEF.DefType.f32:
                case PARAMDEF.DefType.angle32:
                    return data.ReadValueAtElementOffset<float>(row.DataIndex, _byteOffset);
                case PARAMDEF.DefType.f64:
                    return data.ReadValueAtElementOffset<double>(row.DataIndex, _byteOffset);
                case PARAMDEF.DefType.u8:
                case PARAMDEF.DefType.dummy8:
                    if (_arrayLength > 1)
                    {
                        return data.ReadByteArrayAtElementOffset(row.DataIndex, _byteOffset, _arrayLength);
                    }
                    var value8 = data.ReadValueAtElementOffset<byte>(row.DataIndex, _byteOffset);
                    if (_bitSize != -1)
                        value8 = (byte)((value8 >> (int)_bitOffset) & (0xFF >> (8 - _bitSize)));
                    return value8;
                case PARAMDEF.DefType.u16:
                    var value16 = data.ReadValueAtElementOffset<ushort>(row.DataIndex, _byteOffset);
                    if (_bitSize != -1)
                        value16 = (ushort)((value16 >> (int)_bitOffset) & (0xFFFF >> (16 - _bitSize)));
                    return value16;
                case PARAMDEF.DefType.u32:
                    var value32 = data.ReadValueAtElementOffset<uint>(row.DataIndex, _byteOffset);
                    if (_bitSize != -1)
                        value32 = (uint)((value32 >> (int)_bitOffset) & (0xFFFFFFFF >> (32 - _bitSize)));
                    return value32;
                case PARAMDEF.DefType.fixstr:
                    return data.ReadFixedStringAtElementOffset(row.DataIndex, _byteOffset, _arrayLength);
                case PARAMDEF.DefType.fixstrW:
                    return data.ReadFixedStringWAtElementOffset(row.DataIndex, _byteOffset, _arrayLength);
                default:
                    throw new NotImplementedException($"Unsupported field type: {Def.DisplayType}");
            }
        }

        public void SetValue(Row row, object value)
        {
            var data = row.Parent._paramData;
            switch (Def.DisplayType)
            {
                case PARAMDEF.DefType.s8:
                    data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, (sbyte)value);
                    break;
                case PARAMDEF.DefType.s16:
                    data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, (short)value);
                    break;
                case PARAMDEF.DefType.s32:
                case PARAMDEF.DefType.b32:
                    data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, (int)value);
                    break;
                case PARAMDEF.DefType.f32:
                case PARAMDEF.DefType.angle32:
                    data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, (float)value);
                    break;
                case PARAMDEF.DefType.f64:
                    data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, (double)value);
                    break;
                case PARAMDEF.DefType.u8:
                case PARAMDEF.DefType.dummy8:
                    if (_arrayLength > 1)
                    {
                        data.WriteByteArrayAtElementOffset(row.DataIndex, _byteOffset, (byte[])value);
                    }
                    else
                    {
                        var value8 = (byte)value;
                        if (_bitSize != -1)
                        {
                            var o8 = data.ReadValueAtElementOffset<byte>(row.DataIndex, _byteOffset);
                            var mask8 = (byte)(0xFF >> (8 - _bitSize) << (int)_bitOffset);
                            value8 = (byte)((o8 & ~mask8) | ((value8 << (int)_bitOffset) & mask8));
                        }

                        data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, value8);
                    }

                    break;
                case PARAMDEF.DefType.u16:
                    var value16 = (ushort)value;
                    if (_bitSize != -1)
                    {
                        var o16 = data.ReadValueAtElementOffset<ushort>(row.DataIndex, _byteOffset);
                        var mask16 = (ushort)(0xFFFF >> (16 - _bitSize) << (int)_bitOffset);
                        value16 = (ushort)((o16 & ~mask16) | ((value16 << (int)_bitOffset) & mask16));
                    }
                    data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, value16);
                    break;
                case PARAMDEF.DefType.u32:
                    var value32 = (uint)value;
                    if (_bitSize != -1)
                    {
                        var o32 = data.ReadValueAtElementOffset<uint>(row.DataIndex, _byteOffset);
                        var mask32 = (uint)(0xFFFFFFFF >> (32 - _bitSize) << (int)_bitOffset);
                        value32 = (uint)((o32 & ~mask32) | ((value32 << (int)_bitOffset) & mask32));
                    }
                    data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, value32);
                    break;
                case PARAMDEF.DefType.fixstr:
                    data.WriteFixedStringAtElementOffset(row.DataIndex, _byteOffset, (string)value, _arrayLength);
                    break;
                case PARAMDEF.DefType.fixstrW:
                    data.WriteFixedStringWAtElementOffset(row.DataIndex, _byteOffset, (string)value, _arrayLength);
                    break;
                default:
                    throw new NotImplementedException($"Unsupported field type: {Def.DisplayType}");
            }
        }
    }
}