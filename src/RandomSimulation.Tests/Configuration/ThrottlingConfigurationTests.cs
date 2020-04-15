using System;
using NUnit.Framework;
using RandomSimulationEngine.Configuration;

namespace RandomSimulation.Tests.Configuration
{
    [TestFixture]
    public class ThrottlingConfigurationTests
    {
        private static ThrottlingConfiguration CreateConfiguration
        (
            int concurrentRequestsLimit = 5,
            int queueLimit = 50,
            int queueTimeoutS = 30,
            int maximumServerConnections = 100
        )
        {
            return new ThrottlingConfiguration(concurrentRequestsLimit, queueLimit, queueTimeoutS, maximumServerConnections);
        }

        public static ThrottlingConfiguration CreateCorrectConfiguration()
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
                concurrentRequestsLimit: 1,
                queueLimit: 0,
                queueTimeoutS: 5,
                maximumServerConnections: 1
            ));
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void ValidationError_ConcurrentRequestsLimit(int concurrentRequestsLimit)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateConfiguration(concurrentRequestsLimit: concurrentRequestsLimit));
        }

        [TestCase(-2)]
        [TestCase(-1)]
        public void ValidationError_QueueLimit(int queueLimit)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateConfiguration(queueLimit: queueLimit));
        }

        [TestCase(-2)]
        [TestCase(0)]
        [TestCase(4)]
        public void ValidationError_QueueTimeout(int queueTimeoutS)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateConfiguration(queueTimeoutS: queueTimeoutS));
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void ValidationError_MaximumServerConnections(int maximumServerConnections)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateConfiguration(maximumServerConnections: maximumServerConnections));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 2, 2)]
        [TestCase(2, 1, 2)]
        [TestCase(2, 2, 3)]
        public void ValidationError_ConnectionsMustBeEnoughForQueueAndExecution(int concurrentRequestsLimit, int queueLimit, int maximumServerConnections)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateConfiguration
            (
                concurrentRequestsLimit: concurrentRequestsLimit,
                queueLimit: queueLimit,
                maximumServerConnections: maximumServerConnections
            ));
        }
    }
}