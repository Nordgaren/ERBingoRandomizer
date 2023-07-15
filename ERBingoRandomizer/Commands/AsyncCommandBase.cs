using System.Threading.Tasks;

namespace ERBingoRandomizer.Commands;

public abstract class AsyncCommandBase : CommandBase {

    public override async void Execute(object? parameter) {
        await ExecuteAsync(parameter);
    }

    public abstract Task ExecuteAsync(object? parameter);


}
