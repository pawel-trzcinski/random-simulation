using System.Linq;
using RandomSimulationEngine.Structures;

namespace RandomSimulation.Tests.Structures
{
    public class QueueTester : ConcurrentLimitedByteQueue
    {
        public byte[] Queue => _queue;

        public QueueTester(byte[] queue, int firstIndex, int lastIndex)
            : base(queue, queue.Count(p => p != 0), firstIndex, lastIndex)
        {
        }
    }
}