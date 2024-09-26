using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RandomSimulation.Tests.Configuration;
using RandomSimulationEngine.Configuration;
using RandomSimulationEngine.Random;
using RandomSimulationEngine.Tasks;
using RandomSimulationEngine.Tasks.Specific;
using RandomSimulationEngine.Tasks.Specific.ImageDownload.WebClientWrapper;

namespace RandomSimulation.Tests.Tasks.ImageDownload
{
    [TestFixture]
    public class ImageDownloadTaskTests
    {
        private class ImageDownloadTaskTester : ImageDownloadTask
        {
            public ImageDownloadTaskTester(IWebClientWrapper webClientWrapper, IRandomService randomService, IConfigurationReader configurationReader)
                : base(webClientWrapper, randomService, configurationReader, "WHATEVER")
            {
            }
        }

        [Test]
        public void TaskDownloadsImagesAndCreatesRandomBytes()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            int bytesPullCount = 0;

            Mock<IWebClientWrapper> webClientWrapperMock = new Mock<IWebClientWrapper>();
            webClientWrapperMock.Setup(p => p.GetImageBytes(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(Helper.GetRandomArray(5000)));

            bool randomSeedSet = false;
            Mock<IRandomService> randomServiceMock = new Mock<IRandomService>();
            randomServiceMock.Setup(p => p.Next()).Returns(1);
            randomServiceMock.Setup(p => p.Next(It.IsAny<int>())).Returns(1);
            randomServiceMock.Setup(p => p.SetSeed(It.IsAny<int>())).Callback<int>(_ => randomSeedSet = true);

            var configuration = new RandomSimulationConfiguration
            (
                ThrottlingConfigurationTests.CreateCorrectConfiguration(),
                ImageDownloadConfigurationTests.CreateCorrectConfiguration(downloadIntervalOverride: 2),
                TasksConfigurationTests.CreateCorrectConfiguration(),
                HistoryConfigurationTests.CreateCorrectConfiguration()
            );

            Mock<IConfigurationReader> configurationReaderMock = new Mock<IConfigurationReader>();
            configurationReaderMock.Setup(p => p.Configuration).Returns(configuration);

            ImageDownloadTaskTester tester = new ImageDownloadTaskTester(webClientWrapperMock.Object, randomServiceMock.Object, configurationReaderMock.Object);
            tester.ExecutionFinished += (_, _) =>
            {
                ++bytesPullCount;

                if (bytesPullCount == 2)
                {
                    source.Cancel();
                }
            };

            Assert.That(tester.IsDataAvailable, Is.False);
            Assert.That(tester.GetBytes(20).IsDtataAvailable, Is.False);

            tester.Start(source.Token);

            source.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(3600));

            Assert.That(tester.IsDataAvailable);
            Assert.That(randomSeedSet);
            Assert.That(tester.IsRunning, Is.False);

            BytesProvidingResult result = tester.GetBytes(20);
            Assert.That(result.IsDtataAvailable);
            Assert.That(result.Data.Count, Is.EqualTo(20));
        }
    }
}