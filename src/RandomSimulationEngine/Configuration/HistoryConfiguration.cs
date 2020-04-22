using System;
using log4net;
using Newtonsoft.Json;

namespace RandomSimulationEngine.Configuration
{
    public class HistoryConfiguration
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(HistoryConfiguration));

        public const int MIN_SAMPLES = 10;
        public const int MAX_SAMPLES = 60 * 1000;

        public int IntSamples { get; }
        public int DoubleSamples { get; }
        public int ByteSamples { get; }

        [JsonConstructor]
        public HistoryConfiguration(int intSamples, int doubleSamples, int byteSamples)
        {
            this.IntSamples = intSamples;
            this.DoubleSamples = doubleSamples;
            this.ByteSamples = byteSamples;

            ValidateConfiguration();
        }

        private void ValidateConfiguration()
        {
            _log.Debug($"Validating {nameof(HistoryConfiguration)}");

            if (IntSamples < MIN_SAMPLES || IntSamples > MAX_SAMPLES)
            {
                throw new ArgumentOutOfRangeException(nameof(IntSamples), $"{nameof(IntSamples)} has to be in range <{MIN_SAMPLES}, {MAX_SAMPLES}>");
            }

            if (DoubleSamples < MIN_SAMPLES || DoubleSamples > MAX_SAMPLES)
            {
                throw new ArgumentOutOfRangeException(nameof(DoubleSamples), $"{nameof(DoubleSamples)} has to be in range <{MIN_SAMPLES}, {MAX_SAMPLES}>");
            }

            if (ByteSamples < MIN_SAMPLES || ByteSamples > MAX_SAMPLES)
            {
                throw new ArgumentOutOfRangeException(nameof(IntSamples), $"{nameof(IntSamples)} has to be in range <{MIN_SAMPLES}, {MAX_SAMPLES}>");
            }

            _log.Info($"{nameof(HistoryConfiguration)} valid");
        }
    }
}