using System.Collections.Generic;
using System.Linq;

namespace RandomSimulationEngine.Health
{
    public class HealthChecker : IHealthChecker
    {
#warning TODO - unit tests
        private readonly  List<IHealthProvider> _providers=new List<IHealthProvider>(50);

        public void Register(IHealthProvider healthProvider)
        {
            _providers.Add(healthProvider);
        }

        public HealthStatus GetHealthStatus()
        {
#warning TEST
            bool[] aliveStatuses = _providers.Select(p => p.IsAlive).ToArray();

            int aliveCount = aliveStatuses.Count(p => p);

            if (aliveCount == aliveStatuses.Length)
            {
                return HealthStatus.Healthy;
            }

            if (aliveCount > aliveStatuses.Length / 2)
            {
                return HealthStatus.SoSo;
            }

            if (aliveStatuses.Length == 0)
            {
                return HealthStatus.Dead;
            }

            if (aliveCount == 0)
            {
                return HealthStatus.Dead;
            }

            return HealthStatus.AlmostDead;
        }
    }
}