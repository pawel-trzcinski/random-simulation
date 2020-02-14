namespace RandomSimulationEngine.Configuration
{
    /// <summary>
    /// Interface for custom reading appsettings.josn configuration.
    /// </summary>
    public interface IConfigurationReader
    {
        /// <summary>
        /// Reads configuration from appsettings.json file.
        /// </summary>
        /// <returns>Bussiness configuration of the ClassNamer.</returns>
        RandomSimulationConfiguration ReadConfiguration();
    }
}
