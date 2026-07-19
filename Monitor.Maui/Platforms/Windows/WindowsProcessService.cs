using System.Diagnostics;
using Monitor.Core.Interfaces;
using Monitor.Core.Models;

namespace Monitor.Maui.Platforms.Windows;

public class WindowsProcessService : IProcessService
{
    public List<ProcessInfo> GetProcesses()
    {
        var processelist = new List<ProcessInfo>();
        
        // Here it takes all processes that currently working on the Windows
        var processes = Process.GetProcesses();

        foreach (var p in processes)
        {
            try
            {
                processelist.Add(new ProcessInfo
                {
                    Id = p.Id,
                    Name = p.ProcessName,
                    // WorkingSet64 return bite. We are dividing it two time by 1024 to get the value in MB
                    RamMb = Math.Round(p.WorkingSet64 / 1024.0 / 1024.0, 1),
                    CpuPercent = 0, // CPU usage is not calculated here, as it requires more complex logic to measure CPU usage over time
                    Category = "Windows App" // You can implement your own logic to categorize processes if needed
                });
            }
            catch 
            {
                // in case there's any problem from Windows system we just skip it
            } 
        }
        // Sort it by the value
        return processelist.OrderByDescending(p => p.RamMb).ToList();
    }
    public bool KillProcess(int processId)
    {
        try
        {
            var process = Process.GetProcessById(processId);
            process.Kill();
            return true;
        }
        catch 
        { 
            return false; 
        }
    }
}