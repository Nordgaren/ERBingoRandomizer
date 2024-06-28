using ERBingoRandomizer.Params;
using FSParam;
using ERBingoRandomizer.Utility;

namespace ERBingoRandomizer.Randomizer;

public partial class BingoRandomizer
{
    private void initializeStats(CharaInitParam tarnished)
    {
        tarnished.soulLv = Config.SoulLevel;
        tarnished.baseVit = Config.MinStat;
        tarnished.baseWil = Config.MinStat;
        tarnished.baseEnd = Config.MinStat;
        tarnished.baseStr = Config.MinStat;
        tarnished.baseDex = Config.MinStat;
        tarnished.baseLuc = Config.MinStat;
        tarnished.baseMag = Config.MinStat;
        tarnished.baseFai = Config.MinStat;
    }
    private void increaseStats(int iterations, CharaInitParam tarnished)
    {
        int lastUpdated = -1;
        int stat = 0; // bumps vigor on 1st iteration
        while (iterations > 0)
        {
            if (stat == lastUpdated && (iterations & 1) == 0) stat = _random.Next(Const.NumStats); // decreases chance of stats streaking

            switch (stat)
            {
                case 0:
                    iterations -= modifyStats(tarnished.GetVitCell());
                    break;
                case 1:
                    iterations -= modifyStats(tarnished.GetWilCell());
                    break;
                case 2:
                    iterations -= modifyStats(tarnished.GetEndCell());
                    break;
                case 3:
                    iterations -= modifyStats(tarnished.GetStrCell());
                    break;
                case 4:
                    iterations -= modifyStats(tarnished.GetDexCell());
                    break;
                case 5:
                    iterations -= modifyStats(tarnished.GetMagCell());
                    break;
                case 6:
                    iterations -= modifyStats(tarnished.GetFaiCell());
                    break;
                case 7:
                    iterations -= modifyStats(tarnished.GetLucCell());
                    break;
            }
            lastUpdated = stat;
            stat = _random.Next(Const.NumStats);
        }
    }
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

    private void randomizeBaseStats(CharaInitParam tarnished)
    {
        initializeStats(tarnished);

        int iterations = Config.PoolSize;
        iterations -= Config.MinStat * Const.NumStats;

        increaseStats(iterations, tarnished);
    }

    private void rerollPrisonerStats(CharaInitParam prisoner)
    {
        initializeStats(prisoner);
        prisoner.baseMag = Config.MinInt;

        int iterations = Config.PoolSize;
        iterations -= Config.MinStat * (Const.NumStats - 1);
        iterations -= Config.MinInt;

        increaseStats(iterations, prisoner);
    }

    private void rerollConfessorStats(CharaInitParam confessor)
    {
        initializeStats(confessor);
        confessor.baseFai = Config.MinFai;

        int iterations = Config.PoolSize;
        iterations -= Config.MinStat * (Const.NumStats - 1);
        iterations -= Config.MinFai;

        increaseStats(iterations, confessor);
    }
}
