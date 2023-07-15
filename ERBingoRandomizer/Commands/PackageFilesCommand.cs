using ERBingoRandomizer.ViewModels;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ERBingoRandomizer.Commands;

public class PackageFilesCommand : AsyncCommandBase {
    private readonly MainWindowViewModel _mwViewModel;
    public PackageFilesCommand(MainWindowViewModel mwViewModel) {
        _mwViewModel = mwViewModel;
        _mwViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }
    public override bool CanExecute(object? parameter) {
        return false;
        //return _mwViewModel.FilesReady;
    }

    public override async Task ExecuteAsync(object? parameter) {
        throw new NotImplementedException();
        _mwViewModel.Log.Add("Packaging files");

    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(MainWindowViewModel.FilesReady)) {
            OnCanExecuteChanged();
        }
    }
}
