using System.Collections.Generic;
using System.IO;

namespace ERBingoRandomizer.Utility; 

public static class Logger {
    static Logger() {
        _randomizerLog = new List<string>();
    }
    private static readonly List<string> _randomizerLog;
    public static void LogItem(string item) {
        _randomizerLog.Add(item);
    }
    public static void WriteLog(string seed) {
        Directory.CreateDirectory(Config.SpoilerPath);
        File.WriteAllLines($"{Config.SpoilerPath}/spoiler-{seed}.log", _randomizerLog);
    }
    
}
