using ERBingoRandomizer.Params;
using static FSParam.Param;
using static ERBingoRandomizer.Utility.Config;


namespace ERBingoRandomizer.Randomizer;

public partial class BingoRandomizer {
    private void randomizeLevels(CharaInitParam chr) {

        int pool = chr.baseVit
            + chr.baseWil
            + chr.baseEnd
            + chr.baseStr
            + chr.baseDex
            + chr.baseMag
            + chr.baseFai
            + chr.baseLuc;

        pool -= MinStat * NumStats;

        chr.baseVit = MinStat;
        chr.baseWil = MinStat;
        chr.baseEnd = MinStat;
        chr.baseStr = MinStat;
        chr.baseDex = MinStat;
        chr.baseMag = MinStat;
        chr.baseFai = MinStat;
        chr.baseLuc = MinStat;

        while (pool > 0) {
            pool += modifyStats(chr.GetVit());
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(chr.GetWil());
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(chr.GetEnd());
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(chr.GetStr());
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(chr.GetDex());
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(chr.GetMag());
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(chr.GetFai());
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(chr.GetLuc());
        }
    }

    int modifyStats(Cell entry) {
        byte value = (byte)entry.Value;
        if (value < MaxStat && _random.NextSingle() < StatRollChance) {
            entry.Value = (byte)(value + 1);
            return -1;
        }
        return 0;
    }
}
