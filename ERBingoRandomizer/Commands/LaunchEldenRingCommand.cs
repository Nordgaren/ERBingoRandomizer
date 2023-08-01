using ERBingoRandomizer.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using static ERBingoRandomizer.Const;

namespace ERBingoRandomizer.Commands;

public class LaunchEldenRingCommand : CommandBase {
    private readonly MainWindowViewModel _mwViewModel;
    public LaunchEldenRingCommand(MainWindowViewModel mwViewModel) {
        _mwViewModel = mwViewModel;
        _mwViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    public override bool CanExecute(object? parameter) {
        return _mwViewModel.FilesReady && !_mwViewModel.InProgress;
    }

    public override void Execute(object? parameter) {
        if (eldenRingIsOpen()) {
            _mwViewModel.DisplayMessage("Elden Ring is still open. Please close Elden Ring or wait for it to full exit.");
            return;
        }
        _mwViewModel.ListBoxDisplay.Clear();
        _mwViewModel.DisplayMessage("Launching Elden Ring via ModEngine 2");
        launchEldenRing();
    }
    private static void launchEldenRing() {

        Process me2 = new() {
            StartInfo = new ProcessStartInfo {
                FileName = "launchmod_bingo.bat",
                WorkingDirectory = ME2Path,
                UseShellExecute = true,
                CreateNoWindow = true,
            },
        };

        me2.Start();
    }
    private bool eldenRingIsOpen() {
        Process[] processes = Process.GetProcesses();
        foreach (Process process in processes) {
            if (process.ProcessName is "eldenring") {
                if (process.HasExited) {
                    _mwViewModel.DisplayMessage("Bingus!");
                }
                return true;
            }
        }
        return false;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(MainWindowViewModel.FilesReady)
            or nameof(MainWindowViewModel.InProgress)) {
            OnCanExecuteChanged();
        }
    }
}
