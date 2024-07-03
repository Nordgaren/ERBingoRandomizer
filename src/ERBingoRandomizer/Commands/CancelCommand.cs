﻿using Project.Utility;
using Project.ViewModels;
using System.ComponentModel;

namespace Project.Commands;

public class CancelCommand : CommandBase {
    private readonly MainWindowViewModel _mwViewModel;
    public CancelCommand(MainWindowViewModel mwViewModel) {
        _mwViewModel = mwViewModel;
        _mwViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }
    public override bool CanExecute(object? parameter) {
        return _mwViewModel.InProgress || _mwViewModel.Packaging;
    }
    public override void Execute(object? parameter) {
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
