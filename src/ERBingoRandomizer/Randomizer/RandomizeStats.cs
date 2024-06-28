using ERBingoRandomizer.Params;
using FSParam;
using ERBingoRandomizer.Utility;

namespace ERBingoRandomizer.Randomizer;

public partial class BingoRandomizer
{   // ideally would get each class ID, then create a method for each class
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
            {   // would have liked to have used an enum to draw a random category from, but there are a fixed number of 8 stats in the game (0 to 7)
                case 0:
                    iterations -= validateIncrease(tarnished.GetVitCell());
                    break;
                case 1:
                    iterations -= validateIncrease(tarnished.GetWilCell());
                    break;
                case 2:
                    iterations -= validateIncrease(tarnished.GetEndCell());
                    break;
                case 3:
                    iterations -= validateIncrease(tarnished.GetStrCell());
                    break;
                case 4:
                    iterations -= validateIncrease(tarnished.GetDexCell());
                    break;
                case 5:
                    iterations -= validateIncrease(tarnished.GetMagCell());
                    break;
                case 6:
                    iterations -= validateIncrease(tarnished.GetFaiCell());
                    break;
                case 7:
                    iterations -= validateIncrease(tarnished.GetLucCell());
                    break;
            }
            lastUpdated = stat;
            stat = _random.Next(Const.NumStats);
        }
    }
    private int validateIncrease(Param.Cell entry)
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
        int iterations = Config.PoolSize - (Config.MinStat * Const.NumStats);
        initializeStats(tarnished);
        increaseStats(iterations, tarnished);
    }

    private void rerollPrisonerStats(CharaInitParam prisoner)
    {
        int iterations = Config.PoolSize - (Config.MinStat * (Const.NumStats - 1));
        iterations -= Config.MinInt;
        initializeStats(prisoner);
        prisoner.baseMag = Config.MinInt;
        increaseStats(iterations, prisoner);
    }

    private void rerollConfessorStats(CharaInitParam confessor)
    {
        int iterations = Config.PoolSize - (Config.MinStat * (Const.NumStats - 1));
        iterations -= Config.MinFai;
        initializeStats(confessor);
        confessor.baseFai = Config.MinFai;
        increaseStats(iterations, confessor);
    }
}
