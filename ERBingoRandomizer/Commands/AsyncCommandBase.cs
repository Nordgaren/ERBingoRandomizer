using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERBingoRandomizer.Commands;

public abstract class AsyncCommandBase : CommandBase {

    public override async void Execute(object? parameter) {
        await ExecuteAsync(parameter);
    }

    public abstract Task ExecuteAsync(object? parameter);


}
