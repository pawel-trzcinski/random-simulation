namespace RandomSimulationEngine.Rest.Throttling.Middlewares
{
    public enum EnqueueStatus
    {
        None = 0,

        AllowExecution,
        QueueFull,
        Cancelled
    }
}