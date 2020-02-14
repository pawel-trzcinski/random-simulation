namespace RandomSimulationEngine.ValueCalculator
{
    public interface IValueCalculator
    {
        int GetInt32(byte[] bytes);
        int GetInt32(byte[] bytes, int min);
        int GetInt32(byte[] bytes, int min, int max);

        double GetDouble(byte[] bytes);
    }
}