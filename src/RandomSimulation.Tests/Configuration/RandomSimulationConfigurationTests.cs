using System;
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

        [Test]
        public void Validation_ThrottlingConfigurationNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var unused = new RandomSimulationConfiguration
                (
                    null,
                    ImageDownloadConfigurationTests.CreateCorrectConfiguration(),
                    TasksConfigurationTests.CreateCorrectConfiguration(),
                    HistoryConfigurationTests.CreateCorrectConfiguration()
                );
            });
        }

        [Test]
        public void Validation_ImageDownloadConfigurationNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var unused = new RandomSimulationConfiguration
                (
                    ThrottlingConfigurationTests.CreateCorrectConfiguration(),
                    null,
                    TasksConfigurationTests.CreateCorrectConfiguration(),
                    HistoryConfigurationTests.CreateCorrectConfiguration()
                );
            });
        }

        [Test]
        public void Validation_TasksConfigurationNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var unused = new RandomSimulationConfiguration
                (
                    ThrottlingConfigurationTests.CreateCorrectConfiguration(),
                    ImageDownloadConfigurationTests.CreateCorrectConfiguration(),
                    null,
                    HistoryConfigurationTests.CreateCorrectConfiguration()
                );
            });
        }

        [Test]
        public void Validation_JistorysConfigurationNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var unused = new RandomSimulationConfiguration
                (
                    ThrottlingConfigurationTests.CreateCorrectConfiguration(),
                    ImageDownloadConfigurationTests.CreateCorrectConfiguration(),
                    TasksConfigurationTests.CreateCorrectConfiguration(),
                    null
                );
            });
        }
    }
}