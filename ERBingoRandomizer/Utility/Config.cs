using static ERBingoRandomizer.Utility.Const;

namespace ERBingoRandomizer.Utility; 

// Anything configurable will be in this class.
public static class Config {
    public const float Chance = 0.001f;
    // Max
    public const ushort MaxArrows = 30;
    public const ushort MaxGreatArrows = 10;
    public const ushort MaxBolts = 30;
    public const ushort MaxBallistaBolts = 10;
    // Base Stats For Magics
    public const byte MinInt = 13;
    public const byte MinFai = 12;
    // Level Randomizer  
    public const byte MaxStat = 13;
    public const byte MinStat = 4;
    public const float StatRollChance = 0.6f;
    public const byte NumStats = 8;
    
    public static readonly string CachePath = $"{ExeDir}/Cache";
    public static readonly string LastSeedPath = $"{CachePath}/LastSeed.json";
    public const string ResourcesPath = "./Resources";
}
