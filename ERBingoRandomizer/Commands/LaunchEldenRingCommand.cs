using ERBingoRandomizer.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using static ERBingoRandomizer.Utility.Const;

namespace ERBingoRandomizer.Commands; 

public class LaunchEldenRingCommand : CommandBase {
    private readonly MainWindowViewModel _mwViewModel;
    public LaunchEldenRingCommand(MainWindowViewModel mwViewModel) {
        _mwViewModel = mwViewModel;
        _mwViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }
    
    public override bool CanExecute(object? parameter) {
        return _mwViewModel.FilesReady;
    }
    
    public override void Execute(object? parameter) {
        _mwViewModel.Log.Add("Launching Elden Ring via ModEngine 2");
        Process me2 = new() {
            StartInfo = new ProcessStartInfo {
                FileName = "launchmod_bingo.bat",
                WorkingDirectory = ME2Path,
                UseShellExecute = true,
                CreateNoWindow = true,
            }
        };

        me2.Start();
    }
    
    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(MainWindowViewModel.FilesReady)) {
            OnCanExecuteChanged();
        }
    }
}
