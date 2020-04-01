using RandomSimulationEngine.Health;

namespace RandomSimulationEngine.Tasks
{
    public interface ISourceTask : IPokableTask, ISingleSourceBytesProvider, IHealthProvider
    {
    }
}