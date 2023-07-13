using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ERBingoRandomizer.BHDReader;

public class BHD5Reader {
    private const string CachePath = "./Cache";
    private const string Data0 = "Data0.bhd";
    private const string Data1 = "Data1.bhd";
    private const string Data2 = "Data2.bhd";
    private const string Data3 = "Data3.bhd";
    private const string Data0CachePath = $"{CachePath}/{Data0}";
    private const string Data1CachePath = $"{CachePath}/{Data1}";
    private const string Data2CachePath = $"{CachePath}/{Data2}";
    private const string Data3CachePath = $"{CachePath}/{Data3}";

    private BHD5 _data0;
    private BHD5 _data1;
    private BHD5 _data2;
    private BHD5 _data3;

    private Dictionary<ulong, BHDInfo> _fileDictionary;
    public BHD5Reader(string path) {

        if (!Directory.Exists(CachePath)) {
            Directory.CreateDirectory(CachePath);
        }
        
        List<Task> tasks = new();
        if (!File.Exists(Data0CachePath)) {
            tasks.Add(Task.Run(() => {
                File.WriteAllBytes(Data0CachePath, CryptoUtil.DecryptRsa($"{path}/{Data0}", Const.ArchiveKeys.DATA0).ToArray());
            }));
        }
        if (!File.Exists(Data1CachePath)) {
            tasks.Add(Task.Run(() => {
                File.WriteAllBytes(Data1CachePath, CryptoUtil.DecryptRsa($"{path}/{Data1}", Const.ArchiveKeys.DATA1).ToArray());
            }));
        }
        if (!File.Exists(Data2CachePath)) {
            tasks.Add(Task.Run(() => {
                File.WriteAllBytes(Data2CachePath, CryptoUtil.DecryptRsa($"{path}/{Data2}", Const.ArchiveKeys.DATA2).ToArray());
            }));
        }
        if (!File.Exists(Data3CachePath)) {
            tasks.Add(Task.Run(() => {
                File.WriteAllBytes(Data3CachePath, CryptoUtil.DecryptRsa($"{path}/{Data3}", Const.ArchiveKeys.DATA3).ToArray());
            }));
        }

        Task.WaitAll(tasks.ToArray());

        _data0 = readBHD5(Data0CachePath);
        _data1 = readBHD5(Data1CachePath);
        _data2 = readBHD5(Data2CachePath);
        _data3 = readBHD5(Data3CachePath);

        _fileDictionary = new();
        
        foreach (BHD5.Bucket bucket in _data0.Buckets) {
            foreach (BHD5.FileHeader header in bucket) {
                _fileDictionary.Add(header.FileNameHash, new BHDInfo(_data0, $"{path}/Data0"));
            }
        }
        foreach (BHD5.Bucket bucket in _data1.Buckets) {
            foreach (BHD5.FileHeader header in bucket) {
                _fileDictionary.Add(header.FileNameHash, new BHDInfo(_data1, $"{path}/Data1"));
            }
        }
        foreach (BHD5.Bucket bucket in _data2.Buckets) {
            foreach (BHD5.FileHeader header in bucket) {
                _fileDictionary.Add(header.FileNameHash, new BHDInfo(_data2, $"{path}/Data2"));
            }
        }
        foreach (BHD5.Bucket bucket in _data3.Buckets) {
            foreach (BHD5.FileHeader header in bucket) {
                _fileDictionary.Add(header.FileNameHash, new BHDInfo(_data3, $"{path}/Data3"));
            }
        }
    }
    private static BHD5 readBHD5(string path) {
        using (FileStream fs = new(path, FileMode.Open)) {
            return BHD5.Read(fs, BHD5.Game.EldenRing);
        }
    }
    public byte[]? GetFile(string filePath) {
        ulong hash = Util.ComputeHash(filePath, BHD5.Game.EldenRing);
        if (!_fileDictionary.TryGetValue(hash, out BHDInfo? bhdInfo)) {
            return null;
        }
        return bhdInfo.GetFile(hash);
    }
}
