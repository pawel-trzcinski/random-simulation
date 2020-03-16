using System;
using Newtonsoft.Json;

namespace RandomSimulationEngine.Configuration
{
    public class TasksConfiguration
    {
        public TimeSpan IdleTimeAllowed { get; }

        [JsonConstructor]
        public TasksConfiguration(int idleTimeAllowedS)
        {
            this.IdleTimeAllowed=TimeSpan.FromSeconds(idleTimeAllowedS);

            ValidateConfiguration();
        }

        private void ValidateConfiguration()
        {
            if (IdleTimeAllowed <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(IdleTimeAllowed), "Idle time must be at least 1s");
            }
        }
    }
}