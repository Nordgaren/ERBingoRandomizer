using System;

namespace Project.Tasks;

public readonly record struct ItemLotEntry(int Id, int Category) : IEquatable<int>
{
    public bool Equals(int other) { return other == Id; }
}