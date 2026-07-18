using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monitor.Core.Interfaces;
using Monitor.Core.Models;

namespace Monitor.Maui.ViewModel;

public partial class ProcessListViewModel : ObservableObject
{
    private readonly IProcessService _processService;

    [ObservableProperty]
    private bool _isBusy;

    public ObservableCollection<ProcessInfo> Processes { get; } = new();

    public ProcessListViewModel(IProcessService processService)
    {
        _processService = processService;
        LoadProcessesCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadProcessesAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var list = await Task.Run(() => _processService.GetProcesses());

            Processes.Clear();
            foreach (var process in list)
                Processes.Add(process);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task KillProcessAsync(int processId)
    {
        var success = await Task.Run(() => _processService.KillProcess(processId));
        if (success)
            await LoadProcessesAsync();
    }
}
