using System;
using log4net;
using Newtonsoft.Json;

namespace RandomSimulationEngine.Configuration
{
    public class TasksConfiguration
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(TasksConfiguration));

        public TimeSpan IdleTimeAllowed { get; }

        [JsonConstructor]
        public TasksConfiguration(int idleTimeAllowedS)
        {
            this.IdleTimeAllowed=TimeSpan.FromSeconds(idleTimeAllowedS);

            ValidateConfiguration();
        }

        private void ValidateConfiguration()
        {
            _log.Debug($"Validating {nameof(TasksConfiguration)}");

            if (IdleTimeAllowed <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(IdleTimeAllowed), "Idle time must be at least 1s");
            }

            _log.Info($"{nameof(TasksConfiguration)} valid");
        }
    }
}