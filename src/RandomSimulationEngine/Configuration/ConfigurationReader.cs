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

        private static readonly object _lockObject = new object();
        private volatile RandomSimulationConfiguration _classNamerConfiguration;

        private readonly string _settingsFile;

        public RandomSimulationConfiguration Configuration
        {
            get
            {
                if (this._classNamerConfiguration != null)
                {
                    return this._classNamerConfiguration;
                }

                lock (_lockObject)
                {
                    if (this._classNamerConfiguration != null)
                    {
                        return this._classNamerConfiguration;
                    }

                    _log.Info($"Reading configuration from {_settingsFile}");

                    string jsonText = File.ReadAllText(_settingsFile);
                    JObject settings = JObject.Parse(jsonText);

                    _log.Debug($"Configuration from file: {settings}");

                    this._classNamerConfiguration = settings[nameof(RandomSimulationConfiguration)]
                        .ToObject<RandomSimulationConfiguration>();
                }

                return this._classNamerConfiguration;
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