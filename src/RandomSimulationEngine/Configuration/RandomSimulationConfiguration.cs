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
        }
    }
}