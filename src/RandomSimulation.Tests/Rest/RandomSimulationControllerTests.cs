﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using RandomSimulationEngine.Health;
using RandomSimulationEngine.History;
using RandomSimulationEngine.RandomBytesPuller;
using RandomSimulationEngine.Rest;
using RandomSimulationEngine.ValueCalculator;

namespace RandomSimulation.Tests.Rest
{
    [TestFixture]
    public class RandomSimulationControllerTests
    {
        private static readonly Random _random = new();

        [Test]
        public void Test()
        {
            Mock<IValueCalculator> valueCalculatorMock = new();
            Mock<IRandomBytesPuller> randomBytesPullerMock = new();
            Mock<IHealthChecker> healthCheckerMock = new();
            Mock<IHistoryStorage> historyStorage = new();

            using (RandomSimulationController controller = new RandomSimulationController(valueCalculatorMock.Object, randomBytesPullerMock.Object, healthCheckerMock.Object, historyStorage.Object))
            {
                StatusCodeResult result = controller.Test();

                Assert.That(result.StatusCode, Is.EqualTo(200));
            }
        }

        [Test]
        public void Next()
        {
            int next = _random.Next();
            byte[] bytes = BitConverter.GetBytes(next);

            Mock<IRandomBytesPuller> randomBytesPullerMock = new();
            randomBytesPullerMock.Setup(p => p.Pull(4)).Returns(bytes);

            Mock<IHealthChecker> healthCheckerMock = new();
            Mock<IHistoryStorage> historyStorage = new();

            bool storeExecuted = false;
            historyStorage.Setup(p => p.StoreNext(It.IsAny<int>())).Callback<int>(_ => storeExecuted = true);

            using (RandomSimulationController controller = new RandomSimulationController(new ValueCalculator(), randomBytesPullerMock.Object, healthCheckerMock.Object, historyStorage.Object))
            {
                ContentResult result = controller.Next();

                Assert.That(Convert.ToInt32(result.Content), Is.EqualTo(next));
                Assert.That(storeExecuted);
            }
        }

        [Test]
        [Repeat(5000)]
        public void NextMax()
        {
            int next = Math.Abs(_random.Next());
            byte[] bytes = BitConverter.GetBytes(next);

            Mock<IRandomBytesPuller> randomBytesPullerMock = new();
            randomBytesPullerMock.Setup(p => p.Pull(4)).Returns(bytes);

            Mock<IValueCalculator> valueCalculatorMock = new();
            valueCalculatorMock.Setup(p => p.GetInt32(bytes, It.IsAny<int>())).Returns(next);

            Mock<IHealthChecker> healthCheckerMock = new();
            Mock<IHistoryStorage> historyStorage = new();

            bool storeExecuted = false;
            historyStorage.Setup(p => p.StoreNextMax(It.IsAny<int>())).Callback<int>(_ => storeExecuted = true);

            using (RandomSimulationController controller = new RandomSimulationController(valueCalculatorMock.Object, randomBytesPullerMock.Object, healthCheckerMock.Object, historyStorage.Object))
            {
                ContentResult result = controller.Next(Math.Abs(_random.Next(1000, 1000 * 1000)));

                Assert.That(Convert.ToInt32(result.Content), Is.EqualTo(next));
                Assert.That(storeExecuted);
            }
        }

        [Test]
        [Repeat(5000)]
        public void NextMinMax()
        {
            int next = Math.Abs(_random.Next());
            byte[] bytes = BitConverter.GetBytes(next);

            Mock<IRandomBytesPuller> randomBytesPullerMock = new();
            randomBytesPullerMock.Setup(p => p.Pull(4)).Returns(bytes);

            Mock<IValueCalculator> valueCalculatorMock = new();
            valueCalculatorMock.Setup(p => p.GetInt32(bytes, It.IsAny<int>(), It.IsAny<int>())).Returns(next);

            Mock<IHealthChecker> healthCheckerMock = new();
            Mock<IHistoryStorage> historyStorage = new();

            bool storeExecuted = false;
            historyStorage.Setup(p => p.StoreNextMinMax(It.IsAny<int>())).Callback<int>(_ => storeExecuted = true);

            using (RandomSimulationController controller = new RandomSimulationController(valueCalculatorMock.Object, randomBytesPullerMock.Object, healthCheckerMock.Object, historyStorage.Object))
            {
                ContentResult result = controller.Next(Math.Abs(_random.Next(5, 999)), Math.Abs(_random.Next(1000, 1000 * 1000)));

                Assert.That(Convert.ToInt32(result.Content), Is.EqualTo(next));
                Assert.That(storeExecuted);
            }
        }

