using System;
using RandomSimulationEngine.DateTime;

namespace RandomSimulationEngine.Random
{
    public class RandomService : IRandomService
    {
        public static TimeSpan SeedMinimumLifeTime { get; } = TimeSpan.FromSeconds(5);
        private System.DateTime _seedBirth = System.DateTime.MinValue; // initial seed is always to be replaced

        private System.Random _random = new();

        private readonly object _lockObject = new();

        private readonly IDateTimeService _dateTimeService;

        public RandomService(IDateTimeService dateTimeService)
        {
            _dateTimeService = dateTimeService;
        }

        public void SetSeed(int seed)
        {
            if (!SeedCanBeReplaced())
            {
                return;
            }

            lock (_lockObject)
            {
                if (!SeedCanBeReplaced())
                {
                    return;
                }

                _seedBirth = _dateTimeService.UtcNow;
                _random = new System.Random(seed);
            }
        }

        private bool SeedCanBeReplaced()
        {
            return _seedBirth + SeedMinimumLifeTime < _dateTimeService.UtcNow;
        }

        public int Next()
        {
            return _random.Next();
        }

        public int Next(int max)
        {
            return _random.Next(max);
        }
    }
}