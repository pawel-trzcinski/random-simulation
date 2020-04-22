using System;
using log4net;
using Newtonsoft.Json;
using RandomSimulationEngine.Configuration.ImageDownload;
using RandomSimulationEngine.Rest.Throttling;

namespace RandomSimulationEngine.Configuration
{
    /// <summary>
    /// Main configuration class for all RandomSimulation bussiness logic.
    /// </summary>
    public class RandomSimulationConfiguration
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(RandomSimulationConfiguration));

        public IThrottlingOptions Throttling { get; }

        public ImageDownloadConfiguration ImageDownload { get; }

        public TasksConfiguration Tasks { get; }

        public HistoryConfiguration History { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSimulationConfiguration"/> class.
        /// </summary>
        [JsonConstructor]
        public RandomSimulationConfiguration(ThrottlingConfiguration throttling, ImageDownloadConfiguration imageDownload, TasksConfiguration tasks, HistoryConfiguration history)
        {
            this.Throttling = throttling;
            this.ImageDownload = imageDownload;
            this.Tasks = tasks;
            this.History = history;

            ValidateConfiguration();
        }

        private void ValidateConfiguration()
        {
            _log.Debug($"Validating {nameof(RandomSimulationConfiguration)}");

            if (Throttling == null)
            {
                throw new ArgumentNullException(nameof(Throttling));
            }

            if (ImageDownload == null)
            {
                throw new ArgumentNullException(nameof(ImageDownload));
            }

            if (Tasks == null)
            {
                throw new ArgumentNullException(nameof(Tasks));
            }

            if (History == null)
            {
                throw new ArgumentNullException(nameof(History));
            }

            _log.Info($"{nameof(RandomSimulationConfiguration)} valid");
        }
    }
}