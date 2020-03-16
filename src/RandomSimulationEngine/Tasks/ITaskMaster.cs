using System.Threading;

namespace RandomSimulationEngine.Tasks
{
    public interface ITaskMaster
    {
        void Register(IPokableTask pokableTask);
        void StartTasks(CancellationToken cancellationToken);
    }
}