﻿
namespace Project.Settings;

// Anything configurable will be in this class.
public static class Config
{
    public const int ArmorChance = 5;
    public const int SpellChance = 20;
    public const int Target = 1;
    public const int Helmet = 660000;
    public const int Armor = 661100;
    public const int Gauntlet = 660200;
    public const int Greaves = 660300;
    // Max
    public const ushort MaxArrows = 30;
    public const ushort FirstClassId = 3000;
    public const ushort NumberOfClasses = 10;
    public const ushort MaxGreatArrows = 10;
    public const ushort MaxBolts = 30;
    public const ushort MaxBallistaBolts = 10;
    // Base Stats For Magics
    public const byte MinInt = 10;
    public const byte MinFai = 10;
    // Level Randomizer  
    public const byte MaxStat = 16;
    public const byte MinStat = 5;
    public const byte SoulLevel = 9;
    public const byte PoolSize = 88;
    public const float StatRollChance = 0.001f;
    // Paths
    public const string ResourcesPath = "./Resources";
    public static readonly string CachePath = $"{Const.ExeDir}\\Cache";
    public static readonly string SpoilerPath = $"{CachePath}/Spoilers";
    public static readonly string LastSeedPath = $"{CachePath}/LastSeed.json";
    public static readonly string PackagesPath = $"{Const.ExeDir}\\Cache\\Packaged";
    // Files
    public const bool CacheBHDs = false;
}
