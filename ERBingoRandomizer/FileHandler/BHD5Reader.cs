using ERBingoRandomizer.Utility;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ERBingoRandomizer.FileHandler;

public class BHD5Reader {
    private const string Data0 = "Data0";
    private const string Data1 = "Data1";
    private const string Data2 = "Data2";
    private const string Data3 = "Data3";
    private static readonly string Data0CachePath = $"{Config.CachePath}/{Data0}";
    private static readonly string Data1CachePath = $"{Config.CachePath}/{Data1}";
    private static readonly string Data2CachePath = $"{Config.CachePath}/{Data2}";
    private static readonly string Data3CachePath = $"{Config.CachePath}/{Data3}";

    private readonly BHDInfo _data0;
    private readonly BHDInfo _data1;
    private readonly BHDInfo _data2;
    private readonly BHDInfo _data3;

    public BHD5Reader(string path, bool cache, CancellationToken cancellationToken) {
            
        if (!Directory.Exists(Config.CachePath)) {
            Directory.CreateDirectory(Config.CachePath);
        }

        bool cacheExists = File.Exists(Data0CachePath);
        byte[][] msbBytes = new byte[4][];
        List<Task> tasks = new();
        switch (cacheExists) {
            case false:
                tasks.Add(Task.Run(() => { msbBytes[0] = CryptoUtil.DecryptRsa($"{path}/{Data0}.bhd", Const.ArchiveKeys.DATA0, cancellationToken).ToArray(); }));
                break;
            default:
                msbBytes[0] = File.ReadAllBytes(Data0CachePath);
                break;
        }
        
        
        try {
            Task.WaitAll(tasks.ToArray(), cancellationToken);
        }
        catch (AggregateException) {
            if (!cancellationToken.IsCancellationRequested) {
                throw;
            }
            cancellationToken.ThrowIfCancellationRequested();
        }

        BHD5 data0 = readBHD5(msbBytes[0]);
        _data0 = new BHDInfo(data0, $"{path}/{Data0}");
        cancellationToken.ThrowIfCancellationRequested();

        if (cache && !cacheExists) {
            File.WriteAllBytes($"{Data0CachePath}.bhd", msbBytes[0]);
        }
    }
    // This is for cached decrypted BHD5s.
    private static BHD5 readBHD5(string path) {
        using FileStream fs = new(path, FileMode.Open);
        return BHD5.Read(fs, BHD5.Game.EldenRing);
    }
    private static BHD5 readBHD5(byte[] bytes) {
        using MemoryStream fs = new(bytes);
        return BHD5.Read(fs, BHD5.Game.EldenRing);
    }
    // Right now just works for data0, as that is where all of the files we need, are.  
    public byte[]? GetFile(string filePath) {
        ulong hash = Util.ComputeHash(filePath, BHD5.Game.EldenRing);
        Debug.WriteLine($"{filePath} : {_data0.GetSalt()}");
        return _data0.GetFile(hash);
    }
}
