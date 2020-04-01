namespace RandomSimulationEngine.Health
{
    public interface IHealthChecker
    {
        void Register(IHealthProvider healthProvider);
        HealthStatus GetHealthStatus();
    }
}