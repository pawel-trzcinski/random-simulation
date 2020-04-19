namespace RandomSimulationEngine.Rest.Throttling.Middlewares
{
    public enum EnqueueStatus
    {
        // ReSharper disable once UnusedMember.Global
        None = 0,

        AllowExecution,
        QueueFull,
        Cancelled
    }
}