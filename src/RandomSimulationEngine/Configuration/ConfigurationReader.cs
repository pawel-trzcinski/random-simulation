using System.Configuration;
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
        private static readonly ILog _log = LogManager.GetLogger(typeof(ConfigurationReader));

        private static readonly object _lockObject = new();
        private volatile RandomSimulationConfiguration? _randomSimulationConfiguration;

        private readonly string _settingsFile;

        public RandomSimulationConfiguration Configuration
        {
            get
            {
                if (this._randomSimulationConfiguration != null)
                {
                    return this._randomSimulationConfiguration;
                }

                lock (_lockObject)
                {
                    if (this._randomSimulationConfiguration != null)
                    {
                        return this._randomSimulationConfiguration;
                    }

                    _log.Info($"Reading configuration from {_settingsFile}");

                    string jsonText = File.ReadAllText(_settingsFile);
                    JObject settings = JObject.Parse(jsonText);

                    _log.Debug($"Configuration from file: {settings}");

                    JToken? configurationObject = settings[nameof(RandomSimulationConfiguration)];
                    if (configurationObject == null)
                    {
                        throw new ConfigurationErrorsException($"Unable to find {nameof(RandomSimulationConfiguration)} token");
                    }

                    this._randomSimulationConfiguration = configurationObject.ToObject<RandomSimulationConfiguration>();
                }

                return this._randomSimulationConfiguration!;
            }
        }

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
            this._settingsFile = settingsFile;
        }
    }
}
