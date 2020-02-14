using System;

namespace SeaChange.Prodis.ProdisCommon.Rest.Throttling
{
    public interface IThrottlingOptions
    {
        /// <summary>
        /// Maximum number of concurrently executing requests. Excess calls will be queued.
        /// </summary>
        int ConcurrentRequestsLimit { get; }

        /// <summary>
        /// Maximum number of requests stored in queue. Excess calls will be rejected.
        /// </summary>
        int QueueLimit { get; }

        /// <summary>
        /// Time after which request will be remoevd from queue.
        /// </summary>
        TimeSpan QueueTimeout { get; }

        /// <summary>
        /// Maximum number of network connections Kestrel will allow. It has to be greater or equal to <see cref="ConcurrentRequestsLimit"/>+<see cref="QueueLimit"/>
        /// null means 'unlimited'
        /// </summary>
        long MaximumServerConnections { get; }
    }
}