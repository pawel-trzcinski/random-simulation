namespace RandomSimulationEngine.Structures
{
    public interface IConcurrentLimitedByteQueue
    {
        /// <summary>
        /// Number of elements currently in the queue
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Maximum elements count
        /// </summary>
        int Capacity { get; }

        void Enqueue(byte b);
        void Enqueue(byte[] bytes);

        /// <summary>
        /// Tries do fetch specified number of bytes.
        /// </summary>
        /// <param name="count">Numbers of bytes to be fetched</param>
        /// <param name="bytes">Array of bytes dequeued</param>
        /// <returns><b>true</b> if operation was a success</returns>
        bool TryFetch(int count, out byte[] bytes);
    }
}