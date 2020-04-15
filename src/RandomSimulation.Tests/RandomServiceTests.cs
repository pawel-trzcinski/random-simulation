using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RandomSimulationEngine.DateTime;
using RandomSimulationEngine.Random;

namespace RandomSimulation.Tests
{
    [TestFixture]
    public class RandomServiceTests
    {
        [Test]
        public void ConsecutiveNextGiveDifferentValues()
        {
            const int numberOfRepeats = 100;
            HashSet<int> testSet = new HashSet<int>(numberOfRepeats);

            RandomService service = new RandomService(new DateTimeService());
            for (int i = 0; i < numberOfRepeats; i++)
            {
                Assert.True(testSet.Add(service.Next()));
            }
        }

        [Test]
        public void ConsecutiveNextMaxGiveDifferentValues()
        {
            const int numberOfRepeats = 100;
            const int max = 10 * 1000 * 1000;
            HashSet<int> testSet = new HashSet<int>(numberOfRepeats);

            RandomService service = new RandomService(new DateTimeService());
            for (int i = 0; i < numberOfRepeats; i++)
            {
                Assert.True(testSet.Add(service.Next(max)));
            }
        }

        [Test]
        public void TheSameSeedGivesTheSameResult()
        {
            const int max = 10 * 1000 * 1000;
            int seed = new Random().Next();

            DateTime currentTime = DateTime.UtcNow;

            Mock<IDateTimeService> dateTimeServiceMock = new Mock<IDateTimeService>();
            // ReSharper disable once AccessToModifiedClosure
            dateTimeServiceMock.Setup(p => p.UtcNow).Returns(() => currentTime);

            RandomService service = new RandomService(dateTimeServiceMock.Object);

            currentTime += RandomService.SeedMinimumLifeTime + TimeSpan.FromSeconds(1);
            service.SetSeed(seed);
            int next = service.Next();
            int nextMax = service.Next(max);

            currentTime += RandomService.SeedMinimumLifeTime + TimeSpan.FromSeconds(1);
            service.SetSeed(seed);
            Assert.AreEqual(next, service.Next());
            Assert.AreEqual(nextMax, service.Next(max));
        }

    }
}