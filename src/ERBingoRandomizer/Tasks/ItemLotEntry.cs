using System;

namespace Project.Tasks;

public readonly record struct ItemLotEntry(int Id, int Category) : IEquatable<int>
{
    public bool Equals(int other) { return other == Id; }
}

public readonly record struct ItemLotWrapper
{
    public ItemLotWrapper(ItemLotEntry entry, ushort type)
    {
        Entry = entry;
        Type = type;
    }
    public ItemLotEntry Entry { get; }
    public ushort Type { get; }
}