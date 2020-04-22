using System;
using NUnit.Framework;
using RandomSimulationEngine.Configuration;

namespace RandomSimulation.Tests.Configuration
{
    [TestFixture]
    public class HistoryConfigurationTests
    {
        private static HistoryConfiguration CreateConfiguration(int intSamples=1000, int doubleSamples=1000, int byteSamples=6000)
        {
            return new HistoryConfiguration(intSamples, doubleSamples, byteSamples);
        }

        public static HistoryConfiguration CreateCorrectConfiguration()
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
            Assert.DoesNotThrow(() => CreateConfiguration
            (
                intSamples: HistoryConfiguration.MIN_SAMPLES,
                doubleSamples: HistoryConfiguration.MIN_SAMPLES,
                byteSamples: HistoryConfiguration.MIN_SAMPLES
            ));
        }

        [Test]
        public void Validation_MaximalValues()
        {
            Assert.DoesNotThrow(() => CreateConfiguration
            (
                intSamples: HistoryConfiguration.MAX_SAMPLES,
                doubleSamples: HistoryConfiguration.MAX_SAMPLES,
                byteSamples: HistoryConfiguration.MAX_SAMPLES
            ));
        }

        [TestCase(HistoryConfiguration.MIN_SAMPLES-1)]
        [TestCase(HistoryConfiguration.MAX_SAMPLES+1)]
        public void ValidationError_IntSamples(int intSamples)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateConfiguration(intSamples: intSamples));
        }

        [TestCase(HistoryConfiguration.MIN_SAMPLES - 1)]
        [TestCase(HistoryConfiguration.MAX_SAMPLES + 1)]
        public void ValidationError_DoubleSamples(int doubleSamples)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateConfiguration(intSamples: doubleSamples));
        }

        [TestCase(HistoryConfiguration.MIN_SAMPLES - 1)]
        [TestCase(HistoryConfiguration.MAX_SAMPLES + 1)]
        public void ValidationError_ByteSamples(int byteSamples)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateConfiguration(intSamples: byteSamples));
        }
    }
}