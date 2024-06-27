using ERBingoRandomizer.Params;
using FSParam;
using ERBingoRandomizer.Utility;

namespace ERBingoRandomizer.Randomizer;
// see void guaranteePrisonerHasSpells
// see void guaranteeConfessorHasIncantation
public partial class BingoRandomizer
{
    private int modifyStats(Param.Cell entry)
    {
        byte value = (byte)entry.Value;
        if (value >= Config.MaxStat)
        {
            return 0;
        }
        entry.Value = (byte)(value + 1);
        return 1; // adjust iterations
    }

    private void randomizeLevels(CharaInitParam chr)
    {
        chr.soulLv = Config.SoulLevel;
        chr.baseVit = Config.MinStat;
        chr.baseWil = Config.MinStat;
        chr.baseEnd = Config.MinStat;
        chr.baseStr = Config.MinStat;
        chr.baseDex = Config.MinStat;
        chr.baseLuc = Config.MinStat;

        chr.baseMag = Config.MinStat;
        chr.baseFai = Config.MinStat;

        int iterations = Config.PoolSize;
        iterations -= Config.MinStat * Const.NumStats; // 88 minus (6 * 8) equals 40

        int lastUpdated = -1;
        int stat = 0; // bumps vigor to 6 on 1st iteration
        while (iterations > 0)
        {
            if (stat == lastUpdated) stat = _random.Next(Const.NumStats); // decreases chance of stats streaking

            switch (stat)
            {
                case 0:
                    iterations -= modifyStats(chr.GetVitCell());
                    break;
                case 1:
                    iterations -= modifyStats(chr.GetWilCell());
                    break;
                case 2:
                    iterations -= modifyStats(chr.GetEndCell());
                    break;
                case 3:
                    iterations -= modifyStats(chr.GetStrCell());
                    break;
                case 4:
                    iterations -= modifyStats(chr.GetDexCell());
                    break;
                case 5:
                    iterations -= modifyStats(chr.GetMagCell());
                    break;
                case 6:
                    iterations -= modifyStats(chr.GetFaiCell());
                    break;
                case 7:
                    iterations -= modifyStats(chr.GetLucCell());
                    break;
            }
            lastUpdated = stat;
            stat = _random.Next(Const.NumStats);
        }
    }

    private void rerollPrisonerStats(CharaInitParam chr)
    {
        chr.soulLv = Config.SoulLevel;
        chr.baseVit = Config.MinStat;
        chr.baseWil = Config.MinStat;
        chr.baseEnd = Config.MinStat;
        chr.baseStr = Config.MinStat;
        chr.baseDex = Config.MinStat;
        chr.baseLuc = Config.MinStat;
        chr.baseFai = Config.MinStat;

        chr.baseMag = Config.MinInt;

        int iterations = Config.PoolSize;
        iterations -= Config.MinStat * (Const.NumStats - 1);
        iterations -= Config.MinInt;

        int lastUpdated = -1;
        int stat = 0; // bumps vigor on 1st iteration
        while (iterations > 0)
        {
            if (stat == lastUpdated) stat = _random.Next(Const.NumStats); // decreases chance of stats streaking

            switch (stat)
            {
                case 0:
                    iterations -= modifyStats(chr.GetVitCell());
                    break;
                case 1:
                    iterations -= modifyStats(chr.GetWilCell());
                    break;
                case 2:
                    iterations -= modifyStats(chr.GetEndCell());
                    break;
                case 3:
                    iterations -= modifyStats(chr.GetStrCell());
                    break;
                case 4:
                    iterations -= modifyStats(chr.GetDexCell());
                    break;
                case 5:
                    iterations -= modifyStats(chr.GetMagCell());
                    break;
                case 6:
                    iterations -= modifyStats(chr.GetFaiCell());
                    break;
                case 7:
                    iterations -= modifyStats(chr.GetLucCell());
                    break;
            }
            lastUpdated = stat;
            stat = _random.Next(Const.NumStats);
        }
    }

    private void rerollConfessorStats(CharaInitParam chr)
    {
        chr.soulLv = Config.SoulLevel;
        chr.baseVit = Config.MinStat;
        chr.baseWil = Config.MinStat;
        chr.baseEnd = Config.MinStat;
        chr.baseStr = Config.MinStat;
        chr.baseDex = Config.MinStat;
        chr.baseLuc = Config.MinStat;
        chr.baseMag = Config.MinStat;

        chr.baseFai = Config.MinFai;

        int iterations = Config.PoolSize;
        iterations -= Config.MinStat * (Const.NumStats - 1);
        iterations -= Config.MinFai;

        int lastUpdated = -1;
        int stat = 0; // bumps vigor on 1st iteration
        while (iterations > 0)
        {
            if (stat == lastUpdated) stat = _random.Next(Const.NumStats); // decreases chance of stats streaking

            switch (stat)
            {
                case 0:
                    iterations -= modifyStats(chr.GetVitCell());
                    break;
                case 1:
                    iterations -= modifyStats(chr.GetWilCell());
                    break;
                case 2:
                    iterations -= modifyStats(chr.GetEndCell());
                    break;
                case 3:
                    iterations -= modifyStats(chr.GetStrCell());
                    break;
                case 4:
                    iterations -= modifyStats(chr.GetDexCell());
                    break;
                case 5:
                    iterations -= modifyStats(chr.GetMagCell());
                    break;
                case 6:
                    iterations -= modifyStats(chr.GetFaiCell());
                    break;
                case 7:
                    iterations -= modifyStats(chr.GetLucCell());
                    break;
            }
            lastUpdated = stat;
            stat = _random.Next(Const.NumStats);
        }
    }
}
