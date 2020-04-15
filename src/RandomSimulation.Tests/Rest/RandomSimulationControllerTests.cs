using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using RandomSimulationEngine.Health;
using RandomSimulationEngine.RandomBytesPuller;
using RandomSimulationEngine.Rest;
using RandomSimulationEngine.ValueCalculator;

namespace RandomSimulation.Tests.Rest
{
    [TestFixture]
    public class RandomSimulationControllerTests
    {
        private static readonly Random _random = new Random();

        [Test]
        public void Test()
        {
            Mock<IValueCalculator> valueCalculatorMock = new Mock<IValueCalculator>();
            Mock<IRandomBytesPuller> randomBytesPullerMock = new Mock<IRandomBytesPuller>();
            Mock<IHealthChecker> healthCheckerMock = new Mock<IHealthChecker>();

            using (RandomSimulationController controller = new RandomSimulationController(valueCalculatorMock.Object, randomBytesPullerMock.Object, healthCheckerMock.Object))
            {
                StatusCodeResult result = controller.Test();

                Assert.AreEqual(200, result.StatusCode);
            }
        }

        [Test]
        public void Next()
        {
            int next = _random.Next();
            byte[] bytes = BitConverter.GetBytes(next);

            Mock<IRandomBytesPuller> randomBytesPullerMock = new Mock<IRandomBytesPuller>();
            randomBytesPullerMock.Setup(p => p.Pull(4)).Returns(bytes);

            Mock<IHealthChecker> healthCheckerMock = new Mock<IHealthChecker>();

            using (RandomSimulationController controller = new RandomSimulationController(new ValueCalculator(), randomBytesPullerMock.Object, healthCheckerMock.Object))
            {
                ContentResult result = controller.Next();

                Assert.AreEqual(next, Convert.ToInt32(result.Content));
            }
        }

        [Test]
        [Repeat(5000)]
        public void NextMax()
        {
            int next = Math.Abs(_random.Next());
            byte[] bytes = BitConverter.GetBytes(next);

            Mock<IRandomBytesPuller> randomBytesPullerMock = new Mock<IRandomBytesPuller>();
            randomBytesPullerMock.Setup(p => p.Pull(4)).Returns(bytes);

            Mock<IValueCalculator> valueCalculatorMock = new Mock<IValueCalculator>();
            valueCalculatorMock.Setup(p => p.GetInt32(bytes, It.IsAny<int>())).Returns(next);

            Mock<IHealthChecker> healthCheckerMock = new Mock<IHealthChecker>();

            using (RandomSimulationController controller = new RandomSimulationController(valueCalculatorMock.Object, randomBytesPullerMock.Object, healthCheckerMock.Object))
            {
                ContentResult result = controller.Next(Math.Abs(_random.Next(1000, 1000 * 1000)));

                Assert.AreEqual(next, Convert.ToInt32(result.Content));
            }
        }

        [Test]
        [Repeat(5000)]
        public void NextMinMax()
        {
            int next = Math.Abs(_random.Next());
            byte[] bytes = BitConverter.GetBytes(next);

            Mock<IRandomBytesPuller> randomBytesPullerMock = new Mock<IRandomBytesPuller>();
            randomBytesPullerMock.Setup(p => p.Pull(4)).Returns(bytes);

            Mock<IValueCalculator> valueCalculatorMock = new Mock<IValueCalculator>();
            valueCalculatorMock.Setup(p => p.GetInt32(bytes, It.IsAny<int>(), It.IsAny<int>())).Returns(next);

            Mock<IHealthChecker> healthCheckerMock = new Mock<IHealthChecker>();

            using (RandomSimulationController controller = new RandomSimulationController(valueCalculatorMock.Object, randomBytesPullerMock.Object, healthCheckerMock.Object))
            {
                ContentResult result = controller.Next(Math.Abs(_random.Next(5, 999)), Math.Abs(_random.Next(1000, 1000 * 1000)));

                Assert.AreEqual(next, Convert.ToInt32(result.Content));
            }
        }

        [Test]
        public void NextDouble()
        {
            double next = _random.NextDouble();
            byte[] bytes = BitConverter.GetBytes(next);

            Mock<IRandomBytesPuller> randomBytesPullerMock = new Mock<IRandomBytesPuller>();
            randomBytesPullerMock.Setup(p => p.Pull(8)).Returns(bytes);

            Mock<IHealthChecker> healthCheckerMock = new Mock<IHealthChecker>();

            using (RandomSimulationController controller = new RandomSimulationController(new ValueCalculator(), randomBytesPullerMock.Object, healthCheckerMock.Object))
            {
                ContentResult result = controller.NextDouble();

                Assert.True(Math.Abs(next - Convert.ToDouble(result.Content)) < Double.Epsilon);
            }
        }

        [Test]
        [Repeat(1000)]
        public void NextBytes()
        {
            byte[] bytes = Helper.GetRandomArray(_random.Next(500, 10000));

            Mock<IRandomBytesPuller> randomBytesPullerMock = new Mock<IRandomBytesPuller>();
            randomBytesPullerMock.Setup(p => p.Pull(bytes.Length)).Returns(bytes);

            Mock<IHealthChecker> healthCheckerMock = new Mock<IHealthChecker>();

            using (RandomSimulationController controller = new RandomSimulationController(new ValueCalculator(), randomBytesPullerMock.Object, healthCheckerMock.Object))
            {
                ContentResult result = controller.NextBytes(bytes.Length);

                Assert.True(bytes.SequenceEqual(Convert.FromBase64String(result.Content)));
            }
        }

        [Test]
        public void HealthDeadReturns500()
        {
            Mock<IRandomBytesPuller> randomBytesPullerMock = new Mock<IRandomBytesPuller>();

            Mock<IHealthChecker> healthCheckerMock = new Mock<IHealthChecker>();
            healthCheckerMock.Setup(p => p.GetHealthStatus()).Returns(HealthStatus.Dead);

            using (RandomSimulationController controller = new RandomSimulationController(new ValueCalculator(), randomBytesPullerMock.Object, healthCheckerMock.Object))
            {
                IActionResult result = controller.Health();
                StatusCodeResult statusResult = result as StatusCodeResult;

                Assert.IsNotNull(statusResult);
                Assert.AreEqual(500, statusResult.StatusCode);
            }
        }

        [TestCase(HealthStatus.AlmostDead)]
        [TestCase(HealthStatus.Healthy)]
        [TestCase(HealthStatus.SoSo)]
        public void HealthReturnItsValue(HealthStatus healthStatus)
        {
            Mock<IRandomBytesPuller> randomBytesPullerMock = new Mock<IRandomBytesPuller>();

            Mock<IHealthChecker> healthCheckerMock = new Mock<IHealthChecker>();
            healthCheckerMock.Setup(p => p.GetHealthStatus()).Returns(healthStatus);

            using (RandomSimulationController controller = new RandomSimulationController(new ValueCalculator(), randomBytesPullerMock.Object, healthCheckerMock.Object))
            {
                IActionResult result = controller.Health();
                ContentResult contentResult = result as ContentResult;

                Assert.IsNotNull(contentResult);
                Assert.AreEqual((int) healthStatus, Convert.ToInt32(contentResult.Content));
            }
        }
    }
}