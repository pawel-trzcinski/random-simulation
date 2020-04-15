namespace RandomSimulationEngine.Tasks
{
    public interface ISingleSourceBytesProvider
    {
#warning TODO - unit test, że jak !IsDataAvailable, to GetBytes zwraca nic
        bool IsDataAvailable { get; }

        /// <summary>
        /// Get really random bytes
        /// </summary>
        /// <param name="count">Array size to fetch</param>
        /// <returns>Empty array if datya is not available</returns>
        byte[] GetBytes(int count);
    }
}