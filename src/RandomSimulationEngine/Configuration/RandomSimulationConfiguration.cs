using System;
using Newtonsoft.Json;
using RandomSimulationEngine.Configuration.ImageDownload;

namespace RandomSimulationEngine.Configuration
{
    /// <summary>
    /// Main configuration class for all ClassNamer bussiness logic.
    /// </summary>
    public class RandomSimulationConfiguration
    {
#warning TODO - unit tests
        public ThrottlingConfiguration Throttling { get; }

        public ImageDownloadConfiguration ImageDownload { get; }

        public TasksConfiguration Tasks { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSimulationConfiguration"/> class.
        /// </summary>
        [JsonConstructor]
        public RandomSimulationConfiguration(ThrottlingConfiguration throttling, ImageDownloadConfiguration imageDownload, TasksConfiguration tasks)
        {
            this.Throttling = throttling;
            this.ImageDownload = imageDownload;
            this.Tasks = tasks;

            ValidateConfiguration();
        }

        private void ValidateConfiguration()
        {
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
        }
    }
}