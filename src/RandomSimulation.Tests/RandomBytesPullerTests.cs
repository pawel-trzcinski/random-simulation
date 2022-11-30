using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using RandomSimulationEngine.DateTime;
using RandomSimulationEngine.Exceptions;
using RandomSimulationEngine.Random;
using RandomSimulationEngine.RandomBytesPuller;
using RandomSimulationEngine.Tasks;

namespace RandomSimulation.Tests
{
    [TestFixture]
    public class RandomBytesPullerTests
    {
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(20)]
        public void ExceptionOnNoData(int numberOfProviders)
        {
            RandomBytesPuller puller = new RandomBytesPuller(new RandomService(new DateTimeService()));

            for (int i = 0; i < numberOfProviders; i++)
            {
                Mock<ISingleSourceBytesProvider> sourceMock = new Mock<ISingleSourceBytesProvider>();
                sourceMock.Setup(p => p.IsDataAvailable).Returns(false);
                sourceMock.Setup(p => p.GetBytes(It.IsAny<int>())).Returns(BytesProvidingResult.Empty);

                puller.Register(sourceMock.Object);
            }

            Assert.Throws<NoDataException>(() => puller.Pull(new Random().Next()));
        }

        [Test]
        public void ReturnsOnlyFromAvailableSource()
        {
            RandomBytesPuller puller = new RandomBytesPuller(new RandomService(new DateTimeService()));

            Random random = new Random();
            int okProvidersCount = random.Next(5, 10);
            int notOkProvidersCount = random.Next(5, 10);

            byte[] okArray = Helper.GetRandomArray(1000);
            byte[] notOkArray = Helper.GetRandomArray(1000);

            for (int i = 0; i < okProvidersCount; i++)
            {
                Mock<ISingleSourceBytesProvider> sourceMock = new Mock<ISingleSourceBytesProvider>();
                sourceMock.Setup(p => p.IsDataAvailable).Returns(true);
                sourceMock.Setup(p => p.GetBytes(It.IsAny<int>())).Returns(BytesProvidingResult.Create(okArray));

                puller.Register(sourceMock.Object);
            }

            for (int i = 0; i < notOkProvidersCount; i++)
            {
                Mock<ISingleSourceBytesProvider> sourceMock = new Mock<ISingleSourceBytesProvider>();
                sourceMock.Setup(p => p.IsDataAvailable).Returns(false);
                sourceMock.Setup(p => p.GetBytes(It.IsAny<int>())).Returns(BytesProvidingResult.Create(notOkArray));

                puller.Register(sourceMock.Object);
            }

            Assert.True(puller.Pull(1000).SequenceEqual(okArray));
        }
    }
}