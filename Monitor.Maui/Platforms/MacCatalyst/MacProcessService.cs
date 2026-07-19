using System.Diagnostics;
using Monitor.Core.Interfaces;
using Monitor.Core.Models;

namespace Monitor.Maui.Platforms.MacCatalyst;

public class MacProcessService : IProcessService
{
    public List<ProcessInfo> GetProcesses()
    {
        var processList = new List<ProcessInfo>();

        try
        {
            var psi = new ProcessStartInfo
            {
                // here the 'command' instead of 'com' gives us full path to file of program 
                FileName = "/bin/ps",
                Arguments = "-axo pid,pcpu,rss,command", 
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            using var reader = process.StandardOutput;
            string output = reader.ReadToEnd();

            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Trim().Split(new[] { ' ' }, 4, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 4) continue;

                if (int.TryParse(parts[0], out int pid) &&
                    double.TryParse(parts[1], out double cpu) &&
                    double.TryParse(parts[2], out double rss))
                {
                    string fullPath = parts[3];
                    string cleanName = Path.GetFileName(fullPath);
                    
                    // === Smart categorization of processes based on their names ===
                    string category = "Background processes";
                    if (fullPath.StartsWith("/Applications/"))
                        category = "My apps";
                    else if (fullPath.StartsWith("/System/") || fullPath.StartsWith("/usr/"))
                        category = "System macOS";
                    if (rss /  1024.00 < 5.0 && category == "System macOS")
                        continue;

                    processList.Add(new ProcessInfo
                    {
                        Id = pid,
                        CpuPercent = Math.Round(cpu, 1), 
                        RamMb = Math.Round(rss / 1024.0, 1), 
                        Name = cleanName,
                        Category = category
                    });
                }
            }
        }
        catch { } // just ignore

        return processList.OrderByDescending(p => p.RamMb).ToList();
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