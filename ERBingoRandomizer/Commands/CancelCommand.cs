using ERBingoRandomizer.ViewModels;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ERBingoRandomizer.Commands;

public class CancelCommand : AsyncCommandBase {
    private readonly MainWindowViewModel _mwViewModel;
    public CancelCommand(MainWindowViewModel mwViewModel) {
        _mwViewModel = mwViewModel;
        _mwViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }
    public override bool CanExecute(object? parameter) {
        return _mwViewModel.InProgress || _mwViewModel.Packaging;
    }
    public override async Task ExecuteAsync(object? parameter) {
        _mwViewModel.DisplayMessage("Cancelling Task");
        _mwViewModel.CancellationTokenSource.Cancel();
    }
    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(MainWindowViewModel.InProgress)
        or nameof(MainWindowViewModel.Packaging)) {
            OnCanExecuteChanged();
        }
    }
}
