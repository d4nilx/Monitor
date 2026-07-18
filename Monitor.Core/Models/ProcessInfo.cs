namespace Monitor.Core.Models;

public class ProcessInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double RamMb { get; set; }
    public double CpuPercent { get; set; }
}
