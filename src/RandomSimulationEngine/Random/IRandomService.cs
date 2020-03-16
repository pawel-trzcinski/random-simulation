namespace RandomSimulationEngine.Random
{
    public interface IRandomService
    {
        void SetSeed(int seed);

        int Next();
        int Next(int max);
    }
}