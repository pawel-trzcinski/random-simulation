using NUnit.Framework;
using RandomSimulationEngine.Configuration;

namespace RandomSimulation.Tests.Configuration
{
    [TestFixture]
    public class RandomSimulationConfigurationTests
    {
        [Test]
        public void ValidationOk()
        {
            Assert.DoesNotThrow(() =>
            {
                var unused = new RandomSimulationConfiguration
                (
                    ThrottlingConfigurationTests.CreateCorrectConfiguration(),
                    ImageDownloadConfigurationTests.CreateCorrectConfiguration(),
                    TasksConfigurationTests.CreateCorrectConfiguration(),
                    HistoryConfigurationTests.CreateCorrectConfiguration()
                );
            });
        }
    }
}
