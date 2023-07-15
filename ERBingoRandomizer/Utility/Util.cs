using Microsoft.Win32;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace ERBingoRandomizer.Utility;

static class Util {
   
    public static int DeleteFromEnd(int num, int n)
    {
        for (int i = 1; num != 0; i++)
        {
            num /= 10;

            if (i == n)
                return num;
        }

        return 0;
    }
    public static int DeleteFromEndAndRestore(int num, int n) {
        int end = DeleteFromEnd(num, n);
        if (end == 0) {
            return end;
        }
        
        for (int i = 1; end != 0; i++)
        {
            end *= 10;

            if (i == n)
                return end;
        }
        
        return end;
    }
    static readonly (string, string)[] _pathValueTuple = new (string, string)[] {
        (@"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam", "SteamPath"),
        (@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath"),
        (@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath"),
        (@"HKEY_CURRENT_USER\SOFTWARE\Wow6432Node\Valve\Steam", "SteamPath"),
    };

    public static string? TryGetGameInstallLocation(string gamePath) {
        if (!gamePath.StartsWith("\\") && !gamePath.StartsWith("/"))
            return null;

        string? steamPath = GetSteamInstallPath();

        if (string.IsNullOrWhiteSpace(steamPath))
            return null;

        string[] libraryFolders = File.ReadAllLines($@"{steamPath}/SteamApps/libraryfolders.vdf");
        char[] seperator = { '\t' };

        foreach (string line in libraryFolders) {
            if (!line.Contains("\"path\""))
                continue;

            string[] split = line.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            string libPath = split.FirstOrDefault(x => x.ToLower().Contains("steam"))?.Replace("\"", "").Replace("\\\\", "\\") ?? string.Empty;
            string libraryPath = libPath + gamePath;

            if (File.Exists(libraryPath))
                return libraryPath.Replace("\\\\", "\\");
        }

        return null;
    }

    public static string? GetSteamInstallPath() {
        string? installPath = null;

        foreach ((string Path, string Value) pathValueTuple in _pathValueTuple) {
            string registryKey = pathValueTuple.Path;
            installPath = (string?)Registry.GetValue(registryKey, pathValueTuple.Value, null);

            if (installPath != null)
                break;
        }

        return installPath;
    }

    private static string[] _oodleGames = {
        "ELDEN RING",
        "Sekiro",
    };
    public static string? GetOodlePath() {
        foreach (string game in _oodleGames) {
            string? path = TryGetGameInstallLocation($"\\steamapps\\common\\{game}\\Game\\oo2core_6_win64.dll");
            if (path != null)
                return path;
        }

        return null;
    }
    public static string GetEmbeddedResource(string item) {
        string resourceName = $"ERBingoRandomizer.{item}";
        return ReadToString(resourceName);
    }
    public static string[] GetEmbeddedFolder(string item) {
        Assembly assembly = Assembly.GetCallingAssembly();
        string resourceName = $"ERBingoRandomizer.{item}";
        if (!resourceName.EndsWith(".")) {
            resourceName += ".";
        }

        string[] resources = assembly.GetManifestResourceNames();
        return resources.Where(s => s.StartsWith(resourceName)).Select(s => ReadToString(s)).ToArray();
    }
    public static byte[] GetEmbeddedResourceBytes(string item) {
        string resourceName = $"ERBingoRandomizer.{item}";
        return ReadToBytes(resourceName);
    }
    public static byte[][] GetEmbeddedFolderBytes(string item) {
        Assembly assembly = Assembly.GetCallingAssembly();
        string resourceName = $"ERBingoRandomizer.{item}";
        if (!resourceName.EndsWith(".")) {
            resourceName += ".";
        }

        string[] resources = assembly.GetManifestResourceNames();
        List<byte[]> files = new();

        foreach (string res in resources) {
            if (res.StartsWith(resourceName)) {
                files.Add(ReadToBytes(res));
            }
        }
        return files.ToArray();
    }
    private static string ReadToString(string resourceName) {
        Assembly assembly = Assembly.GetCallingAssembly();
        using (Stream? stream = assembly.GetManifestResourceStream(resourceName)) {
            if (stream == null)
                throw new NullReferenceException($"Could not find embedded resource: {resourceName} in the {Assembly.GetCallingAssembly().GetName()} assembly");

            using (StreamReader reader = new(stream)) {
                return reader.ReadToEnd();
            }
        }
    }
    private static byte[] ReadToBytes(string resourceName) {
        Assembly assembly = Assembly.GetCallingAssembly();
        using (Stream? stream = assembly.GetManifestResourceStream(resourceName)) {
            if (stream == null)
                throw new NullReferenceException($"Could not find embedded resource: {resourceName} in the {Assembly.GetCallingAssembly().GetName()} assembly");

            byte[] bytes = new byte[stream.Length];
            int read = stream.Read(bytes);

            return bytes[..read];

        }
    }
    public static PARAMDEF XmlDeserialize(string xml_string) {
        XmlDocument xml = new();
        xml.LoadXml(xml_string);
        return PARAMDEF.XmlSerializer.Deserialize(xml);
    }
    private const uint PRIME = 37;
    private const ulong PRIME64 = 0x85ul;
    public static ulong ComputeHash(string path, BHD5.Game game)
    {
        string hashable = path.Trim().Replace('\\', '/').ToLowerInvariant();
        if (!hashable.StartsWith("/"))
            hashable = '/' + hashable;
        return game >= BHD5.Game.EldenRing ? hashable.Aggregate(0ul, (i, c) => i * PRIME64 + c) : hashable.Aggregate(0u, (i, c) => i * PRIME + c);
    }
    public static string SplitCharacterText(bool useSpaces, List<string> items)
    {
        int lineCount = 3;
        int sizeLimit = 250;
        // Pick some generic serif font to approximate Garamond
        Font f = new("Times New Roman", 12);
        // Hardcode this for the time being
        string delimeter = useSpaces ? ", " : "，";
        List<string> committed = new List<string>();
        foreach (string item in items)
        {
            List<string> cand = committed.ToList();
            if (cand.Count == 0)
            {
                cand.Add("");
            }
            else
            {
                // This adds a space, trim it later if it matters
                cand[cand.Count - 1] += delimeter;
            }
            // Japanese, Chinese, and Thai lack ascii spaces
            // Otherwise, add one word at a time, also measuring the space before.
            foreach (string token in item.Split(' ').Select((s, i) => (i == 0 ? "" : " ") + s))
            {
                string lastLine = cand[cand.Count - 1];
                string addedLine = (lastLine + token).Trim(' ');
                if (TextRenderer.MeasureText(addedLine, f).Width < sizeLimit)
                {
                    cand[cand.Count - 1] = addedLine;
                }
                else
                {
                    cand.Add(token.Trim(' '));
                }
            }
            if (cand.Count > lineCount)
            {
                break;
            }
            committed = cand;
        }
        if (committed.Count == 0)
        {
            // Make each item a line, hope it goes well
            committed = items.Take(lineCount).ToList();
        }
        return string.Join("\n", committed.Select(t => t.Trim(' ')));
    }
}
