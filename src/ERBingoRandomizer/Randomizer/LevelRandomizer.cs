using ERBingoRandomizer.Params;
using FSParam;
using ERBingoRandomizer.Utility;


namespace ERBingoRandomizer.Randomizer;

public partial class BingoRandomizer {
    private void randomizeLevels(CharaInitParam chr) {

        chr.soulLv = Config.SoulLevel;

        int pool = Config.PoolSize;

        pool -= Config.MinStat * Const.NumStats;

        chr.baseVit = Config.MinStat;
        chr.baseWil = Config.MinStat;
        chr.baseEnd = Config.MinStat;
        chr.baseStr = Config.MinStat;
        chr.baseDex = Config.MinStat;
        chr.baseMag = Config.MinStat;
        chr.baseFai = Config.MinStat;
        chr.baseLuc = Config.MinStat;

        while (pool > 0) {
            pool += modifyStats(chr.GetVitCell());
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(chr.GetWilCell());
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(chr.GetEndCell());
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(chr.GetStrCell());
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(chr.GetDexCell());
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(chr.GetMagCell());
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(chr.GetFaiCell());
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(chr.GetLucCell());
        }
    }

    private int modifyStats(Param.Cell entry) {
        byte value = (byte)entry.Value;
        if (value >= Config.MaxStat || !(_random.NextSingle() < Config.StatRollChance)) {
            return 0;
        }
        entry.Value = (byte)(value + 1);
        return -1;
    }
}
