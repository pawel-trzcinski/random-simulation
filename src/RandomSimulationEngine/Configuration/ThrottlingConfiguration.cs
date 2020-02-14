using Newtonsoft.Json;
using SeaChange.Prodis.ProdisCommon.Rest.Throttling;
using System;

namespace RandomSimulationEngine.Configuration
{
    [JsonObject("Throttling")]
    public class ThrottlingConfiguration : IThrottlingOptions
    {
#warning TODO - unit tests
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
        }
    }
}