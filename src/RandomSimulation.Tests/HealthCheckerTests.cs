using Moq;
using NUnit.Framework;
using RandomSimulationEngine.Health;

namespace RandomSimulation.Tests
{
    [TestFixture]
    public class HealthCheckerTests
    {
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(30)]
        public void AllAlive(int providersCount)
        {
            HealthChecker checker = new HealthChecker();

            for (int i = 0; i < providersCount; i++)
            {
                Mock<IHealthProvider> healthProviderMock = new Mock<IHealthProvider>();
                healthProviderMock.Setup(p => p.IsAlive).Returns(true);
                checker.Register(healthProviderMock.Object);
            }

            Assert.That(checker.GetHealthStatus(), Is.EqualTo(HealthStatus.Healthy));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(30)]
        public void AllDead(int providersCount)
        {
            HealthChecker checker = new HealthChecker();

            for (int i = 0; i < providersCount; i++)
            {
                Mock<IHealthProvider> healthProviderMock = new Mock<IHealthProvider>();
                healthProviderMock.Setup(p => p.IsAlive).Returns(false);
                checker.Register(healthProviderMock.Object);
            }

            Assert.That(checker.GetHealthStatus(), Is.EqualTo(HealthStatus.Dead));
        }

        [TestCase(4)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(30)]
        public void MoreThanHalfAlive(int providersCount)
        {
            HealthChecker checker = new HealthChecker();

            int aliveCount = providersCount / 2 + 1;
            int deadCount = providersCount - aliveCount;

            for (int i = 0; i < aliveCount; i++)
            {
                Mock<IHealthProvider> healthProviderMock = new Mock<IHealthProvider>();
                healthProviderMock.Setup(p => p.IsAlive).Returns(true);
                checker.Register(healthProviderMock.Object);
            }

            for (int i = 0; i < deadCount; i++)
            {
                Mock<IHealthProvider> healthProviderMock = new Mock<IHealthProvider>();
                healthProviderMock.Setup(p => p.IsAlive).Returns(false);
                checker.Register(healthProviderMock.Object);
            }

            Assert.That(checker.GetHealthStatus(), Is.EqualTo(HealthStatus.SoSo));
        }

        [TestCase(4)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(30)]
        public void LessThanHalfAlive(int providersCount)
        {
            HealthChecker checker = new HealthChecker();

            int deadCount = providersCount / 2 + 1;
            int aliveCount = providersCount - deadCount;

            for (int i = 0; i < aliveCount; i++)
            {
                Mock<IHealthProvider> healthProviderMock = new Mock<IHealthProvider>();
                healthProviderMock.Setup(p => p.IsAlive).Returns(true);
                checker.Register(healthProviderMock.Object);
            }

            for (int i = 0; i < deadCount; i++)
            {
                Mock<IHealthProvider> healthProviderMock = new Mock<IHealthProvider>();
                healthProviderMock.Setup(p => p.IsAlive).Returns(false);
                checker.Register(healthProviderMock.Object);
            }

            Assert.That(checker.GetHealthStatus(), Is.EqualTo(HealthStatus.AlmostDead));
        }
    }
}