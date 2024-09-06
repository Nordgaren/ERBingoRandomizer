using Microsoft.Win32;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml;
using System.Windows.Forms; // TextRenderer
using Project.Settings;

namespace Project.Utility;

static class Util
{
    private const uint PRIME = 37;
    private const ulong PRIME64 = 0x85ul;
    private static readonly (string, string)[] _pathValueTuple = {
        (@"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam", "SteamPath"),
        (@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath"),
        (@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath"),
        (@"HKEY_CURRENT_USER\SOFTWARE\Wow6432Node\Valve\Steam", "SteamPath"),
    };

    private static readonly string[] _oodleGames = {
        "ELDEN RING",
        "Sekiro",
    };
    private static int DeleteFromEnd(int num, int n)
    {
        for (int i = 1; num != 0; i++)
        {
            num /= 10;
            if (i == n) { return num; }
        }
        return 0;
    }
    public static int DeleteFromEndAndRestore(int num, int n)
    {
        int end = DeleteFromEnd(num, n);
        if (end == 0) { return end; }

        for (int i = 1; end != 0; i++)
        {
            end *= 10;
            if (i == n) { return end; }
        }
        return end;
    }

    public static string? TryGetGameInstallLocation(string gamePath)
    {
        if (!gamePath.StartsWith("\\") && !gamePath.StartsWith("/"))
        { return null; }

        string? steamPath = GetSteamInstallPath();

        if (string.IsNullOrWhiteSpace(steamPath))
        { return null; }

        string[] libraryFolders = File.ReadAllLines($@"{steamPath}/SteamApps/libraryfolders.vdf");
        char[] separator = { '\t' };

        foreach (string line in libraryFolders)
        {
            if (!line.Contains("\"path\""))
            { continue; }

            string[] split = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            string libPath = split.FirstOrDefault(x => x.ToLower().Contains("steam"))?.Replace("\"", "").Replace("\\\\", "\\") ?? string.Empty;
            string libraryPath = libPath + gamePath;

            if (File.Exists(libraryPath))
            { return libraryPath.Replace("\\\\", "\\"); }
        }

        return null;
    }

    private static string? GetSteamInstallPath()
    {
        string? installPath = null;

        foreach ((string Path, string Value) pathValueTuple in _pathValueTuple)
        {
            string registryKey = pathValueTuple.Path;
            installPath = (string?)Registry.GetValue(registryKey, pathValueTuple.Value, null);

            if (installPath != null)
            { break; }
        }

        return installPath;
    }
    public static string? GetOodlePath()
    {
        foreach (string game in _oodleGames)
        {
            string? path = TryGetGameInstallLocation($"\\steamapps\\common\\{game}\\Game\\oo2core_6_win64.dll");
            if (path != null)
            { return path; }
        }
        return null;
    }
    public static string[] GetResourcesInFolder(string item)
    {
        string[] resources = Directory.GetFiles($"{Config.ResourcesPath}/{item}");
        return resources.Select(ReadToString).ToArray();
    }
    private static string ReadToString(string resourceName) { return File.ReadAllText(resourceName); }

    public static PARAMDEF XmlDeserialize(string xmlString)
    {
        XmlDocument xml = new();
        xml.LoadXml(xmlString);
        return PARAMDEF.XmlSerializer.Deserialize(xml);
    }
    public static ulong ComputeHash(string path, BHD5.Game game)
    {
        string hashable = path.Trim().Replace('\\', '/').ToLowerInvariant();
        if (!hashable.StartsWith("/"))
        { hashable = '/' + hashable; }

        return game >= BHD5.Game.EldenRing ? hashable.Aggregate(0ul, (i, c) => i * PRIME64 + c) : hashable.Aggregate(0u, (i, c) => i * PRIME + c);
    }

    private static string GetFileSha256Hash(string path)
    {
        if (File.Exists(path))
        {
            byte[] file = File.ReadAllBytes(path);
            byte[] hash = SHA256.HashData(file);
            return BitConverter.ToString(hash).Replace("-", "");
        }
        return string.Empty;
    }
    public static string GetShaRegulation256Hash() { return GetFileSha256Hash($"{Const.BingoPath}/{Const.RegulationName}"); }
}
