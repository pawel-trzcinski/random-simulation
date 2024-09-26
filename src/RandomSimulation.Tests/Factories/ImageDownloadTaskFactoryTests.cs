using System;
using Moq;
using NUnit.Framework;
using RandomSimulation.Tests.Configuration;
using RandomSimulationEngine.Configuration;
using RandomSimulationEngine.Configuration.ImageDownload;
using RandomSimulationEngine.Factories.ImageDownload;
using RandomSimulationEngine.Random;
using RandomSimulationEngine.Tasks.Specific;

namespace RandomSimulation.Tests.Factories
{
    [TestFixture]
    public class ImageDownloadTaskFactoryTests
    {
        [Test]
        public void FactoryCreatesCorrectObject()
        {
            Random random = new Random();
            int next1 = random.Next(1000);
            int next2 = random.Next(1000);
            int next3 = random.Next(1000);
            int next4 = random.Next(1000);
            string url = Guid.NewGuid().ToString("N");

            Mock<IRandomService> randomServiceMock = new Mock<IRandomService>();
            randomServiceMock.Setup(p => p.Next()).Returns(next1);

            RandomSimulationConfiguration configuration = new RandomSimulationConfiguration
            (
                new ThrottlingConfiguration(next2, 1000, 500, 5000),
                new ImageDownloadConfiguration(new[] {"wp.pl"}, next3, 1, 1500),
                new TasksConfiguration(next4),
                HistoryConfigurationTests.CreateCorrectConfiguration()
            );

            Mock<IConfigurationReader> configurationReaderMock = new Mock<IConfigurationReader>();
            configurationReaderMock.Setup(p => p.Configuration).Returns(configuration);

            ImageDownloadTaskFactory factory = new ImageDownloadTaskFactory(randomServiceMock.Object, configurationReaderMock.Object);
            Assert.That(factory.GetNewTask(url) is ImageDownloadTask);
        }
    }
}