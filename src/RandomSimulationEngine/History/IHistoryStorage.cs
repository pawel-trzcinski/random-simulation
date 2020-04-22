namespace RandomSimulationEngine.History
{
    public interface IHistoryStorage
    {
        void StoreNext(int next);
        void StoreNextMax(int next);
        void StoreNextMinMax(int next);
        void StoreNextDouble(double next);
        void StoreNextBytes(byte[] bytes);

        string GetHistogramReport(int bucketsCount);
    }
}