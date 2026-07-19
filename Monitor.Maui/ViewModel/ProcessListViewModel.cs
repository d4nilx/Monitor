using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monitor.Core.Interfaces;
using Monitor.Core.Models;

namespace Monitor.Maui.ViewModel;

public partial class ProcessListViewModel : ObservableObject
{
    private readonly IProcessService _processService;
    private IDispatcherTimer _timer;

    [ObservableProperty]
    private bool _isBusy;
    public int TotalCores => Environment.ProcessorCount; // Counts the cores | threads of processor
    public string OsVersion => Environment.OSVersion.VersionString;
    public string MachineName => Environment.MachineName;

    public ObservableCollection<ProcessInfo> Processes { get; } = new();

    public ProcessListViewModel(IProcessService processService)
    {
        _processService = processService;
        LoadProcessesCommand.Execute(null);
        _timer = Application.Current.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(3);
        _timer.Tick += (s, e) => 
        {
            if (!IsBusy) LoadProcessesCommand.Execute(null);
        };
        _timer.Start();
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
