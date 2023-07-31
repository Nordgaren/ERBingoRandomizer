using System;

namespace ERBingoRandomizer.Randomizer; 

public readonly record struct ItemLotWeapon(int Id, int Category) : IEquatable<int> {
    public bool Equals(int other) {
        return other == Id;
    }
}