using Newtonsoft.Json;
using System;
using RandomSimulationEngine.Rest.Throttling;

namespace RandomSimulationEngine.Configuration
{
    [JsonObject(nameof(RandomSimulationConfiguration.Throttling))]
    public class ThrottlingConfiguration : IThrottlingOptions
    {
        public int ConcurrentRequestsLimit { get; }
        public int QueueLimit { get; }
        public TimeSpan QueueTimeout { get; }
        public long MaximumServerConnections { get; }

        [JsonConstructor]
        public ThrottlingConfiguration(int concurrentRequestsLimit, int queueLimit, int queueTimeoutS, int maximumServerConnections)
        {
            ConcurrentRequestsLimit = concurrentRequestsLimit;
            QueueLimit = queueLimit;
            QueueTimeout = TimeSpan.FromSeconds(queueTimeoutS);
            MaximumServerConnections = maximumServerConnections;

            ValidateConfiguration();
        }

        private void ValidateConfiguration()
        {
            if (ConcurrentRequestsLimit < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(ConcurrentRequestsLimit), $"{nameof(ConcurrentRequestsLimit)} has to be greater than 0");
            }

            if (QueueLimit < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(QueueLimit), $"{nameof(QueueLimit)} has to be greater or equal to 0");
            }

            if (QueueTimeout.TotalSeconds < 5)
            {
                throw new ArgumentOutOfRangeException(nameof(QueueTimeout), $"{nameof(QueueTimeout)} has to be greater or equal to 5");
            }

            if (MaximumServerConnections < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(MaximumServerConnections), $"{nameof(MaximumServerConnections)} has to be greater than 0");
            }

            if (MaximumServerConnections < ConcurrentRequestsLimit + QueueLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(MaximumServerConnections), $"{nameof(MaximumServerConnections)} has to be greater or equal to the sum of {nameof(ConcurrentRequestsLimit)} and {nameof(QueueLimit)}");
            }
        }
    }
}