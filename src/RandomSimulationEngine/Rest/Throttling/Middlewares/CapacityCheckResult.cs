using System.Threading.Tasks;

namespace RandomSimulationEngine.Rest.Throttling.Middlewares
{
    public class CapacityCheckResult
    {
        private static readonly CapacityCheckResult _allowed = new(true, Task.FromResult(EnqueueStatus.AllowExecution));
        private static readonly CapacityCheckResult _cancelled = new(false, Task.FromResult(EnqueueStatus.Cancelled));

        /// <summary>
        /// <b>true</b> if capacity to execute is not excedeed
        /// </summary>
        public bool ExecutionAllowed { get; }

        /// <summary>
        /// Task that waits for until execution is dequeued
        /// </summary>
        public Task<EnqueueStatus> QueueTask { get; }

        private CapacityCheckResult(bool executionAllowed, Task<EnqueueStatus> queueTask)
        {
            ExecutionAllowed = executionAllowed;
            QueueTask = queueTask;
        }

        public static CapacityCheckResult GetAllowed()
        {
            return _allowed;
        }

        public static CapacityCheckResult GetQueued(Task<EnqueueStatus> queueTask)
        {
            return new CapacityCheckResult(false, queueTask);
        }

        public static CapacityCheckResult GetCancelled()
        {
            return _cancelled;
        }
    }
}