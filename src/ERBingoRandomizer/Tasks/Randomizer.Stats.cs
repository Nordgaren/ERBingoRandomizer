using System.Collections.Generic;
using FSParam;
using Project.Params;
using Project.Settings;

namespace Project.Tasks;

public partial class Randomizer
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
    {   // There are a fixed number of 8 class stats in the game (0 to 7)
        List<int> stats = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
        int index = 0;
        int stat = stats[index]; // bumps vigor on 1st iteration
        while (iterations > 0)
        {
            _cancellationToken.ThrowIfCancellationRequested();
            switch (stat)
            {
                case 0:
                    iterations -= validateIncrease(tarnished.GetVitCell());
                    if (exceedsMaxStat(tarnished.GetVitCell())) { stats.Remove(0); }
                    break;
                case 1:
                    iterations -= validateIncrease(tarnished.GetWilCell());
                    if (exceedsMaxStat(tarnished.GetWilCell())) { stats.Remove(1); }
                    break;
                case 2:
                    iterations -= validateIncrease(tarnished.GetEndCell());
                    if (exceedsMaxStat(tarnished.GetEndCell())) { stats.Remove(2); }
                    break;
                case 3:
                    iterations -= validateIncrease(tarnished.GetStrCell());
                    if (exceedsMaxStat(tarnished.GetStrCell())) { stats.Remove(3); }
                    break;
                case 4:
                    iterations -= validateIncrease(tarnished.GetDexCell());
                    if (exceedsMaxStat(tarnished.GetDexCell())) { stats.Remove(4); }
                    break;
                case 5:
                    iterations -= validateIncrease(tarnished.GetMagCell());
                    if (exceedsMaxStat(tarnished.GetMagCell())) { stats.Remove(5); }
                    break;
                case 6:
                    iterations -= validateIncrease(tarnished.GetFaiCell());
                    if (exceedsMaxStat(tarnished.GetFaiCell())) { stats.Remove(6); }
                    break;
                case 7:
                    iterations -= validateIncrease(tarnished.GetLucCell());
                    if (exceedsMaxStat(tarnished.GetLucCell())) { stats.Remove(7); }
                    break;
            }
            index = _random.Next(stats.Count);
            stat = stats[index];
        }
    }
    private int validateIncrease(Param.Cell entry)
    {
        byte value = (byte)entry.Value;

        if (value >= Config.MaxStat)
        { return 0; }

        entry.Value = (byte)(value + 1);
        return 1; // adjust iterations
    }

    private bool exceedsMaxStat(Param.Cell entry) { return Config.MaxStat <= (byte)entry.Value; }

    private void setClassStats(CharaInitParam tarnished)
    {
        int iterations = Config.PoolSize - (Config.MinStat * Const.NumStats);
        initializeStats(tarnished);
        increaseStats(iterations, tarnished);
    }

    private void setPrisonerStats(CharaInitParam prisoner)
    {
        int iterations = Config.PoolSize - (Config.MinStat * (Const.NumStats - 1));
        iterations -= Config.MinInt;
        initializeStats(prisoner);
        prisoner.baseMag = Config.MinInt;
        increaseStats(iterations, prisoner);
    }

    private void setConfessorStats(CharaInitParam confessor)
    {
        int iterations = Config.PoolSize - (Config.MinStat * (Const.NumStats - 1));
        iterations -= Config.MinFai;
        initializeStats(confessor);
        confessor.baseFai = Config.MinFai;
        increaseStats(iterations, confessor);
    }
}