        [Test]
        public void NextDouble()
        {
            double next = _random.NextDouble();
            byte[] bytes = BitConverter.GetBytes(next);

            Mock<IRandomBytesPuller> randomBytesPullerMock = new();
            randomBytesPullerMock.Setup(p => p.Pull(8)).Returns(bytes);

            Mock<IHealthChecker> healthCheckerMock = new();
            Mock<IHistoryStorage> historyStorage = new();

            bool storeExecuted = false;
            historyStorage.Setup(p => p.StoreNextDouble(It.IsAny<double>())).Callback<double>(_ => storeExecuted = true);

            using (RandomSimulationController controller = new RandomSimulationController(new ValueCalculator(), randomBytesPullerMock.Object, healthCheckerMock.Object, historyStorage.Object))
            {
                ContentResult result = controller.NextDouble();

                Assert.That(Math.Abs(next - Convert.ToDouble(result.Content)) < Double.Epsilon);
                Assert.That(storeExecuted);
            }
        }

        [Test]
        [Repeat(1000)]
        public void NextBytes()
        {
            byte[] bytes = Helper.GetRandomArray(_random.Next(500, 10000));

            Mock<IRandomBytesPuller> randomBytesPullerMock = new();
            randomBytesPullerMock.Setup(p => p.Pull(bytes.Length)).Returns(bytes);

            Mock<IHealthChecker> healthCheckerMock = new();
            Mock<IHistoryStorage> historyStorage = new();

            bool storeExecuted = false;
            historyStorage.Setup(p => p.StoreNextBytes(It.IsAny<byte[]>())).Callback<byte[]>(_ => storeExecuted = true);

            using (RandomSimulationController controller = new RandomSimulationController(new ValueCalculator(), randomBytesPullerMock.Object, healthCheckerMock.Object, historyStorage.Object))
            {
                ContentResult result = controller.NextBytes(bytes.Length);
                
                Assert.That(result.Content, Is.Not.Null);
                Assert.That(bytes.SequenceEqual(Convert.FromBase64String(result.Content!)));
                Assert.That(storeExecuted);
            }
        }

        [Test]
        public void HealthDeadReturns500()
        {
            Mock<IRandomBytesPuller> randomBytesPullerMock = new();

            Mock<IHealthChecker> healthCheckerMock = new();
            healthCheckerMock.Setup(p => p.GetHealthStatus()).Returns(HealthStatus.Dead);

            Mock<IHistoryStorage> historyStorage = new();

            using (RandomSimulationController controller = new RandomSimulationController(new ValueCalculator(), randomBytesPullerMock.Object, healthCheckerMock.Object, historyStorage.Object))
            {
                IActionResult result = controller.Health();
                StatusCodeResult? statusResult = result as StatusCodeResult;

                Assert.That(statusResult, Is.Not.Null);
                Assert.That(statusResult!.StatusCode, Is.EqualTo(500));
            }
        }

        [TestCase(HealthStatus.AlmostDead)]
        [TestCase(HealthStatus.Healthy)]
        [TestCase(HealthStatus.SoSo)]
        public void HealthReturnItsValue(HealthStatus healthStatus)
        {
            Mock<IRandomBytesPuller> randomBytesPullerMock = new();

            Mock<IHealthChecker> healthCheckerMock = new();
            healthCheckerMock.Setup(p => p.GetHealthStatus()).Returns(healthStatus);

            Mock<IHistoryStorage> historyStorage = new();

            using (RandomSimulationController controller = new RandomSimulationController(new ValueCalculator(), randomBytesPullerMock.Object, healthCheckerMock.Object, historyStorage.Object))
            {
                IActionResult result = controller.Health();
                ContentResult? contentResult = result as ContentResult;

                Assert.That(contentResult, Is.Not.Null);
                Assert.That(Convert.ToInt32(contentResult!.Content), Is.EqualTo((int)healthStatus));
            }
        }

        [Test]
        public void Histogram()
        {
            string content = Guid.NewGuid().ToString();

            Mock<IRandomBytesPuller> randomBytesPullerMock = new();
            Mock<IHealthChecker> healthCheckerMock = new();
            Mock<IHistoryStorage> historyStorage = new();
            historyStorage.Setup(p => p.GetHistogramReport(It.IsAny<int>())).Returns(content);

            using (RandomSimulationController controller = new RandomSimulationController(new ValueCalculator(), randomBytesPullerMock.Object, healthCheckerMock.Object, historyStorage.Object))
            {
                ContentResult result = controller.Histogram(50);

                Assert.That(content, Is.EqualTo(result.Content));
            }
        }
    }
}