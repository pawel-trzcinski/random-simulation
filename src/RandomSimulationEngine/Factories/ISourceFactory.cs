using RandomSimulationEngine.Tasks;

namespace RandomSimulationEngine.Factories
{
    public interface ISourceFactory<in T>
    {
        ISourceTask GetNewTask(T configuration);
    }
}