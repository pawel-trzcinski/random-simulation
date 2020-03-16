using System.Collections.Generic;

namespace RandomSimulationEngine.Configuration
{
    public class ITasksConfiguration<T>
    {
        private IEnumerable<ISingleTaskConfiguration<T>> Tasks { get; }
    }
}