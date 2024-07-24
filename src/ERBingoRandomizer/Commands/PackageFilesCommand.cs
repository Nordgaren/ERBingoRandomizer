using Project.Settings;
using Project.ViewModels;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Project.Commands;

public class PackageFilesCommand : AsyncCommandBase
{
    private readonly MainWindowViewModel _mwViewModel;
    public PackageFilesCommand(MainWindowViewModel mwViewModel)
    {
        _mwViewModel = mwViewModel;
        _mwViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }
    public override bool CanExecute(object? parameter)
    {
        return _mwViewModel.FilesReady && !_mwViewModel.Packaging && !_mwViewModel.InProgress;
    }

    protected override async Task ExecuteAsync(object? parameter)
    {
        try
        {
            _mwViewModel.Packaging = true;
            _mwViewModel.ListBoxDisplay.Clear(); ;
            _mwViewModel.DisplayMessage($"Packaging seed {_mwViewModel.Seed}");
            await Task.Run(PackageFiles);
            Process.Start("explorer.exe", $"{Config.PackagesPath}");
        }
        catch (OperationCanceledException)
        {
            _mwViewModel.DisplayMessage("Packaging Canceled");
        }
        finally
        {
            _mwViewModel.Packaging = false;
        }
    }

    private Task PackageFiles()
    {
        string[] filenames = Directory.GetFiles(Const.ME2Path, "*", SearchOption.AllDirectories);
        Directory.CreateDirectory(Config.PackagesPath);
        using (ZipOutputStream stream = new(File.Create($"{Config.PackagesPath}\\{_mwViewModel.Seed}.zip")))
        {
            byte[] buffer = new byte[4096];
            foreach (string file in filenames)
            {
                ZipEntry entry = new(file.Replace(Const.ME2Path, ""))
                {
                    DateTime = DateTime.Now,
                };

                stream.PutNextEntry(entry);

                using FileStream fs = File.OpenRead(file);
                int sourceBytes;
                do
                {
                    _mwViewModel.CancellationToken.ThrowIfCancellationRequested();
                    sourceBytes = fs.Read(buffer, 0, buffer.Length);
                    stream.Write(buffer, 0, sourceBytes);
                } while (sourceBytes > 0);

            }
        }
        return Task.CompletedTask;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(MainWindowViewModel.FilesReady)
        or nameof(MainWindowViewModel.Packaging)
        or nameof(MainWindowViewModel.InProgress))
        {
            OnCanExecuteChanged();
        }
    }
}
