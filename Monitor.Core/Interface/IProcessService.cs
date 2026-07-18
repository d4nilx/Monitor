using Monitor.Core.Models;

namespace Monitor.Core.Interfaces;

public interface IProcessService
{
    List<ProcessInfo> GetProcesses();
    bool KillProcess(int processId);
}
