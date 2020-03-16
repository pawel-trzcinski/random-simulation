using RandomSimulationEngine.Tasks;

namespace RandomSimulationEngine.RandomBytesPuller
{
    public interface IRandomBytesPuller
    {
        void Register(ISingleSourceBytesProvider singleSourceBytesProvider);
        byte[] Pull(int count);
    }
}
