using FSParam;
using static FSParam.Param;
using static ERBingoRandomizer.Const;

namespace ERBingoRandomizer.Randomizer;

public partial class BingoRandomizer {
    private void randomizeLevels(Row chr) {
        Cell baseVit = chr["baseVit"].Value;
        Cell baseWil = chr["baseWil"].Value;
        Cell baseEnd = chr["baseEnd"].Value;
        Cell baseStr = chr["baseStr"].Value;
        Cell baseDex = chr["baseDex"].Value;
        Cell baseMag = chr["baseMag"].Value;
        Cell baseFai = chr["baseFai"].Value;
        Cell baseLuc = chr["baseLuc"].Value;

        int pool = (byte)baseVit.Value
            + (byte)baseWil.Value
            + (byte)baseEnd.Value
            + (byte)baseStr.Value
            + (byte)baseDex.Value
            + (byte)baseMag.Value
            + (byte)baseFai.Value
            + (byte)baseLuc.Value;

        pool -= MinStat * NumStats;

        baseVit.Value = MinStat;
        baseWil.Value = MinStat;
        baseEnd.Value = MinStat;
        baseStr.Value = MinStat;
        baseDex.Value = MinStat;
        baseMag.Value = MinStat;
        baseFai.Value = MinStat;
        baseLuc.Value = MinStat;

        while (pool > 0) {
            pool += modifyStats(baseVit);
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(baseWil);
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(baseEnd);
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(baseStr);
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(baseDex);
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(baseMag);
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(baseFai);
            if (pool <= 0) {
                break;
            }
            pool += modifyStats(baseLuc);
        }
    }

    int modifyStats(Cell entry) {
        byte value = (byte)entry.Value;
        if (value < MaxStat && _random.NextSingle() < StatRollChance) {
            value += 1;
            entry.Value = value;
            return -1;
        }
        return 0;
    }
}
