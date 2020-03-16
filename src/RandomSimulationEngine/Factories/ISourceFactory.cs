using RandomSimulationEngine.Tasks;

namespace RandomSimulationEngine.Factories
{
    public interface ISourceFactory<T>
    {
        ISourceTask GetNewTask(T configuration);
    }
}