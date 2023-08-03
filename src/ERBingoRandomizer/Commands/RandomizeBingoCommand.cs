using ERBingoRandomizer.Randomizer;
using ERBingoRandomizer.ViewModels;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace ERBingoRandomizer.Commands;

public class RandomizeBingoCommand : AsyncCommandBase {
    private readonly MainWindowViewModel _mwViewModel;
    public RandomizeBingoCommand(MainWindowViewModel mwViewModel) {
        _mwViewModel = mwViewModel;
        _mwViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }
    public override bool CanExecute(object? parameter) {
        return !_mwViewModel.InProgress
            && _mwViewModel.LastSeed?.Seed != _mwViewModel.Seed
            && !string.IsNullOrWhiteSpace(_mwViewModel.Path)
            && _mwViewModel.Path.ToLower().EndsWith(Const.ExeName)
            && File.Exists(_mwViewModel.Path);
    }
    protected override async Task ExecuteAsync(object? parameter) {
        _mwViewModel.ListBoxDisplay.Clear();;
        _mwViewModel.DisplayMessage("Randomizing Elden Ring Regulation");
        _mwViewModel.InProgress = true;
        _mwViewModel.RandoButtonText = "Cancel";
        // _mwViewModel.Path is not null, and is a valid path to eldenring.exe, because of the conditions in CanExecute.
        try {
            BingoRandomizer randomizer = await BingoRandomizer.BuildRandomizerAsync(_mwViewModel.Path!, _mwViewModel.Seed, _mwViewModel.CancellationToken);
            await Task.Run(() => randomizer.RandomizeRegulation());
            _mwViewModel.LastSeed = randomizer.SeedInfo;
            _mwViewModel.FilesReady = true;
            _mwViewModel.DisplayMessage($"Randomization Finished. Seed: {randomizer.SeedInfo.Seed}");
        }
        catch (OperationCanceledException) {
            _mwViewModel.DisplayMessage("Randomization Canceled");
        }
        finally {
            _mwViewModel.RandoButtonText = "Randomize!";
            _mwViewModel.InProgress = false;
        }
    }
    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(MainWindowViewModel.InProgress)
            or nameof(MainWindowViewModel.Path)
            or nameof(MainWindowViewModel.LastSeed)
            or nameof(MainWindowViewModel.Seed)
            or nameof(MainWindowViewModel.FilesReady)) {
            OnCanExecuteChanged();
        }
    }
}
