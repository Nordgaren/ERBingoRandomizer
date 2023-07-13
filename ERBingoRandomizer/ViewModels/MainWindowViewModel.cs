using ERBingoRandomizer.Commands;
using System.Windows.Input;

namespace ERBingoRandomizer.ViewModels; 

public class MainWindowViewModel : ViewModelBase {
    private string _seed = string.Empty;
    public string Seed {
        get => _seed;
        set => SetField(ref _seed, value);
    }

    private string? _path;
    public string? Path {
        get => _path;
        set => SetField(ref _path, value);
    }

    private bool _inProgress;
    public bool InProgress {
        get => _inProgress;
        set => SetField(ref _inProgress, value);
    }

    public ICommand RandomizeBingo { get; }
    
    
    
    public MainWindowViewModel() {
        RandomizeBingo = new RandomizeBingoCommand(this);
        Path = Util.TryGetGameInstallLocation("\\steamapps\\common\\ELDEN RING\\Game\\eldenring.exe");
    }
}
