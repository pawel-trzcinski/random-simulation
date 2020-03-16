namespace RandomSimulationEngine.Configuration
{
    public interface ISingleTaskConfiguration<out T>
    {
        T Configuration { get; }
    }
}