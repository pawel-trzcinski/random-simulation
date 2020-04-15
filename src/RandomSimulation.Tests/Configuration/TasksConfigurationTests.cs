using System;
using NUnit.Framework;
using RandomSimulationEngine.Configuration;

namespace RandomSimulation.Tests.Configuration
{
    public class TasksConfigurationTests
    {
        private static TasksConfiguration CreateConfiguration(int idleTimeAllowedS = 5)
        {
            return new TasksConfiguration(idleTimeAllowedS);
        }

        public static TasksConfiguration CreateCorrectConfiguration()
        {
            return CreateConfiguration();
        }

        [Test]
        public void ValidationOk()
        {
            Assert.DoesNotThrow(() => CreateConfiguration());
        }

        [Test]
        public void Validation_MinimalValues()
        {
            Assert.DoesNotThrow(() => CreateConfiguration(1));
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void ValidationError_ConcurrentRequestsLimit(int idleTimeAllowedS)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateConfiguration(idleTimeAllowedS));
        }
    }
}