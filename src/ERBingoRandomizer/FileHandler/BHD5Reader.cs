using Project.Utility;
using Project.Settings;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Project.FileHandler;
public class BHD5Reader
{
    const uint FileCount = 5;
    private const string DataDLC = "DLC"; // TODO get file name
    private const string Data0 = "Data0";
    //> unused for project
    // private const string Data1 = "Data1";
    // private const string Data2 = "Data2";
    // private const string Data3 = "Data3";
    // private static readonly string Data1CachePath = $"{Config.CachePath}/{Data1}";
    // private static readonly string Data2CachePath = $"{Config.CachePath}/{Data2}";
    // private static readonly string Data3CachePath = $"{Config.CachePath}/{Data3}";
    // private readonly BHDInfo _data1;
    // private readonly BHDInfo _data2;
    // private readonly BHDInfo _data3;
    //^ unused for project
    private static readonly string DlcCachePath = $"{Config.CachePath}/{DataDLC}";
    private static readonly string Data0CachePath = $"{Config.CachePath}/{Data0}";

    private readonly BHDInfo _dataDLC;
    private readonly BHDInfo _data0; // TODO where is this used

    public BHD5Reader(string path, bool cache, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(Config.CachePath)) Directory.CreateDirectory(Config.CachePath);

        bool cacheExists = File.Exists(Data0CachePath) && File.Exists(DlcCachePath);
        byte[][] msbBytes = new byte[FileCount][];
        List<Task> tasks = new();

        if (cacheExists)
        {
            msbBytes[0] = File.ReadAllBytes(Data0CachePath);
            msbBytes[1] = File.ReadAllBytes(DlcCachePath);
        }
        else
        {
            tasks.Add(Task.Run(() =>
            {
                msbBytes[0] = CryptoUtil.DecryptRsa($"{path}/{Data0}.bhd", Const.ArchiveKeys.DATA0, cancellationToken).ToArray();
                msbBytes[1] = CryptoUtil.DecryptRsa($"{path}/{DataDLC}.bhd", Const.ArchiveKeys.DLC, cancellationToken).ToArray();
            }));
        }

        try
        { Task.WaitAll(tasks.ToArray(), cancellationToken); }
        catch (AggregateException)
        { throw; } // TODO maybe have more in depth error handling

        BHD5 data0 = readBHD5(msbBytes[0]);
        BHD5 dlc00 = readBHD5(msbBytes[1]);
        _data0 = new BHDInfo(data0, $"{path}/{Data0}");
        _dataDLC = new BHDInfo(dlc00, $"{path}/{DataDLC}");
        // cancellationToken.ThrowIfCancellationRequested();

        if (cache && !cacheExists)
        {
            File.WriteAllBytes($"{Data0CachePath}.bhd", msbBytes[0]);
            File.WriteAllBytes($"{DlcCachePath}.bhd", msbBytes[1]);
        }
    }
    // This is for cached decrypted BHD5s.
    private static BHD5 readBHD5(string path)
    {
        using FileStream fs = new(path, FileMode.Open);
        return BHD5.Read(fs, BHD5.Game.EldenRing);
    }
    private static BHD5 readBHD5(byte[] bytes)
    {
        using MemoryStream fs = new(bytes);
        return BHD5.Read(fs, BHD5.Game.EldenRing);
    }
    // Right now just works for data0 (needed basegame files)
    // Need to add DLC gear    
    public byte[]? GetFile(string filePath)
    {
        ulong hash = Util.ComputeHash(filePath, BHD5.Game.EldenRing);
        byte[]? file = _data0.GetFile(hash);
        if (file != null)
        {
            Debug.WriteLine($"{filePath} Data0: {_data0.GetSalt()}");
            return file;
        }
        file = _dataDLC.GetFile(hash);
        if (file != null)
        {
            Debug.WriteLine($"{filePath} DLC01: {(_dataDLC.GetSalt)}");
            return file;
        }

        // file = _data1.GetFile(hash);
        // if (file != null) {
        //     Debug.WriteLine($"{filePath} Data1: {_data1.GetSalt()}");
        //     return file;
        // }
        // would continue on for other files, but unneeded for project
        return file;
    }
}
