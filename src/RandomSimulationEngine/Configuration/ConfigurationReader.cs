using System.IO;
using log4net;
using Newtonsoft.Json.Linq;

namespace RandomSimulationEngine.Configuration
{
    /// <summary>
    /// Default implementation of <see cref="IConfigurationReader"/>.
    /// Reads implementation from appsettings.json that is in the current working folder.
    /// </summary>
    public class ConfigurationReader : IConfigurationReader
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ConfigurationReader));

        private static readonly object lockObject = new object();
        private volatile RandomSimulationConfiguration classNamerConfiguration;

        private readonly string settingsFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationReader"/> class.
        /// </summary>
        public ConfigurationReader()
            : this("./appsettings.json")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationReader"/> class.
        /// </summary>
        /// <param name="settingsFile">File to read configuration from.</param>
        protected ConfigurationReader(string settingsFile)
        {
            this.settingsFile = settingsFile;
        }

        /// <inheritdoc/>
        public RandomSimulationConfiguration ReadConfiguration()
        {
            if (this.classNamerConfiguration != null)
            {
                return this.classNamerConfiguration;
            }

            lock (lockObject)
            {
                if (this.classNamerConfiguration != null)
                {
                    return this.classNamerConfiguration;
                }

                log.Info($"Reading configuration from {settingsFile}");

                string jsonText = File.ReadAllText(settingsFile);
                JObject settings = JObject.Parse(jsonText);

                log.Debug($"Configuration from file: {settings.ToString()}");

                this.classNamerConfiguration = settings[nameof(RandomSimulationConfiguration)].ToObject<RandomSimulationConfiguration>();
            }

            return this.classNamerConfiguration;
        }
    }
}