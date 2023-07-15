using ERBingoRandomizer.ViewModels;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ERBingoRandomizer.Commands;

public class CancelRandomizeBingoCommand : AsyncCommandBase {
    private readonly MainWindowViewModel _mwViewModel;
    public CancelRandomizeBingoCommand(MainWindowViewModel mwViewModel) {
        _mwViewModel = mwViewModel;
        _mwViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }
    public override bool CanExecute(object? parameter) {
        return _mwViewModel.InProgress;
    }
    public override async Task ExecuteAsync(object? parameter) {
        _mwViewModel.Log.Add("Cancelling Task");
        _mwViewModel.CancellationTokenSource.Cancel();
    }
    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(MainWindowViewModel.InProgress)) {
            OnCanExecuteChanged();
        }
    }
}
