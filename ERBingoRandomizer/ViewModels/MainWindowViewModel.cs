using ERBingoRandomizer.Commands;
using ERBingoRandomizer.Randomizer;
using ERBingoRandomizer.Utility;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;
using static ERBingoRandomizer.Utility.Const;
using static ERBingoRandomizer.Utility.Config;

namespace ERBingoRandomizer.ViewModels;

public class MainWindowViewModel : ViewModelBase, IDisposable {
    private readonly FileSystemWatcher _watcher;
    public MainWindowViewModel() {
        RandomizeBingo = new RandomizeBingoCommand(this);
        LaunchEldenRing = new LaunchEldenRingCommand(this);
        PackageFiles = new PackageFilesCommand(this);
        Cancel = new CancelCommand(this);
        FilesReady = AllFilesReady();
        if (FilesReady) {
            LastSeed = File.Exists(LastSeedPath) ? JsonSerializer.Deserialize<SeedInfo>(File.ReadAllText(LastSeedPath)) : null;
        }
        ListBoxDisplay = new ObservableCollection<string>();
        DisplayView = CollectionViewSource.GetDefaultView(ListBoxDisplay);
        GetNewCancellationToken();
        _watcher = new FileSystemWatcher(ME2Path);
        _watcher.NotifyFilter = NotifyFilters.Attributes
            | NotifyFilters.CreationTime
            | NotifyFilters.DirectoryName
            | NotifyFilters.FileName
            | NotifyFilters.LastAccess
            | NotifyFilters.LastWrite
            | NotifyFilters.Size;

        _watcher.Changed += OnChanged;
        _watcher.Created += OnCreated;
        _watcher.Deleted += OnDeleted;
        _watcher.Renamed += OnRenamed;
        _watcher.Error += OnError;

        _watcher.Filter = "*";
        _watcher.IncludeSubdirectories = true;
        _watcher.EnableRaisingEvents = true;
    }
    private string _seed = string.Empty;
    public string Seed {
        get => _seed;
        set => SetField(ref _seed, value);
    }
    private string? _path = Util.TryGetGameInstallLocation("\\steamapps\\common\\ELDEN RING\\Game\\eldenring.exe");
    public string? Path {
        get => _path;
        set => SetField(ref _path, value);
    }
    private string _randoButtonText = "Randomize";
    public string? RandoButtonText {
        get => _randoButtonText;
        set => SetField(ref _randoButtonText, value);
    }
    private bool _inProgress;
    public bool InProgress {
        get => _inProgress;
        set {
            if (SetField(ref _inProgress, value)) {
                _watcher.EnableRaisingEvents = !_inProgress;
            }
        }
    }
    private bool _packaging;
    public bool Packaging {
        get => _packaging;
        set {
            if (SetField(ref _packaging, value)) {
                _watcher.EnableRaisingEvents = !_packaging;
            }
        }
    }
    private bool _filesReady;
    public bool FilesReady {
        get => _filesReady;
        set => SetField(ref _filesReady, value);
    }

    public ICommand RandomizeBingo { get; }
    public ICommand LaunchEldenRing { get; }
    public ICommand PackageFiles { get; }
    public ICommand Cancel { get; }
    private ObservableCollection<string> _listBoxDisplay;
    public ObservableCollection<string> ListBoxDisplay {
        get => _listBoxDisplay;
        set {
            if (SetField(ref _listBoxDisplay, value)) {
                OnPropertyChanged(nameof(DisplayView));
            }
        }
    }
    public void DisplayMessage(string message) {
        ListBoxDisplay.Add(message);
    }
    public ICollectionView DisplayView { get; }

    public CancellationTokenSource CancellationTokenSource { get; private set; }
    public CancellationToken CancellationToken { get; private set; }
    public string LastSeedText => string.IsNullOrWhiteSpace(Seed) ? "Unknown Seed" : Seed;
    private SeedInfo? _lastSeed;
    public SeedInfo? LastSeed {
        get => _lastSeed;
        set {
            Seed = value?.Seed ?? string.Empty;
            if (SetField(ref _lastSeed, value)) {
                OnPropertyChanged(nameof(LastSeedText));
            }
        }
    }
    public void Dispose() {
        _watcher.Dispose();
    }
    private void OnChanged(object sender, FileSystemEventArgs e) {
        if (e.ChangeType != WatcherChangeTypes.Changed) {
            return;
        }
        FilesReady = AllFilesReady();
    }

    private void OnCreated(object sender, FileSystemEventArgs e) {
        FilesReady = AllFilesReady();
        if (FilesReady && LastSeed == null && File.Exists(LastSeedPath)) {
            LastSeed = JsonSerializer.Deserialize<SeedInfo>(File.ReadAllText(LastSeedPath));
        }
    }
    private static bool AllFilesReady() {
        return File.Exists(BingoRegulationPath) && File.Exists($"{BingoPath}{MenuMsgBNDPath}");
    }

    private void OnDeleted(object sender, FileSystemEventArgs e) {
        if (!Directory.Exists(BingoPath)) {
            LastSeed = null;
            return;
        }
        FilesReady = AllFilesReady();
        if (!FilesReady) {
            LastSeed = null;
        }
    }

    private void OnRenamed(object sender, RenamedEventArgs e) {
        FilesReady = AllFilesReady();
    }

    private void OnError(object sender, ErrorEventArgs e) {
        PrintException(e.GetException());
    }

    private static void PrintException(Exception? ex) {
        if (ex != null) {
            Debug.WriteLine($"Message: {ex.Message}");
            Debug.WriteLine("Stacktrace:");
            Debug.WriteLine(ex.StackTrace);
            Debug.WriteLine("");
            PrintException(ex.InnerException);
        }
    }
    public void GetNewCancellationToken() {
        CancellationTokenSource = new CancellationTokenSource();
        CancellationToken = CancellationTokenSource.Token;
        CancellationToken.Register(GetNewCancellationToken);
    }
    private void CancelCalled() {
        GetNewCancellationToken();
    }
}
