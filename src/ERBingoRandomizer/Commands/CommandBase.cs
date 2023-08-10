using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace ERBingoRandomizer.Commands;

public abstract class CommandBase : ICommand, INotifyPropertyChanged {
    public event EventHandler? CanExecuteChanged;

    public virtual bool CanExecute(object? parameter) {
        return true;
    }

    public abstract void Execute(object? parameter);
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnCanExecuteChanged() {
        Application.Current.Dispatcher.Invoke(() => { CanExecuteChanged?.Invoke(this, EventArgs.Empty); });
    }

    private void OnPropertyChanged([CallerMemberName]string? name = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName]string? propertyName = null) {
        if (EqualityComparer<T>.Default.Equals(field, value)) {
            return false;
        }
        field = value;
        OnPropertyChanged(propertyName ?? "");
        return true;
    }
}
