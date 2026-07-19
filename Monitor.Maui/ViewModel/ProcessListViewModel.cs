using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monitor.Core.Interfaces;
using Monitor.Core.Models;

namespace Monitor.Maui.ViewModel;

public partial class ProcessListViewModel : ObservableObject
{
    private readonly IProcessService _processService;
    private IDispatcherTimer? _timer;

    private readonly List<double> _cpuHistory = new();
    private readonly List<double> _ramHistory = new();

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private LineChartDrawable? _cpuDrawable;

    [ObservableProperty]
    private LineChartDrawable? _ramDrawable;

    public int TotalCores => Environment.ProcessorCount;
    public string OsVersion => Environment.OSVersion.VersionString;
    public string MachineName => Environment.MachineName;

    public ObservableCollection<ProcessInfo> Processes { get; } = new();

    public ProcessListViewModel(IProcessService processService)
    {
        _processService = processService;
    }

    public void Initialize()
    {
        LoadProcessesCommand.Execute(null);

        _timer = Application.Current!.Dispatcher.CreateTimer();
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

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Processes.Clear();
                foreach (var process in list)
                    Processes.Add(process);

                // CPU — сума по всіх процесах / кількість ядер
                double totalCpu = TotalCores > 0
                    ? Math.Min(100, Processes.Sum(p => p.CpuPercent) / TotalCores)
                    : 0;

                // RAM — сума в GB
                double totalRamGb = Math.Round(Processes.Sum(p => p.RamMb) / 1024.0, 2);

                _cpuHistory.Add(totalCpu);
                _ramHistory.Add(totalRamGb);

                if (_cpuHistory.Count > 20) _cpuHistory.RemoveAt(0);
                if (_ramHistory.Count > 20) _ramHistory.RemoveAt(0);

                // Оновлюємо drawable — GraphicsView перемалює автоматично
                CpuDrawable = new LineChartDrawable(
                    new List<double>(_cpuHistory),
                    Color.FromArgb("#32D74B"),
                    Color.FromArgb("#1A3320"),
                    maxValue: 100
                );

                RamDrawable = new LineChartDrawable(
                    new List<double>(_ramHistory),
                    Color.FromArgb("#FF9F0A"),
                    Color.FromArgb("#331F00"),
                    maxValue: 16 // GB, підстав скільки у тебе є
                );
            });
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
