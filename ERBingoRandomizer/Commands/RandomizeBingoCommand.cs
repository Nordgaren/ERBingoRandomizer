using ERBingoRandomizer.Randomizer;
using ERBingoRandomizer.ViewModels;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using static ERBingoRandomizer.Const;

namespace ERBingoRandomizer.Commands;

public class RandomizeBingoCommand : AsyncCommandBase {
    private readonly MainWindowViewModel _mwViewModel;
    public RandomizeBingoCommand(MainWindowViewModel mwViewModel) {
        _mwViewModel = mwViewModel;
        _mwViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }
    public override bool CanExecute(object? parameter) {
        return !_mwViewModel.InProgress 
            && !string.IsNullOrWhiteSpace(_mwViewModel.Path) 
            && _mwViewModel.Path.ToLower().EndsWith(ExeName) 
            && File.Exists(_mwViewModel.Path);
    }
    public override async Task ExecuteAsync(object? parameter) {
        _mwViewModel.InProgress = true;
        _mwViewModel.Seed = string.IsNullOrWhiteSpace(_mwViewModel.Seed) ? Random.Shared.NextInt64().ToString() : _mwViewModel.Seed.Trim();
        // _mwViewModel.Path is not null, and is a valid path to eldenring.exe, because of the conditions in CanExecute.
        BingoRandomizer randomizer = await BingoRandomizer.BuildRandomizerAsync(_mwViewModel.Path!, _mwViewModel.Seed);
        await Task.Run(() => randomizer.RandomizeRegulation());
        _mwViewModel.InProgress = false;
    }
    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(MainWindowViewModel.InProgress)
            or nameof(MainWindowViewModel.Path)) {
            OnCanExecuteChanged();
        }
    }
}
